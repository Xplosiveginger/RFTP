using System.Collections.Generic;
using UnityEngine;

public class LaserWeaponRefactored : WeaponBase
{
    [Header("Spawner Settings")]
    [Range(1, 3)]
    public int range = 1; // 1 = Right, 2 = Right + Left, 3 = Right + Left + Up

    public GameObject laserPrefab;


    [Header("Offsets (distance from center)")]
    public float offsetRight = 0.5f;
    public float offsetLeft = 0.5f;
    public float offsetUp = 0.5f;


    [Header("Oscillation Settings")]
    public float moveRange = 0.5f;
    public float moveSpeed = 2f;


    [Header("Runtime")]
    public List<GameObject> activeLasers =
        new List<GameObject>();


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start()
    {
        // Base.Start() updates:
        // damage
        // projectileCount
        // cooldown
        // duration
        // etc.
        base.Start();

        SpawnLasers();

        // Make sure the spawned lasers immediately receive
        // the correct damage.
        UpdateDamageForEachLaser();

        // Make sure projectile visibility matches the
        // current projectile count.
        UpdateLaserVisibility();
    }


    // =========================================================
    // SPAWN WEAPON
    // =========================================================

    public override void SpawnWeapon(
        Transform parent)
    {
        base.SpawnWeapon(parent);
    }


    // =========================================================
    // LASER SPAWNING
    // =========================================================

    private void SpawnLasers()
    {
        ClearLasers();

        if (laserPrefab == null)
        {
            Debug.LogWarning(
                $"LaserWeapon '{name}' has no laserPrefab."
            );

            return;
        }

        Debug.Log(
            $"Spawning lasers for {name}. " +
            $"Projectile Count: {projectileCount}"
        );


        // Always spawn Right.
        CreateLaser(
            Vector2.right,
            0f,
            offsetRight
        );


        // Second laser.
        if (range >= 2)
        {
            CreateLaser(
                Vector2.left,
                180f,
                offsetLeft
            );
        }


        // Third laser.
        if (range >= 3)
        {
            CreateLaser(
                Vector2.up,
                90f,
                offsetUp
            );
        }


        UpdateLaserVisibility();
    }


    private void CreateLaser(
        Vector2 dir,
        float angle,
        float distanceOffset)
    {
        GameObject laser =
            Instantiate(
                laserPrefab,
                transform
            );


        // Position relative to spawner.
        laser.transform.localPosition =
            dir * distanceOffset;


        // Rotate correctly.
        laser.transform.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );


        // Add oscillator.
        LaserOscillator osc =
            laser.AddComponent<LaserOscillator>();

        osc.range = moveRange;
        osc.speed = moveSpeed;

        osc.startPos =
            laser.transform.localPosition;


        // Random phase for non-simultaneous movement.
        osc.phaseOffset =
            Random.Range(
                0f,
                Mathf.PI * 2f
            );


        activeLasers.Add(laser);


        // Immediately give this laser the current damage.
        UpdateLaserDamage(laser);
    }


    private void ClearLasers()
    {
        foreach (GameObject laser in activeLasers)
        {
            if (laser != null)
            {
                Destroy(laser);
            }
        }

        activeLasers.Clear();
    }


    // =========================================================
    // LASER VISIBILITY
    // =========================================================

    public override void ToggleGFXVisibility(
        bool visible)
    {
        if (activeLasers == null ||
            activeLasers.Count == 0)
        {
            return;
        }


        foreach (GameObject laser in activeLasers)
        {
            if (laser != null)
            {
                laser.SetActive(visible);
            }
        }


        UpdateLaserVisibility();
    }


    private void UpdateLaserVisibility()
    {
        if (activeLasers == null ||
            activeLasers.Count == 0)
        {
            return;
        }


        /*
         * The current Laser setup supports up to 3 lasers.
         *
         * projectileCount determines how many are active.
         *
         * Count 1:
         *   Right
         *
         * Count 2:
         *   Right + Left
         *
         * Count 3+:
         *   Right + Left + Up
         */


        int numberOfActiveLasers =
            Mathf.Clamp(
                Mathf.RoundToInt(projectileCount),
                1,
                activeLasers.Count
            );


        for (int i = 0;
             i < activeLasers.Count;
             i++)
        {
            if (activeLasers[i] == null)
                continue;


            bool shouldBeVisible =
                isActive &&
                i < numberOfActiveLasers;


            activeLasers[i].SetActive(
                shouldBeVisible
            );
        }
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();

        UpdateDamageForEachLaser();
    }


    private void UpdateDamageForEachLaser()
    {
        if (activeLasers == null)
            return;


        foreach (GameObject laser in activeLasers)
        {
            UpdateLaserDamage(laser);
        }
    }


    private void UpdateLaserDamage(
        GameObject laser)
    {
        if (laser == null)
            return;


        /*
         * Your current laser prefab expects:
         *
         * Child 2 -> Damage component
         *
         * Keep that structure.
         */

        if (laser.transform.childCount <= 2)
        {
            Debug.LogWarning(
                $"Laser '{laser.name}' does not have " +
                $"the expected Damage child."
            );

            return;
        }


        Damage damageComponent =
            laser.transform
                .GetChild(2)
                .GetComponent<Damage>();


        if (damageComponent == null)
        {
            Debug.LogWarning(
                $"Laser '{laser.name}' is missing " +
                $"the Damage component on child 2."
            );

            return;
        }


        damageComponent.damage =
            damage;
    }


    // =========================================================
    // WEAPON LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeLaser();
    }


    private void UpgradeLaser()
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
                    -0.5f
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
                    EStatType.Damage,
                    5f
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

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

                statManager.ModifyStatValue(
                    EStatType.Damage,
                    5f
                );

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

                statManager.ModifyStatValue(
                    EStatType.AttackCooldown,
                    -0.5f
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

                break;


            // -------------------------------------------------
            // MAX
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
    // STAT CHANGE
    // =========================================================

    protected override void UpdateStatsHandled()
    {
        // IMPORTANT:
        //
        // WeaponBase reads the values from THIS weapon's
        // StatManager.
        //
        // It does NOT read player stats anymore.
        base.UpdateStatsHandled();


        // Update damage on every laser.
        UpdateDamageForEachLaser();


        // Update which lasers should be active.
        UpdateLaserVisibility();
    }
}