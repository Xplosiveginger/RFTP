using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour
{
    private const string RESOURCE_PATH = "PersistentObject";

    void Awake()
    {
        if (PersistentObject.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(RESOURCE_PATH);

            if (prefab == null)
            {
                Debug.LogError("PersistentObject prefab not found in Resources!");
                return;
            }

            Instantiate(prefab);
        }
    }
}