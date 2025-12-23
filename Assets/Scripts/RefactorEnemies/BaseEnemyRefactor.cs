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
    [SerializeField] protected StatManager statManager;

    public StatManager StatManager => statManager;

    protected virtual void OnEnable()
    {
        statManager.OnStatChanged += UpdateStatsHandled;
    }

    protected virtual void OnDisable()
    {
        statManager.OnStatChanged -= UpdateStatsHandled;
    }

    protected virtual void Awake()
    {
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

        EnemyManager.Instance.DespawnEnemy(this);
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
