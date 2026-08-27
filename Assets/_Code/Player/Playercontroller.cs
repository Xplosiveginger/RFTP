using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    public bool inflicted = false;

    private Rigidbody2D rb;
    public Animator animator;
    public HealthSystem health {get; private set;}

    private Vector2 moveInput;
    private float lastHorizontalDir = 1f; // 1 = right, -1 = left

    public float speedModifier;
    private Coroutine speedCoroutine;

    public StatManager statManager;
    public ReworkedWeaponManager weaponManager;
    public ItemManager itemManager;

    //test
    public WeaponDataSO weaponToAdd;

    private void OnEnable()
    {
        statManager.OnMoveSpeedChanged += GetModifiedSpeed;
        statManager.OnHealthChanged += GetModifiedHealth;
        CardManager.CardSelected += OnCardSelectedHandled;
    }

    private void OnDisable()
    {
        statManager.OnMoveSpeedChanged -= GetModifiedSpeed;
        statManager.OnHealthChanged -= GetModifiedHealth;
        CardManager.CardSelected -= OnCardSelectedHandled;

    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthSystem>();
        statManager.InitializeStats();
    }

    private void Start()
    {
        

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

    private void GetModifiedSpeed()
    {
        Stat stat = statManager.GetStat(EStatType.MoveSpeed);
        moveSpeed = stat.currentValue;
    }

    private void GetModifiedHealth()
    {
        Stat stat = statManager.GetStat(EStatType.Health);
        health.maxHealth= (int)stat.maxValue;
        health.currentHealth= (int)stat.currentValue;

    }

    private void AddWeapon(WeaponDataSO weapon)
    {
        weaponManager.AddNewWeapon(weapon);
    }

    private void OnCardSelectedHandled(CardDataSO card)
    {
        if (card == null)
            return;

        switch (card.cardType)
        {
            case ECardType.AddsWeapon:
                AddWeapon(card.weaponToAdd);
                break;

            case ECardType.AffectsPlayer:
                // ItemManager handles:
                // 1. Adding/upgrading the item
                // 2. Applying the item's stat effect
                // 3. Updating GameStat_SO
                itemManager.AddCurrentItems(card);
                break;

            case ECardType.AffectsWeaponLevel:
                weaponManager.LevelUpWeapon(card.weaponName);
                break;

            case ECardType.AffectsSpecificWeaponStat:
                weaponManager.UpdateWeaponStat(
                    card.weaponName,
                    card.affectedWeaponStat,
                    card.weaponStatModifier
                );
                break;

            case ECardType.ExtraCard_Health:
                //AddsHealth();
                health.Heal((int)card.healthToAdd);
                Debug.Log("Health added");
                break;
            case ECardType.ExtraCard_Money:
                //Adds money 
                EconomyManager.Instance.AddMoney(card.moneyToAdd);
                Debug.Log("Moeny added");

                break;


            default:
                break;
        }
    }
}