using System;
using UnityEngine;

public class MutatedRats_Refactor : BaseEnemyRefactor
{
    private Vector3 visualStartScale;
    private bool facingLeft;

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

        // Store original scale
        visualStartScale = transform.localScale;

        // Keep initial state consistent with BaseEnemyRefactor
        facingLeft = defaultFacingLeft
            ? visualStartScale.x >= 0f
            : visualStartScale.x < 0f;

        // Hook the default spawn method
        OnSpawnAcid += SpawnAcidPrefabs;
    }

    public void CheckAcidSpawn(Vector3 playerPosition, float deltaTime)
    {
        if (!gameObject.activeInHierarchy)
            return;

        // Face the player
        FlipVisual(playerPosition);

        spawnTimer -= deltaTime;

        if (Vector3.Distance(transform.position, playerPosition) <= spawnRadius &&
            spawnTimer <= 0f)
        {
            OnSpawnAcid?.Invoke(transform.position);
            spawnTimer = acidSpawnInterval;
        }
    }

    private void FlipVisual(Vector3 playerPosition)
    {
        bool shouldFaceLeft = playerPosition.x < transform.position.x;

        if (shouldFaceLeft == facingLeft)
            return;

        facingLeft = shouldFaceLeft;

        Vector3 scale = visualStartScale;
        float xScale = Mathf.Abs(visualStartScale.x);

        // A positive X scale faces the prefab's default direction.
        // Respect defaultFacingLeft.
        bool usePositiveXScale = facingLeft == defaultFacingLeft;

        scale.x = usePositiveXScale ? xScale : -xScale;

        transform.localScale = scale;
    }

    private void SpawnAcidPrefabs(Vector3 spawnPos)
    {
        /*
        float lifetime = 2f;

        if (acidParticlePrefab != null)
        {
            GameObject particleObj = GameObject.Instantiate(
                acidParticlePrefab,
                spawnPos,
                Quaternion.identity
            );

            Destroy(particleObj, lifetime);
        }

        if (acidSpritePrefab != null)
        {
            GameObject spriteObj = GameObject.Instantiate(
                acidSpritePrefab,
                spawnPos,
                Quaternion.identity
            );

            Destroy(spriteObj, lifetime);
        }
        */
    }
}