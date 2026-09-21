using DG.Tweening;
using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    [Header("Settings")]
    public bool canDestroyOnDeath = true;
    public bool isPlayer = false;

    [Header("Death / Damage Events")]
    public UnityEvent onDeath;
    public UnityEvent onPostDeath;
    public UnityEvent<int> onDamageTaken;

    private StatManager statManager;
    private float healthRegenAccumulator = 0f;

    [Header("Effects")]
    public GameObject deathEffect;

    [Header("Damage Flash")]
    [Tooltip("Material used while the character is taking damage.")]
    public Material flashMaterial;

    [Tooltip("Normal material restored after the damage flash.")]
    public Material normalMaterial;

    [Tooltip("How long the flash material stays active.")]
    public float flashDuration = 0.1f;

    [Header("Player Camera Shake")]
    [Tooltip("Strength of the camera shake when the player takes damage.")]
    public float cameraShakeStrength = 0.15f;

    [Tooltip("Duration of the camera shake.")]
    public float cameraShakeDuration = 0.15f;

    [Tooltip("Vibrato/frequency of the camera shake.")]
    public int cameraShakeVibrato = 10;

    [Tooltip("Randomness of the camera shake.")]
    [Range(0f, 180f)]
    public float cameraShakeRandomness = 90f;

    [Header("Death Animation")]
    public float jumpPower = 0.6f;
    public int jumpNum = 1;
    public float jumpDuration = 0.45f;
    public float scaleDuration = 0.4f;
    public Ease scaleEase = Ease.InCubic;

    [Header("Hurt Settings")]
    public float hurtCooldown = 0.1f;

    [Header("UI")]
    public Slider healthSlider;

    private bool isDead = false;
    private float lastHurtTime = -1f;

    private SpriteRenderer sr;

    // Cached original scale.
    private Vector3 originalScale;
    private Vector3 defaultScale;

    private Sequence hurtSeq;
    private Sequence flashSeq;

    private EnemyAI enemy;

    // Cinemachine Impulse Source.
    private CinemachineImpulseSource impulseSource;

    public bool debug;

    public bool takingDOT { get; private set; }
    private int dotDamage = 0;

    public event Action OnDeath;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    public event Action<float> OnHealthChanged;

    private GameStat_SO GameStat_SO;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        enemy = GetComponent<EnemyAI>();

        sr = GetComponentInChildren<SpriteRenderer>();

        // Cache the original scale once.
        defaultScale = transform.localScale;
        originalScale = defaultScale;

        // Only the player needs a Cinemachine impulse source.
        if (isPlayer)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();

            if (impulseSource == null)
            {
                Debug.LogWarning(
                    $"{name}: Player HealthSystem could not find a CinemachineImpulseSource."
                );
            }
        }

        if (sr != null)
        {
            // Start with the normal material.
            if (normalMaterial != null)
                sr.material = normalMaterial;
        }

        currentHealth = maxHealth;

        UpdateHealthUI();
    }

    private void OnEnable()
    {
        // Kill existing tweens.
        if (DOTween.IsTweening(transform))
            transform.DOKill(true);

        hurtSeq?.Kill();
        flashSeq?.Kill();

        // Always restore the cached scale.
        transform.localScale = originalScale;

        // Restore normal material.
        if (sr != null && normalMaterial != null)
            sr.material = normalMaterial;

        ResetHealth();
    }

    private void Start()
    {
        GameStat_SO = PersistentObject.Instance.GameStat_SO;
        statManager = GetComponent<StatManager>();
    }

    private void Update()
    {
        if (!isPlayer || isDead)
            return;

        if (statManager == null)
            return;

        Stat healthRegenStat =
            statManager.GetStat(EStatType.HealthRegen);

        if (healthRegenStat == null)
            return;

        float regenPerSecond = healthRegenStat.currentValue;

        if (regenPerSecond <= 0f)
            return;

        if (currentHealth >= maxHealth)
            return;

        float healAmount =
            regenPerSecond * Time.deltaTime;

        Heal(healAmount);
    }

    public void Damage(int damageAmount)
    {
        if (isDead || damageAmount <= 0)
            return;

        if (debug)
            Debug.Log(damageAmount);

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthUI();

        if (isPlayer && IDCardManager.instance != null)
            IDCardManager.instance.UpdatePlayerPortrait();

        if (isPlayer)
        {
            GameStat_SO.RegisterDamageTaken(damageAmount);
            Debug.Log("Player damage: " + damageAmount);
        }
        else
        {
            GameStat_SO.RegisterDamageGiven(damageAmount);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayHurtEffect();

            onDamageTaken?.Invoke(damageAmount);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead || healAmount <= 0f)
            return;

        currentHealth =
            Mathf.Min(maxHealth, currentHealth + healAmount);

        UpdateHealthUI();

        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        onDeath?.Invoke();

        if (deathEffect)
            Instantiate(
                deathEffect,
                transform.position,
                Quaternion.identity
            );

        if (!isPlayer)
        {
            GameStat_SO.RegisterEnemyKilled();
        }

        // Kill ongoing tweens.
        if (DOTween.IsTweening(transform))
        {
            transform.DOKill();
            hurtSeq?.Kill();

            // Restore cached scale.
            transform.localScale = originalScale;
        }

        flashSeq?.Kill();

        // Restore normal material.
        if (sr != null && normalMaterial != null)
            sr.material = normalMaterial;

        Sequence deathSeq =
            DOTween.Sequence().SetUpdate(true);

        deathSeq.Join(
            transform.DOJump(
                transform.position + Vector3.up * 0.05f,
                jumpPower,
                jumpNum,
                jumpDuration
            ).SetEase(Ease.OutQuad)
        );

        deathSeq.Join(
            transform.DOScale(
                Vector3.zero,
                scaleDuration
            ).SetEase(scaleEase)
        );

        deathSeq.OnComplete(() =>
        {
            onPostDeath?.Invoke();

            if (isPlayer && canDestroyOnDeath)
                Destroy(gameObject);
            else
                OnDeath?.Invoke();
        });
    }

    private void PlayHurtEffect()
    {
        if (Time.time - lastHurtTime < hurtCooldown)
            return;

        lastHurtTime = Time.time;

        // Kill previous effects.
        hurtSeq?.Kill();
        flashSeq?.Kill();

        // ==========================================
        // DAMAGE MATERIAL FLASH
        // ==========================================

        if (sr != null)
        {
            sr.DOKill();

            // Always start from the normal material.
            if (normalMaterial != null)
                sr.material = normalMaterial;

            if (flashMaterial != null)
            {
                // Switch immediately to flash material.
                sr.material = flashMaterial;

                flashSeq = DOTween.Sequence()
                    .SetUpdate(true)

                    // Keep flash material active.
                    .AppendInterval(flashDuration)

                    // Return to normal material.
                    .AppendCallback(() =>
                    {
                        if (sr != null && normalMaterial != null)
                            sr.material = normalMaterial;
                    });
            }
        }

        // ==========================================
        // HURT SCALE PUNCH
        // ==========================================

        transform.DOKill();

        // IMPORTANT:
        // Restore the original cached scale before every punch.
        // This prevents continuous damage from accumulating scale.
        transform.localScale = originalScale;

        float punchDuration = 0.2f;

        hurtSeq = DOTween.Sequence()
            .SetUpdate(true);

        hurtSeq.Join(
            transform.DOPunchScale(
                new Vector3(0.06f, 0.06f, 0f),
                punchDuration,
                vibrato: 0,
                elasticity: 0f
            )
        );

        // ==========================================
        // PLAYER CAMERA SHAKE ONLY
        // ==========================================

        if (isPlayer && impulseSource != null)
        {
            // Cinemachine handles the actual camera movement.
            //
            // The Impulse Source must be configured on the player.
            impulseSource.GenerateImpulse(
                cameraShakeStrength
            );
        }
    }

    public void ResetHealth()
    {
        // Kill any running tweens.
        if (DOTween.IsTweening(transform))
        {
            transform.DOKill(true);
            hurtSeq?.Kill();
        }

        flashSeq?.Kill();

        isDead = false;
        currentHealth = maxHealth;

        healthRegenAccumulator = 0f;

        // Restore cached original scale.
        transform.localScale = originalScale;

        // Restore normal material.
        if (sr != null && normalMaterial != null)
            sr.material = normalMaterial;

        UpdateHealthUI();
    }

    public void SetMaxHealth(
        float newMaxHealth,
        bool resetCurrentHealth = true
    )
    {
        maxHealth = newMaxHealth;

        if (resetCurrentHealth)
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
        }
        else
        {
            currentHealth =
                Mathf.Min(currentHealth, maxHealth);

            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value =
                (float)currentHealth / maxHealth;
        }
    }

    public void TakeDamageOverTime(
        float duration,
        float interval,
        int damageAmount
    )
    {
        if (isDead || takingDOT)
            return;

        takingDOT = true;
        dotDamage = damageAmount;

        InvokeRepeating(
            nameof(DoDamageTick),
            0f,
            interval
        );

        Invoke(
            nameof(StopDOT),
            duration
        );
    }

    private void DoDamageTick()
    {
        if (isDead)
        {
            CancelInvoke(nameof(DoDamageTick));
            takingDOT = false;
            return;
        }

        Damage(dotDamage);

        Debug.Log($"Taking Damage {dotDamage}");
    }

    private void StopDOT()
    {
        CancelInvoke(nameof(DoDamageTick));

        dotDamage = 0;
        takingDOT = false;
    }
}
