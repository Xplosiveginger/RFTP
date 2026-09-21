using UnityEngine;

public class BacteriaReworked : WeaponBase
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


        // projectileCount is already converted to a whole
        // number by WeaponBase.
        int targetProjectileCount =
            Mathf.Max(
                0,
                Mathf.RoundToInt(projectileCount)
            );


        // -----------------------------------------------------
        // FIRE
        // -----------------------------------------------------

        if (firedProjectileCount < targetProjectileCount)
        {
            if (fireRate > 0f &&
                Time.time >= timeToFire)
            {
                timeToFire =
                    Time.time + (1f / fireRate);

                ShootProjectile();
            }
        }
        else
        {
            // Finished this firing cycle.
            coolDownTimer = cooldown;

            firedProjectileCount = 0;

            isActive = false;
            inCooldown = true;
        }
    }


    // =========================================================
    // SHOOT PROJECTILE
    // =========================================================

    private void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                $"Bacteria '{name}' has no projectilePrefab."
            );

            return;
        }

        if (enemyDetector == null)
        {
            Debug.LogWarning(
                $"Bacteria '{name}' has no EnemyDetection."
            );

            return;
        }


        GameObject projectile =
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );


        // -----------------------------------------------------
        // DAMAGE
        // -----------------------------------------------------

        Projectile projectileComponent =
            projectile.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            // This is the FINAL effective weapon damage.
            //
            // Example:
            // Weapon damage = 20
            // Player global damage multiplier = 1.10
            //
            // damage = 22
            //
            projectileComponent.damage = damage;
        }
        else
        {
            Debug.LogWarning(
                $"Bacteria projectile '{projectile.name}' " +
                "does not have a Projectile component."
            );
        }


        // -----------------------------------------------------
        // TARGET
        // -----------------------------------------------------

        Vector3 shootAt =
            enemyDetector.GetPositionOfNearestEnemy();

        Vector3 direction =
            (shootAt - transform.position).normalized;


        // -----------------------------------------------------
        // SPEED
        // -----------------------------------------------------

        Rigidbody2D rb =
            projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * projectileSpeed;
        }
        else
        {
            Debug.LogWarning(
                $"Bacteria projectile '{projectile.name}' " +
                "does not have a Rigidbody2D."
            );
        }


        firedProjectileCount++;
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeBacteria();
    }


    private void UpgradeBacteria()
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
                    EStatType.AttackCooldown,
                    -0.3f
                );

                break;


            // -------------------------------------------------
            // LEVEL 3
            // -------------------------------------------------

            case 3:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 4
            // -------------------------------------------------

            case 4:

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
                    5f
                );

                statManager.ModifyStatValue(
                    EStatType.AttackCooldown,
                    -0.3f
                );

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

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
                    5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 8
            // -------------------------------------------------

            case 8:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    5f
                );

                statManager.ModifyStatValue(
                    EStatType.AttackCooldown,
                    -0.3f
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
         * WeaponBase calculates the effective values.
         *
         * Example:
         *
         * Bacteria own damage:
         *     20
         *
         * Player damage multiplier:
         *     1.10
         *
         * Final:
         *     20 × 1.10 = 22
         *
         * The player does NOT replace the weapon's damage.
         */

        base.UpdateStatsHandled();
    }
}