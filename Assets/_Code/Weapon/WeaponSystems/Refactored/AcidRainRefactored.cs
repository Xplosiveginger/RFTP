using System.Collections.Generic;
using UnityEngine;

public class AcidRainRefactored : WeaponBase
{
    [Header("Spawn Area")]
    public float boxLength = 5f;
    public float boxBreadth = 3f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;

    [Header("Spawn Timing")]
    public bool spawnOnEnable = true;

    private readonly List<GameObject> puddles =
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
        base.Start();

        Debug.Log(
            $"[{name}] Acid Rain Damage: {damage}"
        );
    }


    // =========================================================
    // PROJECTILE SPAWNING
    // =========================================================

    public void SpawnProjectiles()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                $"AcidRain '{name}' has no projectilePrefab."
            );

            return;
        }

        // WeaponBase keeps projectileCount as a float internally,
        // but projectile count must always be a whole number.
        int count = Mathf.Max(
            0,
            Mathf.RoundToInt(projectileCount)
        );

        for (int i = 0; i < count; i++)
        {
            // Pick a random point inside the box.
            float randX = Random.Range(
                -boxLength / 2f,
                boxLength / 2f
            );

            float randY = Random.Range(
                -boxBreadth / 2f,
                boxBreadth / 2f
            );

            Vector3 spawnPos =
                transform.position +
                new Vector3(
                    randX,
                    randY,
                    0f
                );

            GameObject go = Instantiate(
                projectilePrefab,
                spawnPos,
                Quaternion.identity
            );

            PreSecDamage damageComponent =
                go.GetComponent<PreSecDamage>();

            if (damageComponent != null)
            {
                damageComponent.Initialize(
                    damage,
                    AOESize
                );
            }
            else
            {
                Debug.LogWarning(
                    $"Acid Rain projectile '{go.name}' " +
                    "is missing PreSecDamage."
                );
            }

            puddles.Add(go);
        }
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                boxLength,
                boxBreadth,
                0f
            )
        );
    }


    // =========================================================
    // GRAPHICS / PROJECTILES
    // =========================================================

    public override void ToggleGFXVisibility(
        bool visible)
    {
        if (!visible)
        {
            ClearPuddles();
        }
        else
        {
            SpawnProjectiles();
        }
    }


    private void ClearPuddles()
    {
        foreach (GameObject puddle in puddles)
        {
            if (puddle != null)
            {
                Destroy(puddle);
            }
        }

        puddles.Clear();
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
    // WEAPON UPDATE
    // =========================================================

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();
    }


    // =========================================================
    // STAT UPDATE
    // =========================================================

    protected override void UpdateStatsHandled()
    {
        /*
         * WeaponBase is responsible for calculating the
         * effective values.
         *
         * The calculation is based on:
         *
         * Weapon's own base stat
         * + Weapon level upgrades
         * + Weapon-specific modifiers
         * × Player global multiplier
         * + Player flat modifier
         *
         * Therefore we must NOT directly do:
         *
         * damage = playerStat.currentValue;
         *
         * or:
         *
         * damage = statManager.GetStat(...).currentValue;
         *
         * here.
         */

        base.UpdateStatsHandled();

        /*
         * Existing puddles keep the values they were
         * initialized with.
         *
         * Newly spawned puddles will automatically use
         * the latest damage and AOE values.
         */
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeAcidRain();
    }


    private void UpgradeAcidRain()
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

                // +10% AOE Size
                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
                );

                break;


            // -------------------------------------------------
            // LEVEL 3
            // -------------------------------------------------

            case 3:

                // +10 flat damage
                statManager.ModifyStatValue(
                    EStatType.Damage,
                    10f
                );

                // +0.5 flat duration
                statManager.ModifyStatValue(
                    EStatType.ActiveDuration,
                    0.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 4
            // -------------------------------------------------

            case 4:

                // +1 projectile
                statManager.ModifyStatValue(
                    EStatType.ProjectileCount,
                    1f
                );

                // +10% AOE Size
                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
                );

                break;


            // -------------------------------------------------
            // LEVEL 5
            // -------------------------------------------------

            case 5:

                // +10 flat damage
                statManager.ModifyStatValue(
                    EStatType.Damage,
                    10f
                );

                // +0.5 flat duration
                statManager.ModifyStatValue(
                    EStatType.ActiveDuration,
                    0.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

                // +1 projectile
                statManager.ModifyStatValue(
                    EStatType.ProjectileCount,
                    1f
                );

                // +10% AOE Size
                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
                );

                break;


            // -------------------------------------------------
            // LEVEL 7
            // -------------------------------------------------

            case 7:

                // +10 flat damage
                statManager.ModifyStatValue(
                    EStatType.Damage,
                    10f
                );

                // +0.5 flat duration
                statManager.ModifyStatValue(
                    EStatType.ActiveDuration,
                    0.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 8
            // -------------------------------------------------

            case 8:

                // +10% AOE Size
                statManager.ModifyStat(
                    EStatType.AOESize,
                    10f
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