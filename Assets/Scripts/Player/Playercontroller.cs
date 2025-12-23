using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [DisplayOnly] public bool inflicted = false;

    private Rigidbody2D rb;
    private Animator animator;
    public HealthSystem health {get; private set;}

    private Vector2 moveInput;
    private float lastHorizontalDir = 1f; // 1 = right, -1 = left

    public float speedModifier;
    private Coroutine speedCoroutine;

    public StatManager statManager;
    public ReworkedWeaponManager weaponManager;

    //test
    public WeaponDataSO weapon;
    public EWeaponName weaponToLevelUp;

    private void OnEnable()
    {
        statManager.OnMoveSpeedChanged += GetModifiedSpeed;
        statManager.OnHealthChanged += GetModifiedHealth;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();

        GetModifiedSpeed();
        GetModifiedHealth();
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            AddNewWeapon();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            weaponManager.GetWeapon(weaponToLevelUp).LevelUpWeapon();
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
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

    // Testing *****************************
    private void AddNewWeapon()
    {
        GetComponent<ReworkedWeaponManager>().AddNewWeapon(weapon);
    }

    private void ApplySpeedModif()
    {
        statManager.ModifyStat(EStatType.MoveSpeed, 10);
    }
    //**********************************

    private void GetModifiedSpeed()
    {
        Stat stat = statManager.GetStat(EStatType.MoveSpeed);
        moveSpeed = stat.currentValue;
    }

    private void GetModifiedHealth()
    {
        Stat stat = statManager.GetStat(EStatType.Health);
        health.currentHealth = (int)stat.currentValue;
        health.maxHealth = (int)stat.maxValue;
    }
}