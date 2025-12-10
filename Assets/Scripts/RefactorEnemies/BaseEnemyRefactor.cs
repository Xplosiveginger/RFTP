using UnityEngine;
using UnityEngine.AI;

public class BaseEnemyRefactor : MonoBehaviour
{
    protected NavMeshAgent agent;

    [Header("Facing Settings")]
    [Tooltip("True if the default sprite (scale.x positive) faces left, false if it faces right")]
    public bool defaultFacingLeft = true;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void UpdateMovement(Vector3 targetPos)
    {
        if (agent != null)
        {
            transform.rotation = Quaternion.identity;
            agent.SetDestination(targetPos);

            // Face the player
            FaceTarget(targetPos);
        }
    }

    public virtual void Die()
    {
        if (agent != null)
            agent.ResetPath();

        EnemyManager.Instance.DespawnEnemy(this);
    }

    // --------------------------------------------------
    // FACE THE PLAYER (works for any default facing)
    // --------------------------------------------------
    protected void FaceTarget(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;

        float xScale = Mathf.Abs(transform.localScale.x);

        if ((direction.x > 0f && defaultFacingLeft) || (direction.x < 0f && !defaultFacingLeft))
            xScale = -xScale;

        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }
}
