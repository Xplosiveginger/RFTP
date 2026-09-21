using UnityEngine;

public class MagnetRefactored : WeaponBase
{
    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    public float rotationSpeed;
    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        gfx.transform.localEulerAngles += new Vector3(0, 0, 1 * rotationSpeed);
    }

    // =========================================================
    // STAT ACCESS
    // =========================================================

    /// <summary>
    /// Returns the FINAL effective value of a Magnet stat.
    ///
    /// These values are maintained by WeaponBase.
    ///
    /// Example:
    ///
    /// Magnet own damage = 12
    /// Player damage multiplier = 1.20
    ///
    /// damage = 14.4
    ///
    /// MagnetDamageObject should read this value directly.
    /// It should NOT apply the player multiplier again.
    /// </summary>
    public float GetStatValue(EStatType type)
    {
        switch (type)
        {
            case EStatType.Damage:
                return damage;

            case EStatType.AOESize:
                return AOESize;

            default:

                Debug.LogWarning(
                    $"Magnet '{name}' is trying to access " +
                    $"an unsupported stat: {type}"
                );

                return -1f;
        }
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        LevelUpMagnet();
    }


    private void LevelUpMagnet()
    {
        switch (level)
        {
            // -------------------------------------------------
            // LEVEL 1
            // -------------------------------------------------

            case 1:
                break;


            // -------------------------------------------------
            // LEVEL 2
            // -------------------------------------------------

            case 2:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
                );

                break;


            // -------------------------------------------------
            // LEVEL 3
            // -------------------------------------------------

            case 3:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
                );

                // +15% AOE
                statManager.ModifyStat(
                    EStatType.AOESize,
                    15f
                );

                break;


            // -------------------------------------------------
            // LEVEL 4
            // -------------------------------------------------

            case 4:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
                );

                // -0.5 cooldown
                statManager.ModifyStatValue(
                    EStatType.AttackCooldown,
                    -0.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 5
            // -------------------------------------------------

            case 5:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
                );

                // +15% AOE
                statManager.ModifyStat(
                    EStatType.AOESize,
                    15f
                );

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
                );

                // -0.5 cooldown
                statManager.ModifyStatValue(
                    EStatType.AttackCooldown,
                    -0.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 7
            // -------------------------------------------------

            case 7:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 8
            // -------------------------------------------------

            case 8:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    6f
                );

                break;


            // -------------------------------------------------
            // MAX LEVEL
            // -------------------------------------------------

            default:

                Debug.Log(
                    $"Max Level Reached for " +
                    $"{weaponData.weaponName}"
                );

                break;
        }
    }


    // =========================================================
    // STAT UPDATE
    // =========================================================

    protected override void UpdateStatsHandled()
    {
        /*
         * WeaponBase is responsible for calculating the final
         * effective weapon values.
         *
         * This includes:
         *
         *   Weapon's own stat
         *          ×
         *   Player global multiplier
         *          +
         *   Player flat modifier
         *
         * The resulting values are stored in:
         *
         * damage
         * AOESize
         * cooldown
         * duration
         * etc.
         */
        base.UpdateStatsHandled();
    }
}