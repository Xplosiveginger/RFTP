using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager statManager;  // NEW: StatManager reference

    [Header("Movement Settings")]
    [SerializeField] private float defaultMoveSpeed = 5f;  // Fallback speed

    [Header("Player State")]
    [SerializeField] public bool inflicted = false;  // Fixed DisplayOnly

    private Rigidbody2D rb;
    private Animator animator;
    public HealthSystem health { get; private set; }

    private Vector2 moveInput;
    private float lastHorizontalDir = 1f; // 1 = right, -1 = left

    // PRESERVED: Your exact speed modifier logic
    public float speedModifier;
    private Coroutine speedCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();

        // Fallback to singleton if not assigned
        if (statManager == null)
            statManager = StatManager.Instance;
    }

    private void Update()
    {
        // Read movement input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // Track last horizontal direction for animation facing
        if (moveInput.x > 0.1f)
            lastHorizontalDir = 1f;
        else if (moveInput.x < -0.1f)
            lastHorizontalDir = -1f;

        // Update animator
        animator.SetInteger("move", isMoving ? 1 : 0);
        animator.SetFloat("facing", lastHorizontalDir);

        inflicted = health.takingDOT;
    }

    private void FixedUpdate()
    {
        // Get move speed from StatManager instead of hardcoded value
        float currentMoveSpeed = GetMoveSpeedFromStatManager();

        // Move the player (your exact logic preserved)
        rb.MovePosition(rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// NEW: Get move speed from StatManager.MoveSpeed stat
    /// Falls back to defaultMoveSpeed if stat not found
    /// </summary>
    private float GetMoveSpeedFromStatManager()
    {
        if (statManager == null) return defaultMoveSpeed;

        var moveSpeedStat = statManager.GetStat(EStatType.MoveSpeed);
        if (moveSpeedStat != null)
        {
            return moveSpeedStat.currentValue * moveSpeedStat.currentMultiplier;
        }

        return defaultMoveSpeed;
    }

    // PRESERVED: Your exact temporary speed modifier methods
    public void ApplyTemporarySpeedModifier(float modifier, float duration)
    {
        if (speedCoroutine != null)
            StopCoroutine(speedCoroutine);

        speedCoroutine = StartCoroutine(TemporarySpeedModifierRoutine(modifier, duration));
    }

    private IEnumerator TemporarySpeedModifierRoutine(float modifier, float duration)
    {
        speedModifier = modifier;
        yield return new WaitForSeconds(duration);
        speedModifier = 1f;
        speedCoroutine = null;
    }
}
