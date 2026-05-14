using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Sirenix.OdinInspector;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Title("Phase Configuration")]
    [ListDrawerSettings(Expanded = true, ShowIndexLabels = true)]
    public List<SpawnPhaseSO> spawnPhases;

    [Title("Spawn Settings")]
    [PropertyRange(5f, 20f)]
    public float spawnDistance = 10f;

    [Required]
    public Transform poolParent;

    [Title("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject EndPanel;

    [Title("Special Enemies")]
    [HorizontalGroup("Atom")]
    [PreviewField(50)]
    [HideLabel]
    public GameObject atomPrefab;

    [VerticalGroup("Atom/Info")]
    [LabelText("Pool Size")]
    [MinValue(1)]
    public int atomPoolSize = 5;

    [Title("Debug")]
    [HorizontalGroup("Debug")]
    public bool debugMode = false;

    [HorizontalGroup("Debug")]
    [ShowIf("debugMode")]
    [PropertyRange(0f, 900f)]
    public float debugStartTime = 0f;

    private Camera mainCamera;
    private float elapsedTime = 0f;
    private int currentPhaseIndex = -1;
    private SpawnPhaseSO currentPhase;

    private readonly Dictionary<EnemySpawnDataNew, DynamicEnemyPooler> poolDictionary =
        new Dictionary<EnemySpawnDataNew, DynamicEnemyPooler>();
    private DynamicEnemyPooler atomPooler;

    private float spawnTimer = 0f;

    [Title("Runtime Info")]
    [ShowInInspector, ReadOnly]
    private string CurrentPhaseInfo => currentPhase != null ?
        $"Phase {currentPhaseIndex}: {currentPhase.name}" : "No active phase";

    [ShowInInspector, ReadOnly]
    private string SpawnRateInfo => currentPhase != null ?
        $"{currentPhase.spawnsPerSecond} enemies/s" : "N/A";

    [ShowInInspector, ReadOnly]
    private int TotalActiveEnemies
    {
        get
        {
            int total = 0;
            foreach (var pool in poolDictionary.Values)
                total += pool.ActiveCount;
            return total;
        }
    }

    private void Awake()
    {
        Instance = this;
        atomPooler = new DynamicEnemyPooler(atomPrefab, atomPoolSize, atomPoolSize * 2, poolParent);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        elapsedTime = debugMode ? debugStartTime : 0f;

        if (spawnPhases.Count > 0)
        {
            int startPhase = 0;
            for (int i = 0; i < spawnPhases.Count; i++)
                if (elapsedTime >= spawnPhases[i].startTime)
                    startPhase = i;

            ActivatePhase(startPhase);
        }

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!debugMode)
            elapsedTime += Time.deltaTime;

        UpdateTimerUI();

        if (elapsedTime >= 901)
            EndPanel.SetActive(true);

        // Check for phase transition
        if (currentPhaseIndex + 1 < spawnPhases.Count &&
            elapsedTime >= spawnPhases[currentPhaseIndex + 1].startTime)
        {
            ActivatePhase(currentPhaseIndex + 1);
        }

        // Handle spawning
        if (currentPhase != null)
        {
            HandleSpawning();
        }
    }

    private void HandleSpawning()
    {
        if (currentPhase.spawnsPerSecond <= 0 || currentPhase.enemiesToSpawn.Count == 0)
            return;

        float spawnInterval = 1f / currentPhase.spawnsPerSecond;
        spawnTimer += Time.deltaTime;

        while (spawnTimer >= spawnInterval)
        {
            SpawnWeightedEnemy();
            spawnTimer -= spawnInterval;
        }
    }

    private void SpawnWeightedEnemy()
    {
        EnemySpawnDataNew selectedData = GetWeightedRandomEnemy();
        if (selectedData == null || selectedData.enemyPrefab == null)
        {
            Debug.LogWarning("Failed to select enemy to spawn!");
            return;
        }

        if (!poolDictionary.ContainsKey(selectedData))
        {
            Debug.LogError($"No pool found for enemy: {selectedData.enemyPrefab.name}");
            return;
        }

        DynamicEnemyPooler pool = poolDictionary[selectedData];
        GameObject enemyObj = pool.Get();

        if (enemyObj == null)
        {
            Debug.LogError($"Failed to get enemy from pool: {selectedData.enemyPrefab.name}");
            return;
        }

        Vector3 spawnPos = GetRandomPositionOutsideCamera();
        enemyObj.transform.position = spawnPos;
        enemyObj.SetActive(true);

        BaseEnemyRefactor enemy = enemyObj.GetComponent<BaseEnemyRefactor>();
        if (enemy != null)
        {
            EnemyManager.Instance.RegisterEnemy(enemy);
        }
    }

    private EnemySpawnDataNew GetWeightedRandomEnemy()
    {
        if (currentPhase == null || currentPhase.enemiesToSpawn.Count == 0)
            return null;

        float totalWeight = currentPhase.TotalWeight;
        if (totalWeight <= 0f)
        {
            Debug.LogWarning("Total weight is 0, cannot select weighted enemy!");
            return null;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var enemyData in currentPhase.enemiesToSpawn)
        {
            currentWeight += enemyData.weight;
            if (randomValue <= currentWeight)
            {
                return enemyData;
            }
        }

        // Fallback to first enemy if something goes wrong
        return currentPhase.enemiesToSpawn[0];
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int m = Mathf.FloorToInt(elapsedTime / 60);
        int s = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = $"{m:00}:{s:00}";
    }

    void ActivatePhase(int phaseIndex)
    {
        currentPhaseIndex = phaseIndex;
        currentPhase = spawnPhases[phaseIndex];

        // Clear old pools if needed
        // Since we removed keepPreviousEnemiesAlive, we can optionally clear old pools
        // based on your game logic. For now, we'll keep them but you might want to add
        // logic to clean up pools from phases that are no longer needed.

        // Create pools for new phase enemies if they don't exist
        foreach (var enemyData in currentPhase.enemiesToSpawn)
        {
            if (!poolDictionary.ContainsKey(enemyData))
            {
                DynamicEnemyPooler pool = new DynamicEnemyPooler(
                    enemyData.enemyPrefab,
                    enemyData.initialPoolSize,
                    enemyData.maxPoolSize,
                    poolParent
                );
                poolDictionary[enemyData] = pool;
            }
        }

        // Reset spawn timer when phase changes
        spawnTimer = 0f;

        Debug.Log($"[EnemySpawner] Activated Phase {phaseIndex} at {elapsedTime:F1}s | " +
                  $"{currentPhase.spawnsPerSecond} enemies/s | " +
                  $"Total Weight: {currentPhase.TotalWeight:F2}");
    }

    Vector3 GetRandomPositionOutsideCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        int side = Random.Range(0, 2);
        float x = (side == 0 ? -camWidth / 2 - spawnDistance : camWidth / 2 + spawnDistance);
        float y = Random.Range(-camHeight / 2, camHeight / 2);

        Vector3 camPos = mainCamera.transform.position;
        Vector3 randomPos = new Vector3(camPos.x + x, camPos.y + y, 0f);

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            return hit.position;

        randomPos.x = camPos.x + (side == 0 ? -camWidth / 2 : camWidth / 2);
        if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
            return hit.position;

        return camPos;
    }

    // -------------------------------
    // Atom Enemy Spawn (called from EnemyManager)
    // -------------------------------
    public void SpawnAtom(Vector3 position)
    {
        GameObject atom = atomPooler.Get();
        atom.transform.position = position;
        atom.SetActive(true);

        BaseEnemyRefactor enemy = atom.GetComponent<BaseEnemyRefactor>();
        EnemyManager.Instance.RegisterEnemy(enemy);
    }

    /// <summary>
    /// Call this when an enemy is despawned to return it to the appropriate pool
    /// </summary>
    public void ReturnEnemyToPool(BaseEnemyRefactor enemy)
    {
        if (enemy == null) return;

        // Find which pool this enemy belongs to
        foreach (var kvp in poolDictionary)
        {
            // You might want to add a more efficient way to identify which pool an enemy belongs to
            // For now, we'll check by prefab name or add a component reference
            if (enemy.gameObject.name.Contains(kvp.Key.enemyPrefab.name))
            {
                kvp.Value.ReturnToPool(enemy.gameObject);
                return;
            }
        }

        Debug.LogWarning($"Could not find pool for enemy: {enemy.name}");
    }

    [Title("Debug Actions")]
    [Button("Log Pool Status")]
    private void LogPoolStatus()
    {
        Debug.Log("=== Pool Status ===");
        foreach (var kvp in poolDictionary)
        {
            Debug.Log($"{kvp.Key.enemyPrefab.name}: {kvp.Value.GetPoolInfo()}");
        }
        Debug.Log($"Total Active Enemies: {TotalActiveEnemies}");
    }

    [Button("Test Weight Distribution")]
    private void TestWeightDistribution()
    {
        if (currentPhase == null)
        {
            Debug.LogWarning("No active phase to test!");
            return;
        }

        Dictionary<string, int> spawnCounts = new Dictionary<string, int>();
        int testCount = 1000;

        for (int i = 0; i < testCount; i++)
        {
            EnemySpawnDataNew selected = GetWeightedRandomEnemy();
            if (selected != null)
            {
                string name = selected.enemyPrefab != null ? selected.enemyPrefab.name : "Unknown";
                if (!spawnCounts.ContainsKey(name))
                    spawnCounts[name] = 0;
                spawnCounts[name]++;
            }
        }

        Debug.Log($"Weight Distribution Test ({testCount} spawns):");
        foreach (var kvp in spawnCounts)
        {
            float percentage = (float)kvp.Value / testCount * 100f;
            Debug.Log($"  {kvp.Key}: {kvp.Value} ({percentage:F1}%)");
        }
    }
}