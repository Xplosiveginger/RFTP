using UnityEngine;

public class LithiumIonReworked : WeaponBase
{
    [Header("Projectile")]
    public GameObject projectilePrefab;

    private float timeToFire;
    private int firedProjectileCount;


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        // WeaponBase.Start() initializes the local effective
        // weapon stats from this weapon's StatManager and
        // the player's global multipliers.
        base.Start();

        firedProjectileCount = 0;
        timeToFire = Time.time;
    }


    // =========================================================
    // WEAPON UPDATE
    // =========================================================

    public override void UpdateWeapon()
    {
        // -----------------------------------------------------
        // COOLDOWN
        // -----------------------------------------------------

        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;

            firedProjectileCount = 0;

            inCooldown = true;
            isActive = false;

            return;
        }


        // -----------------------------------------------------
        // ACTIVE
        // -----------------------------------------------------

        isActive = true;
        inCooldown = false;


        // Projectile count is always treated as a whole number.
        int targetProjectileCount =
            Mathf.Max(
                0,
                Mathf.RoundToInt(projectileCount)
            );


        // -----------------------------------------------------
        // FIRE PROJECTILES
        // -----------------------------------------------------

        if (firedProjectileCount < targetProjectileCount)
        {
            if (fireRate > 0f &&
                Time.time >= timeToFire)
            {
                timeToFire =
                    Time.time +
                    (1f / fireRate);

                ShootProjectile();
            }
        }
        else if (firedProjectileCount >= targetProjectileCount)
        {
            coolDownTimer = cooldown;

            firedProjectileCount = 0;

            isActive = false;
            inCooldown = true;
        }
    }


    // =========================================================
    // SHOOT
    // =========================================================

    private void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                $"LithiumIon '{name}' has no projectilePrefab."
            );

            return;
        }

        if (enemyDetector == null)
        {
            Debug.LogWarning(
                $"LithiumIon '{name}' has no EnemyDetection."
            );

            return;
        }


        GameObject projectile =
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );


        Projectile projectileComponent =
            projectile.GetComponent<Projectile>();


        if (projectileComponent != null)
        {
            // This is the FINAL effective weapon damage.
            //
            // Example:
            // Lithium base damage = 10
            // Weapon level bonuses = +4
            // Player damage multiplier = 1.2
            //
            // damage = 16.8
            //
            // We never replace Lithium's own stat with
            // the player's stat value.
            projectileComponent.damage = damage;
        }
        else
        {
            Debug.LogWarning(
                $"LithiumIon projectile '{projectile.name}' " +
                $"does not have a Projectile component."
            );
        }


        firedProjectileCount++;


        Vector3 shootAt =
            enemyDetector.GetPositionOfNearestEnemy();


        Vector3 direction =
            (shootAt - transform.position)
            .normalized;


        Rigidbody2D rb =
            projectile.GetComponent<Rigidbody2D>();


        if (rb != null)
        {
            rb.linearVelocity =
                direction *
                projectileSpeed;
        }
        else
        {
            Debug.LogWarning(
                $"LithiumIon projectile '{projectile.name}' " +
                $"does not have a Rigidbody2D."
            );
        }
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        LevelUpLiIon();
    }


    private void LevelUpLiIon()
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
                    2f
                );

                break;


            // -------------------------------------------------
            // LEVEL 3
            // -------------------------------------------------

            case 3:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    2f
                );

                break;


            // -------------------------------------------------
            // LEVEL 4
            // -------------------------------------------------

            case 4:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    2f
                );

                statManager.ModifyStatValue(
                    EStatType.ProjectileCount,
                    1f
                );

                break;


            // -------------------------------------------------
            // LEVEL 5
            // -------------------------------------------------

            case 5:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    2f
                );

                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
                );

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    2f
                );

                // +1 projectile is a flat weapon bonus.
                statManager.ModifyStatValue(
                    EStatType.ProjectileCount,
                    1f
                );

                break;


            // -------------------------------------------------
            // LEVEL 7
            // -------------------------------------------------

            case 7:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    2f
                );

                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
                );

                break;


            // -------------------------------------------------
            // LEVEL 8
            // -------------------------------------------------

            case 8:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    4f
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
}