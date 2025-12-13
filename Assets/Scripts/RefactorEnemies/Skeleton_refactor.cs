using System;
using UnityEngine;

public class Skeleton_refactor : BaseEnemyRefactor
{
    [Header("Attack Settings")]
    public GameObject boneProjectilePrefab;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public Transform throwPoint;

    private float attackTimer = 0f;

    // Event called when Skeleton wants to attack
    public Action<Vector3> OnAttack;

    protected override void Awake()
    {
        base.Awake();
        EnemyManager.Instance.RegisterEnemy(this);

        // Hook default attack function
        OnAttack += ThrowBone;
    }

    // Called from EnemyManager each frame
    public void CheckAttack(Vector3 playerPos, float deltaTime)
    {
        if (!gameObject.activeInHierarchy || throwPoint == null || boneProjectilePrefab == null)
            return;

        attackTimer -= deltaTime;
        float distance = Vector3.Distance(transform.position, playerPos);

        if (distance <= attackRange && attackTimer <= 0f)
        {
            OnAttack?.Invoke(playerPos);
            attackTimer = attackCooldown;
        }
    }

    private void ThrowBone(Vector3 targetPos)
    {
        Vector2 start = throwPoint.position;
        Vector2 target = targetPos;

        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float launchAngleDegrees = 45f;
        float launchAngleRadians = launchAngleDegrees * Mathf.Deg2Rad;

        float distance = Vector2.Distance(start, target);
        float heightDifference = target.y - start.y;

        float initialVelocitySq = (gravity * distance * distance) /
                                  (2 * (heightDifference - Mathf.Tan(launchAngleRadians) * distance) *
                                  Mathf.Pow(Mathf.Cos(launchAngleRadians), 2));

        if (initialVelocitySq <= 0f)
        {
            ThrowStraightBone(target);
            return;
        }

        float initialVelocity = Mathf.Sqrt(initialVelocitySq);
        float vx = initialVelocity * Mathf.Cos(launchAngleRadians);
        float vy = initialVelocity * Mathf.Sin(launchAngleRadians);

        Vector2 dir = (target - start).normalized;
        if (dir.x < 0) vx = -vx;

        GameObject bone = GameObject.Instantiate(boneProjectilePrefab, start, Quaternion.identity);
        Rigidbody2D rb = bone.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(vx, vy);
            rb.angularVelocity = UnityEngine.Random.Range(-500f, 500f);
        }
    }

    private void ThrowStraightBone(Vector3 targetPos)
    {
        Vector2 dir = (targetPos - throwPoint.position).normalized;

        GameObject bone = GameObject.Instantiate(boneProjectilePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = bone.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float speed = 10f;
            rb.velocity = dir * speed;
            rb.angularVelocity = UnityEngine.Random.Range(-500f, 500f);
        }
    }
}
