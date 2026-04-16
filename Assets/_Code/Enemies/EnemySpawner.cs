using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Phase Setup")]
    public List<SpawnPhaseSO> spawnPhases;

    [Header("Spawn Settings")]
    public float spawnDistance = 10f;
    public Transform poolParent;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject EndPanel;

    [Header("Atom Enemy")]
    public GameObject atomPrefab;
    public int atomPoolSize = 5;

    [Header("Debug Settings")]
    public bool debugMode = false;
    [Range(0f, 900f)]
    public float debugStartTime = 0f;

    private Camera mainCamera;
    private float elapsedTime = 0f;
    private int currentPhaseIndex = -1;

    private readonly List<EnemyPooler> currentPools = new List<EnemyPooler>();
    private EnemyPooler atomPooler;

    private readonly Dictionary<EnemyPooler, float> spawnTimers = new Dictionary<EnemyPooler, float>();
    private readonly Dictionary<EnemyPooler, float> elapsedPoolTime = new Dictionary<EnemyPooler, float>();

    private void Awake()
    {
        Instance = this;
        atomPooler = new EnemyPooler(atomPrefab, atomPoolSize, poolParent);
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

        if (currentPhaseIndex + 1 < spawnPhases.Count &&
            elapsedTime >= spawnPhases[currentPhaseIndex + 1].startTime)
        {
            ActivatePhase(currentPhaseIndex + 1);
        }

        // Spawn enemies from pool
        foreach (var pool in currentPools)
        {
            if (!spawnTimers.ContainsKey(pool))
            {
                spawnTimers[pool] = 0f;
                elapsedPoolTime[pool] = 0f;
            }

            elapsedPoolTime[pool] += Time.deltaTime;

            var data = pool.EnemyData;
            float t = Mathf.Clamp01(elapsedPoolTime[pool] / data.timeToIncrease);
            float currentRate = Mathf.Lerp(data.minSpawnRate, data.maxSpawnRate, t);
            float interval = 1f / currentRate;

            spawnTimers[pool] -= Time.deltaTime;
            if (spawnTimers[pool] <= 0f)
            {
                SpawnFromPool(pool);
                spawnTimers[pool] = interval;
            }
        }
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
        var phase = spawnPhases[phaseIndex];

        if (!phase.keepPreviousEnemiesAlive)
        {
            foreach (var pool in currentPools)
                pool.ClearPool();

            currentPools.Clear();
            spawnTimers.Clear();
            elapsedPoolTime.Clear();
        }

        foreach (var enemyData in phase.enemiesToSpawn)
        {
            EnemyPooler pool = new EnemyPooler(enemyData.enemyPrefab, enemyData.poolSize, poolParent);
            pool.EnemyData = enemyData;
            currentPools.Add(pool);
        }

        Debug.Log($"[EnemySpawner] Activated Phase {phaseIndex} at {elapsedTime:F1}s");
    }

    void SpawnFromPool(EnemyPooler pool)
    {
        GameObject enemyObj = pool.Get();
        enemyObj.transform.position = GetRandomPositionOutsideCamera();
        enemyObj.SetActive(true);

        BaseEnemyRefactor enemy = enemyObj.GetComponent<BaseEnemyRefactor>();
        EnemyManager.Instance.RegisterEnemy(enemy);
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
}
