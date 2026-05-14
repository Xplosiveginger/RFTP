using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Performance Settings")]
    [SerializeField] private float combinationCheckInterval = 0.5f;
    [SerializeField] private int maxEnemiesForCombination = 50;

    // Main enemy collection - single source of truth
    private readonly HashSet<BaseEnemyRefactor> allEnemies = new HashSet<BaseEnemyRefactor>();

    // Type-specific collections for specialized behaviors
    private readonly List<ENP_Enemy> enpEnemies = new List<ENP_Enemy>();
    private readonly List<MutatedRats_Refactor> mutatedRats = new List<MutatedRats_Refactor>();
    private readonly List<Skeleton_refactor> skeletons = new List<Skeleton_refactor>();
    private readonly List<LitmusPaper_Refactor> litmusPapers = new List<LitmusPaper_Refactor>();

    // Cached player position and timing
    private Vector3 cachedPlayerPosition;
    private float combinationCheckTimer;
    private bool playerIsValid;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerIsValid = player != null;
        if (!playerIsValid)
        {
            Debug.LogWarning("EnemyManager: Player reference is not set!");
        }
    }

    private void Update()
    {
        if (!playerIsValid)
        {
            if (player != null) playerIsValid = true;
            else return;
        }

        // Cache player position once per frame
        cachedPlayerPosition = player.position;
        float deltaTime = Time.deltaTime;

        // Process all enemy updates in a single pass
        ProcessEnemyUpdates(cachedPlayerPosition, deltaTime);
    }

    private void ProcessEnemyUpdates(Vector3 playerPos, float deltaTime)
    {
        // Update movement for all enemies
        UpdateAllEnemyMovement(playerPos);

        // Run expensive combination check on interval
        combinationCheckTimer += deltaTime;
        if (combinationCheckTimer >= combinationCheckInterval && enpEnemies.Count >= 3)
        {
            combinationCheckTimer = 0f;
            CheckENPCombinations(playerPos);
        }

        // Update special enemy behaviors
        UpdateMutatedRats(playerPos, deltaTime);
        UpdateSkeletons(playerPos, deltaTime);
    }

    #region Movement

    private void UpdateAllEnemyMovement(Vector3 playerPos)
    {
        foreach (var enemy in allEnemies)
        {
            if (enemy != null && enemy.isActiveAndEnabled)
            {
                enemy.UpdateMovement(playerPos);
            }
        }
    }

    #endregion

    #region ENP Combination System

    private void CheckENPCombinations(Vector3 playerPos)
    {
        if (enpEnemies.Count > maxEnemiesForCombination)
        {
            // Fallback to sampling if too many enemies
            CheckENPCombinationsSampled(playerPos);
            return;
        }

        // Use spatial hashing or grid for better performance with large numbers
        for (int i = 0; i < enpEnemies.Count; i++)
        {
            ENP_Enemy e1 = enpEnemies[i];
            if (e1 == null || !e1.isActiveAndEnabled) continue;

            for (int j = i + 1; j < enpEnemies.Count; j++)
            {
                ENP_Enemy e2 = enpEnemies[j];
                if (e2 == null || !e2.isActiveAndEnabled) continue;

                float combineRadius = Mathf.Max(e1.combineRadius, e2.combineRadius);
                if (Vector3.SqrMagnitude(e1.transform.position - e2.transform.position) > combineRadius * combineRadius)
                    continue;

                for (int k = j + 1; k < enpEnemies.Count; k++)
                {
                    ENP_Enemy e3 = enpEnemies[k];
                    if (e3 == null || !e3.isActiveAndEnabled) continue;

                    if (Vector3.SqrMagnitude(e1.transform.position - e3.transform.position) >
                        Mathf.Max(e1.combineRadius, e3.combineRadius) * Mathf.Max(e1.combineRadius, e3.combineRadius))
                        continue;

                    if (IsValidCombination(e1, e2, e3))
                    {
                        CombineENPGroup(e1, e2, e3);
                        return; // Only one combination per check
                    }
                }
            }
        }
    }

    private void CheckENPCombinationsSampled(Vector3 playerPos)
    {
        // Sample-based approach for very large enemy counts
        int sampleSize = Mathf.Min(enpEnemies.Count, maxEnemiesForCombination);
        List<ENP_Enemy> sampledEnemies = new List<ENP_Enemy>();

        // Simple random sampling
        for (int i = 0; i < sampleSize; i++)
        {
            int index = Random.Range(0, enpEnemies.Count);
            sampledEnemies.Add(enpEnemies[index]);
        }

        // Check combinations in sampled set
        for (int i = 0; i < sampledEnemies.Count; i++)
        {
            for (int j = i + 1; j < sampledEnemies.Count; j++)
            {
                for (int k = j + 1; k < sampledEnemies.Count; k++)
                {
                    if (IsValidCombination(sampledEnemies[i], sampledEnemies[j], sampledEnemies[k]))
                    {
                        CombineENPGroup(sampledEnemies[i], sampledEnemies[j], sampledEnemies[k]);
                        return;
                    }
                }
            }
        }
    }

    private bool IsValidCombination(ENP_Enemy e1, ENP_Enemy e2, ENP_Enemy e3)
    {
        // Use bit flags for faster type checking
        int typeFlags = 0;
        typeFlags |= (e1.Type == ENPType.Electron) ? 1 : 0;
        typeFlags |= (e1.Type == ENPType.Proton) ? 2 : 0;
        typeFlags |= (e1.Type == ENPType.Neutron) ? 4 : 0;

        typeFlags |= (e2.Type == ENPType.Electron) ? 1 : 0;
        typeFlags |= (e2.Type == ENPType.Proton) ? 2 : 0;
        typeFlags |= (e2.Type == ENPType.Neutron) ? 4 : 0;

        typeFlags |= (e3.Type == ENPType.Electron) ? 1 : 0;
        typeFlags |= (e3.Type == ENPType.Proton) ? 2 : 0;
        typeFlags |= (e3.Type == ENPType.Neutron) ? 4 : 0;

        // Check if we have all three types (flags 1,2,4 = 7)
        return typeFlags == 7;
    }

    private void CombineENPGroup(ENP_Enemy e1, ENP_Enemy e2, ENP_Enemy e3)
    {
        Vector3 spawnPos = (e1.transform.position + e2.transform.position + e3.transform.position) / 3f;
        Debug.Log("🔵 ENP Combination → Atom Spawn!");

        DespawnEnemy(e1);
        DespawnEnemy(e2);
        DespawnEnemy(e3);

        EnemySpawner.Instance.SpawnAtom(spawnPos);
    }

    #endregion

    #region Special Enemy Behaviors

    private void UpdateMutatedRats(Vector3 playerPos, float deltaTime)
    {
        for (int i = mutatedRats.Count - 1; i >= 0; i--)
        {
            if (mutatedRats[i] == null || !mutatedRats[i].isActiveAndEnabled)
            {
                mutatedRats.RemoveAt(i);
                continue;
            }
            mutatedRats[i].CheckAcidSpawn(playerPos, deltaTime);
        }
    }

    private void UpdateSkeletons(Vector3 playerPos, float deltaTime)
    {
        for (int i = skeletons.Count - 1; i >= 0; i--)
        {
            if (skeletons[i] == null || !skeletons[i].isActiveAndEnabled)
            {
                skeletons.RemoveAt(i);
                continue;
            }
            skeletons[i].CheckAttack(playerPos, deltaTime);
        }
    }

    #endregion

    #region Enemy Registration

    public void RegisterEnemy(BaseEnemyRefactor enemy)
    {
        if (enemy == null) return;

        allEnemies.Add(enemy);

        // Register to type-specific lists based on type
        switch (enemy)
        {
            case ENP_Enemy enp:
                enpEnemies.Add(enp);
                break;
            case MutatedRats_Refactor rat:
                mutatedRats.Add(rat);
                break;
            case Skeleton_refactor skel:
                skeletons.Add(skel);
                break;
            case LitmusPaper_Refactor litmus:
                litmusPapers.Add(litmus);
                break;
        }
    }

    public void DespawnEnemy(BaseEnemyRefactor enemy)
    {
        if (enemy == null) return;

        allEnemies.Remove(enemy);

        // Remove from type-specific lists
        switch (enemy)
        {
            case ENP_Enemy enp:
                enpEnemies.Remove(enp);
                break;
            case MutatedRats_Refactor rat:
                mutatedRats.Remove(rat);
                break;
            case Skeleton_refactor skel:
                skeletons.Remove(skel);
                break;
            case LitmusPaper_Refactor litmus:
                litmusPapers.Remove(litmus);
                break;
        }

        // Reset and deactivate
        enemy.ResetOnDeath();

        // Return to pool if available
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.ReturnEnemyToPool(enemy);
        }
        else
        {
            enemy.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Utility Methods

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        playerIsValid = player != null;
    }

    public int GetEnemyCount<T>() where T : BaseEnemyRefactor
    {
        if (typeof(T) == typeof(ENP_Enemy)) return enpEnemies.Count;
        if (typeof(T) == typeof(MutatedRats_Refactor)) return mutatedRats.Count;
        if (typeof(T) == typeof(Skeleton_refactor)) return skeletons.Count;
        if (typeof(T) == typeof(LitmusPaper_Refactor)) return litmusPapers.Count;

        return allEnemies.Count;
    }

    public int TotalEnemyCount => allEnemies.Count;

    public void ClearAllEnemies()
    {
        // Create a copy to avoid modification during iteration
        BaseEnemyRefactor[] enemiesArray = new BaseEnemyRefactor[allEnemies.Count];
        allEnemies.CopyTo(enemiesArray);

        foreach (var enemy in enemiesArray)
        {
            if (enemy != null)
            {
                DespawnEnemy(enemy);
            }
        }
    }

    #endregion
}