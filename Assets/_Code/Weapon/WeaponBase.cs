using Sirenix.OdinInspector;
using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    public StatManager statManager;

    // Player StatManager.
    // Player stats are GLOBAL modifiers only.
    protected StatManager playerStatManager;

    public GameObject gfx;

    public WeaponDataSO weaponData;
    public EnemyDetection enemyDetector;


    // =========================================================
    // WEAPON DEBUFF
    // =========================================================

    [Header("Weapon Debuff")]
    public float debuffCooldownAdditionDuration = 0f;


    // =========================================================
    // WEAPON LEVEL
    // =========================================================

    [ReadOnly]
    public int level = 1;

    public int GetLevel
    {
        get { return level; }
    }


    // =========================================================
    // EFFECTIVE RUNTIME VALUES
    // =========================================================

    // These are the FINAL values actually used by the weapon.
    //
    // Example:
    //
    // Weapon damage = 12
    // Player damage multiplier = 1.1
    //
    // damage = 12 * 1.1 = 13.2

    protected float damage;
    protected float projectileSpeed;
    protected float projectileCount;
    protected float AOESize;
    protected float cooldown;
    protected float duration;
    protected float fireRate;


    // =========================================================
    // TIMERS
    // =========================================================

    protected float activeTimer;
    protected float coolDownTimer;


    [SerializeField]
    protected bool isActive;

    [SerializeField]
    protected bool inCooldown;


    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<WeaponBase> OnWeaponCreated;


    // =========================================================
    // SPAWN
    // =========================================================

    public virtual void SpawnWeapon(Transform parent)
    {
        if (weaponData == null)
        {
            Debug.LogError(
                $"Weapon '{name}' has no WeaponDataSO."
            );

            return;
        }

        if (weaponData.weaponPrefab != null)
        {
            WeaponBase weapon =
                Instantiate(
                    weaponData.weaponPrefab,
                    parent.position,
                    Quaternion.identity,
                    parent
                ).GetComponent<WeaponBase>();

            OnWeaponCreated?.Invoke(weapon);
        }
    }


    // =========================================================
    // ENABLE / DISABLE
    // =========================================================

    protected virtual void OnEnable()
    {
        if (statManager != null)
        {
            statManager.OnStatChanged +=
                UpdateStatsHandled;
        }

        SubscribeToPlayerStats();
    }


    protected virtual void OnDisable()
    {
        if (statManager != null)
        {
            statManager.OnStatChanged -=
                UpdateStatsHandled;
        }

        UnsubscribeFromPlayerStats();
    }


    // =========================================================
    // AWAKE
    // =========================================================

    protected virtual void Awake()
    {
        statManager =
            GetComponent<StatManager>();

        if (statManager == null)
        {
            Debug.LogError(
                $"Weapon '{name}' has no StatManager."
            );

            return;
        }

        if (weaponData == null)
        {
            Debug.LogError(
                $"Weapon '{name}' has no WeaponDataSO."
            );

            return;
        }

        // The weapon owns its own StatManager data.
        statManager.statDataList =
            weaponData.GetAllWeaponStatDatas();

        statManager.InitializeStats();
    }


    // =========================================================
    // START
    // =========================================================

    protected virtual void Start()
    {
        /*
         * Find the player StatManager.
         *
         * The weapon keeps its OWN stats.
         * Player stats are only global modifiers.
         */

        if (playerStatManager == null)
        {
            FindPlayerStatManager();
        }

        UpdateStatsHandled();
    }


    // =========================================================
    // FIND PLAYER STAT MANAGER
    // =========================================================

    private void FindPlayerStatManager()
    {
        if (transform.parent == null)
            return;

        playerStatManager =
            transform.parent.GetComponentInParent<StatManager>();
    }


    // =========================================================
    // PLAYER STAT EVENTS
    // =========================================================

    private void SubscribeToPlayerStats()
    {
        if (playerStatManager == null)
        {
            FindPlayerStatManager();
        }

        if (playerStatManager != null)
        {
            playerStatManager.OnStatChanged +=
                UpdateStatsHandled;
        }
    }


    private void UnsubscribeFromPlayerStats()
    {
        if (playerStatManager != null)
        {
            playerStatManager.OnStatChanged -=
                UpdateStatsHandled;
        }
    }


    // =========================================================
    // WEAPON UPDATE
    // =========================================================

    public virtual void UpdateWeapon()
    {
        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;

            inCooldown = true;
            isActive = false;

            return;
        }

        if (!isActive)
        {
            activeTimer =
                duration;

            ToggleGFXVisibility(true);

            isActive = true;
            inCooldown = false;

            return;
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
        }
        else
        {
            coolDownTimer =
                cooldown +
                debuffCooldownAdditionDuration;

            ToggleGFXVisibility(false);

            isActive = false;
            inCooldown = true;
        }
    }


    // =========================================================
    // GFX
    // =========================================================

    public virtual void ToggleGFXVisibility(bool b)
    {
        if (gfx == null)
            return;

        gfx.SetActive(b);
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    public virtual void UpdateWeaponDamage()
    {
        UpdateStatsHandled();
    }


    // =========================================================
    // UPDATE ALL EFFECTIVE STATS
    // =========================================================

    protected virtual void UpdateStatsHandled()
    {
        if (statManager == null)
            return;

        UpdateEffectiveWeaponStat(
            EStatType.Damage
        );

        UpdateEffectiveWeaponStat(
            EStatType.ProjectileSpeed
        );

        UpdateEffectiveWeaponStat(
            EStatType.ProjectileCount
        );

        UpdateEffectiveWeaponStat(
            EStatType.AOESize
        );

        UpdateEffectiveWeaponStat(
            EStatType.AttackCooldown
        );

        UpdateEffectiveWeaponStat(
            EStatType.ActiveDuration
        );

        UpdateEffectiveWeaponStat(
            EStatType.FireRate
        );
    }


    // =========================================================
    // EFFECTIVE WEAPON STAT
    // =========================================================

    protected virtual void UpdateEffectiveWeaponStat(
        EStatType statType)
    {
        if (statManager == null)
            return;

        Stat weaponStat =
            statManager.GetStat(statType);

        if (weaponStat == null)
            return;

        // -----------------------------------------------------
        // WEAPON'S OWN VALUE
        // -----------------------------------------------------

        float weaponValue =
            weaponStat.currentValue;


        // -----------------------------------------------------
        // PLAYER GLOBAL MODIFIER
        // -----------------------------------------------------

        float playerMultiplier =
            GetPlayerMultiplier(statType);

        float playerFlatModifier =
            GetPlayerFlatModifier(statType);


        // -----------------------------------------------------
        // FINAL VALUE
        // -----------------------------------------------------

        float finalValue =
            (weaponValue * playerMultiplier)
            + playerFlatModifier;


        // -----------------------------------------------------
        // STORE FINAL VALUE
        // -----------------------------------------------------

        switch (statType)
        {
            case EStatType.Damage:

                damage =
                    finalValue;

                break;


            case EStatType.ProjectileSpeed:

                projectileSpeed =
                    finalValue;

                break;


            case EStatType.ProjectileCount:

                // Projectile count must always be whole.
                projectileCount =
                    Mathf.RoundToInt(
                        finalValue
                    );

                // Prevent invalid negative projectile counts.
                projectileCount =
                    Mathf.Max(
                        0,
                        projectileCount
                    );

                break;


            case EStatType.AOESize:

                AOESize =
                    finalValue;

                break;


            case EStatType.AttackCooldown:

                cooldown =
                    finalValue;

                // Never allow a negative cooldown.
                cooldown =
                    Mathf.Max(
                        0f,
                        cooldown
                    );

                break;


            case EStatType.ActiveDuration:

                duration =
                    finalValue;

                break;


            case EStatType.FireRate:

                fireRate =
                    finalValue;

                break;
        }
    }


    // =========================================================
    // PLAYER MULTIPLIER
    // =========================================================

    private float GetPlayerMultiplier(
        EStatType statType)
    {
        if (playerStatManager == null)
            return 1f;

        Stat playerStat =
            playerStatManager.GetStat(statType);

        if (playerStat == null)
            return 1f;


        /*
         * Player stats do NOT provide the weapon's base value.
         *
         * They only provide the multiplier.
         *
         * Example:
         *
         * Player startMultiplier = 1.0
         * Player currentMultiplier = 1.1
         *
         * multiplier = 1.1 / 1.0
         *            = 1.1
         *
         * Weapon damage = 12
         *
         * Final damage = 12 * 1.1
         *              = 13.2
         */

        return
            playerStat.currentMultiplier /
            Mathf.Max(
                playerStat.startMultiplier,
                0.0001f
            );
    }


    // =========================================================
    // PLAYER FLAT MODIFIER
    // =========================================================

    private float GetPlayerFlatModifier(
        EStatType statType)
    {
        if (playerStatManager == null)
            return 0f;

        Stat playerStat =
            playerStatManager.GetStat(statType);

        if (playerStat == null)
            return 0f;

        return playerStat.flatModifier;
    }


    // =========================================================
    // MANUAL PLAYER STAT UPDATE
    // =========================================================

    public virtual void UpdateStatsFromPlayer(
        StatManager playerStats)
    {
        if (playerStats == null)
            return;

        /*
         * Store the player's StatManager.
         *
         * IMPORTANT:
         *
         * We DO NOT copy:
         *
         * playerStat.currentValue
         *
         * into the weapon.
         *
         * The weapon keeps its own base/current value.
         *
         * Player stats are only used as:
         *
         *     multiplier
         *     +
         *     flat modifier
         */

        if (playerStatManager != null)
        {
            playerStatManager.OnStatChanged -=
                UpdateStatsHandled;
        }

        playerStatManager =
            playerStats;

        playerStatManager.OnStatChanged +=
            UpdateStatsHandled;

        UpdateStatsHandled();
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public virtual void LevelUpWeapon()
    {
        level++;
    }


    // =========================================================
    // COOLDOWN DEBUFF
    // =========================================================

    public virtual void AddCooldownDebuff(
        float debuffDuration)
    {
        coolDownTimer +=
            debuffDuration;

        debuffCooldownAdditionDuration +=
            debuffDuration;
    }


    public virtual void RemoveCooldownDebuff(
        float removalDuration)
    {
        debuffCooldownAdditionDuration -=
            removalDuration;

        debuffCooldownAdditionDuration =
            Mathf.Max(
                0f,
                debuffCooldownAdditionDuration
            );
    }
}