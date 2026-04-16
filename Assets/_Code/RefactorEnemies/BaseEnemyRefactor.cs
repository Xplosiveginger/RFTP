using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyRefactor : MonoBehaviour
{
    protected NavMeshAgent agent;

    [Header("Facing Settings")]
    [Tooltip("True if the default sprite (scale.x positive) faces left, false if it faces right")]
    public bool defaultFacingLeft = true;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float moveSpeedMultiplier;
    protected float health;
    protected float maxHealth;
    HealthSystem healthSystem;
    [SerializeField] protected StatManager statManager;
    [SerializeField] protected bool DiesOnContactWithPlayer;
    [SerializeField] protected bool DoesDamageOnContact;

    // to be removed later
    [SerializeField] GameObject XpOrbPrefab;
    [SerializeField] int expDrop;
    // ********************

    public StatManager StatManager => statManager;

    protected virtual void OnEnable()
    {
        healthSystem.OnDeath += Die;
        statManager.OnStatChanged += UpdateStatsHandled;
    }

    protected virtual void OnDisable()
    {
        healthSystem.OnDeath -= Die;
        statManager.OnStatChanged -= UpdateStatsHandled;
    }

    protected virtual void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        agent = GetComponent<NavMeshAgent>();
        statManager = GetComponent<StatManager>();
        statManager.InitializeStats();
        moveSpeed = statManager.GetStat(EStatType.MoveSpeed).currentValue;
        health = statManager.GetStat(EStatType.Health).currentValue;
        maxHealth = statManager.GetStat(EStatType.Health).maxValue;
        agent.updateUpAxis = false;
        MoveSpeedApplier();
    }

    public void MoveSpeedApplier()
    {
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    public virtual void UpdateMovement(Vector3 targetPos)
    {
        if (agent != null)
        {
            transform.rotation = Quaternion.identity;
            agent.SetDestination(targetPos);
            FaceTarget(targetPos);
        }

    }

    public virtual void Die()
    {
        if (agent != null)
            agent.ResetPath();

        SpawnXp();
        //statManager.ResetHealthStatOnDeath();
        EnemyManager.Instance.DespawnEnemy(this);
    }

    public virtual void ResetOnDeath()
    {
        statManager.ResetHealthStatOnDeath();
        health = statManager.GetStat(EStatType.Health).maxValue;
        healthSystem.ResetHealth();
    }

    protected void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;

        float xScale = Mathf.Abs(transform.localScale.x);

        if ((direction.x > 0f && defaultFacingLeft) || (direction.x < 0f && !defaultFacingLeft))
            xScale = -xScale;

        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void UpdateStatsHandled()
    {
        moveSpeed = statManager.GetStat(EStatType.MoveSpeed).currentValue;
        health = statManager.GetStat(EStatType.Health).currentValue;
        maxHealth = statManager.GetStat(EStatType.Health).maxValue;
        MoveSpeedApplier();
    }
    public virtual void UpdateHealth()
    {
        health = statManager.GetStat(EStatType.Health).currentValue;
    }

    public void SpawnXp()
    {
        if (XpOrbPrefab == null) return;

        GameObject xpOrb = Instantiate(XpOrbPrefab, transform.position, Quaternion.identity);
        XpDrop orb = xpOrb.GetComponent<XpDrop>();
        if (orb != null)
            orb.xpAmount = expDrop;
    }

    public void Freeze(float time)
    {
        StartCoroutine(FreezeMovement(time));
    }

    IEnumerator FreezeMovement(float time)
    {
        float temp = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(time);
        moveSpeed = temp;
    }
}
