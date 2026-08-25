using UnityEngine;
using UnityEngine.AI;

public class PendulumEnemy2D : BaseEnemyRefactor
{
    [Header("Movement")]
    public float stoppingDistance = 5f;
    public float attackCooldown = 3f;
    
    [Header("Wave Effect")]
    public PendulumDamageWave damageWavePrefab;   // Particle system GameObject (child or prefab)
    [SerializeField] private float DamageAmount;

    [Space]
    public Pendulum_Wave wavePrefab;   // Particle system GameObject (child or prefab)
    public Transform waveSpawnPoint;
    public float CooldownIncrease;
    public int waveProjectileCount = 5;
    public float CooldownDebuffDuration = 10f;

    private int currentProjecetileCount = 0;
    private Transform playerTarget;
    private float attackTimer;
    private bool isDead = false;
            
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stoppingDistance;
        }
        if (StatManager == null)
        {
            Debug.LogError("StatManager is missing");
        }
        else
        {
            StatManager.OnStatChanged += () =>
            {
                UpdateStats();
            };
        }
        DamageAmount = StatManager.GetStat(EStatType.Damage).currentValue;

        
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackTimer = attackCooldown;

    }

    void UpdateStats()
    {
        DamageAmount = StatManager.GetStat(EStatType.Damage).currentValue;
    }

    private void Update()
    {
        if (isDead || playerTarget == null || agent == null)
            return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            SpawnWave();
            attackTimer = attackCooldown;
        }
    }

    private void SpawnWave()
    {
        currentProjecetileCount++;
        if (currentProjecetileCount % waveProjectileCount == 0)
        {
            SpawnCooldownWave();
        }
        else
        {
            SpawnDamageWave();
        }
    }
    void SpawnDamageWave()
    {
        Vector2 dir = (playerTarget.position - transform.position).normalized;
        PendulumDamageWave wave = Instantiate(damageWavePrefab, waveSpawnPoint.position, Quaternion.identity);
        wave.Init(DamageAmount, dir);
    }
    void SpawnCooldownWave()
    {
        Vector2 dir = (playerTarget.position - transform.position).normalized;
        Pendulum_Wave wave = Instantiate(wavePrefab, waveSpawnPoint.position, Quaternion.identity);
        wave.Init(CooldownIncrease, dir, CooldownDebuffDuration);
    }
}
