using System;
using UnityEngine;

public class MutatedRats_Refactor : BaseEnemyRefactor
{
    [Header("Acid Settings")]
    public GameObject acidParticlePrefab;
    public GameObject acidSpritePrefab;
    public float spawnRadius = 3f;
    public float acidSpawnInterval = 2f;

    private float spawnTimer = 0f;

    // Event for acid spawn
    public Action<Vector3> OnSpawnAcid;

    protected override void Awake()
    {
        base.Awake();
        EnemyManager.Instance.RegisterEnemy(this);

        // Hook the default spawn method
        OnSpawnAcid += SpawnAcidPrefabs;
    }

    public void CheckAcidSpawn(Vector3 playerPosition, float deltaTime)
    {
        if (!gameObject.activeInHierarchy) return;

        spawnTimer -= deltaTime;

        if (Vector3.Distance(transform.position, playerPosition) <= spawnRadius && spawnTimer <= 0f)
        {
            OnSpawnAcid?.Invoke(transform.position);
            spawnTimer = acidSpawnInterval;
        }
    }

    private void SpawnAcidPrefabs(Vector3 spawnPos)
    {
        float lifetime = 2f;

        if (acidParticlePrefab != null)
        {
            GameObject particleObj = GameObject.Instantiate(acidParticlePrefab, spawnPos, Quaternion.identity);
            Destroy(particleObj, lifetime);
        }

        if (acidSpritePrefab != null)
        {
            GameObject spriteObj = GameObject.Instantiate(acidSpritePrefab, spawnPos, Quaternion.identity);
            Destroy(spriteObj, lifetime);
        }
    }
}
