using System;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyRefactor : MonoBehaviour
{
    protected NavMeshAgent agent;

    [Header("Facing Settings")]
    [Tooltip("True if the default sprite (scale.x positive) faces left, false if it faces right")]
    public bool defaultFacingLeft = true;

    [SerializeField] protected float moveSpeed = 3.5f;
    public Action moveSpeedincrease;

    private void OnEnable()
    {
        moveSpeedincrease += MoveSpeedApplier;
    }

    
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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

    private void OnDisable()
    {
        moveSpeedincrease -= MoveSpeedApplier;
    }
}
