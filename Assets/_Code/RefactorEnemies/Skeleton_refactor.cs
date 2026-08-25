using System;
using UnityEngine;

public class Skeleton_refactor : BaseEnemyRefactor
{
    private Vector3 visualStartScale;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float attackAnimationLength = 0.8f;

    [Header("Attack Settings")]
    public GameObject boneProjectilePrefab;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public Transform throwPoint;

    private float attackTimer = 0f;
    private float attackAnimationTimer = 0f;

    private bool facingLeft;
    private bool isAttacking;

    // Used to detect movement
    private Vector3 lastPosition;

    public Action<Vector3> OnAttack;

    protected override void Awake()
    {
        base.Awake();

        EnemyManager.Instance.RegisterEnemy(this);

        visualStartScale = transform.localScale;

        // Keep the initial state consistent with BaseEnemyRefactor's
        // default-facing convention.
        facingLeft = defaultFacingLeft
            ? visualStartScale.x >= 0f
            : visualStartScale.x < 0f;

        lastPosition = transform.position;

        OnAttack += ThrowBone;
    }

    // Called from EnemyManager each frame
    public void CheckAttack(Vector3 playerPos, float deltaTime)
    {
        if (!gameObject.activeInHierarchy || throwPoint == null || boneProjectilePrefab == null)
            return;

        // Face the player
        FlipVisual(playerPos);

        attackTimer -= deltaTime;

        // Handle attack animation timer
        if (isAttacking)
        {
            attackAnimationTimer -= deltaTime;

            if (attackAnimationTimer <= 0f)
            {
                EndAttack();
            }
        }

        // Only update walking animation when not attacking
        if (!isAttacking)
        {
            UpdateWalkAnimation();
        }

        float distance = Vector3.Distance(transform.position, playerPos);

        if (distance <= attackRange &&
            attackTimer <= 0f &&
            !isAttacking)
        {
            OnAttack?.Invoke(playerPos);
            attackTimer = attackCooldown;
        }
    }

    private void UpdateWalkAnimation()
    {
        if (animator == null)
            return;

        bool isWalking = Vector3.Distance(transform.position, lastPosition) > 0.001f;

        animator.SetBool("Walk", isWalking);

        lastPosition = transform.position;
    }

    private void FlipVisual(Vector3 playerPos)
    {
        bool shouldFaceLeft = playerPos.x < transform.position.x;

        if (shouldFaceLeft == facingLeft)
            return;

        facingLeft = shouldFaceLeft;

        Vector3 scale = visualStartScale;
        float xScale = Mathf.Abs(visualStartScale.x);

        // A positive X scale faces the prefab's default direction.
        // Respect defaultFacingLeft so this matches BaseEnemyRefactor.FaceTarget.
        bool usePositiveXScale = facingLeft == defaultFacingLeft;

        scale.x = usePositiveXScale ? xScale : -xScale;

        transform.localScale = scale;
    }

    private void ThrowBone(Vector3 targetPos)
    {
        if (animator != null)
        {
            isAttacking = true;

            // Stop walk animation
            animator.SetBool("Walk", false);

            // Start attack animation
            animator.SetBool("Attack", true);

            // Use the length provided in the Inspector
            attackAnimationTimer = attackAnimationLength;
        }

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

        if (dir.x < 0)
            vx = -vx;

        GameObject bone = GameObject.Instantiate(
            boneProjectilePrefab,
            start,
            Quaternion.identity
        );

        Rigidbody2D rb = bone.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(vx, vy);
            rb.angularVelocity = UnityEngine.Random.Range(-500f, 500f);
        }
    }

    private void ThrowStraightBone(Vector3 targetPos)
    {
        Vector2 dir = (targetPos - throwPoint.position).normalized;

        GameObject bone = GameObject.Instantiate(
            boneProjectilePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        Rigidbody2D rb = bone.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float speed = 10f;

            rb.linearVelocity = dir * speed;
            rb.angularVelocity = UnityEngine.Random.Range(-500f, 500f);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;

        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        lastPosition = transform.position;
    }
}