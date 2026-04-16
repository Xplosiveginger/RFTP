using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner2D : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] assetPrefabs;       // Prefab assets to spawn
    public float minInterval = 5f;          // Minimum spawn interval in seconds
    public float maxInterval = 15f;         // Maximum spawn interval in seconds

    // Internal class to track each spawn location
    private class SpawnData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public GameObject prefabReference;  // Store reference to prefab, not instance
        public GameObject currentInstance;  // Store reference to current instance at this location
    }

    private List<SpawnData> spawnLocations = new List<SpawnData>();

    void Start()
    {
        if (assetPrefabs == null || assetPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to EnvironmentSpawner2D!");
            return;
        }

        // Find all instances of assigned prefabs in the scene
        FindAndRecordPrefabInstances();

        // Start independent spawn routines for each location
        foreach (var spawnData in spawnLocations)
        {
            StartCoroutine(SpawnRoutine(spawnData));
        }
    }

    void FindAndRecordPrefabInstances()
    {
        // Find all objects in scene (excluding prefabs in project)
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects)
        {
            // Skip if this is the spawner itself or its children
            if (obj == gameObject || obj.transform.IsChildOf(transform))
                continue;

            // Check if this object matches any of our prefabs by name
            foreach (GameObject prefab in assetPrefabs)
            {
                if (prefab == null) continue;

                // Check if names match (remove "(Clone)" suffix for comparison)
                string objName = obj.name.Replace("(Clone)", "").Trim();
                string prefabName = prefab.name;

                if (objName == prefabName)
                {
                    // Record this spawn location with full transform data, tie to existing scene object!
                    SpawnData data = new SpawnData
                    {
                        position = obj.transform.position,
                        rotation = obj.transform.rotation,
                        scale = obj.transform.localScale,
                        prefabReference = prefab,    // Store the prefab asset reference
                        currentInstance = obj        // Set to *existing scene instance*!
                    };
                    spawnLocations.Add(data);

                    //Debug.Log($"Recorded spawn location for {prefabName} at {obj.transform.position}");
                    break;
                }
            }
        }

        Debug.Log($"Total spawn locations found: {spawnLocations.Count}");
    }

    IEnumerator SpawnRoutine(SpawnData spawnData)
    {
        // A continuous loop checking whether currentInstance exists
        while (true)
        {
            if (spawnData.currentInstance == null)
            {
                // Wait a random interval before respawn
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                // Spawn a new item and update currentInstance
                spawnData.currentInstance = Instantiate(
                    spawnData.prefabReference,
                    spawnData.position,
                    spawnData.rotation
                );
                spawnData.currentInstance.transform.localScale = spawnData.scale;

                Debug.Log($"Spawned {spawnData.prefabReference.name} at {spawnData.position}");
            }

            yield return new WaitForSeconds(1f); // Check every second if instance was destroyed
        }
    }
}
