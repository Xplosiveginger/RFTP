using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BacteriaReworked : WeaponBase
{
    public GameObject projectilePrefab;
    private float timeToFire;
    private int firedProjectileCount;

    protected override void Awake()
    {
        base.Awake();

        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        fireRate = statManager.GetStat(EStatType.FireRate).currentValue;
    }

    protected override void Start()
    {
        UpdateWeaponDamage();
    }

    public override void UpdateWeapon()
    {
        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;
            firedProjectileCount = 0;
            inCooldown = true;
            isActive = false;
            return; // exit out since the weapon is in cooldown
        }
        isActive = true;
        inCooldown = false;

        if (firedProjectileCount < projectileCount && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            ShootProjectiles();
        }
        else if (firedProjectileCount >= projectileCount)
        {
            coolDownTimer = cooldown;
        }
    }

    private void ShootProjectiles()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().damage = damage;
        firedProjectileCount++;
        Vector3 shootAt = enemyDetector.GetPositionOfRandomEnemy(); // Change this later to detect enemies and fire in their direction.
        projectile.GetComponent<Rigidbody2D>().velocity = (shootAt - transform.position).normalized * projectileSpeed;
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeBacteria();
    }

    private void UpgradeBacteria()
    {
        switch(level)
        {
            case 1:
                break;
            case 2:
                statManager.ModifyStatValue(EStatType.AttackCooldown, 0.3f, true);
                break;
            case 3:
                statManager.ModifyStatValue(EStatType.Damage, 5f, false);
                break;
            case 4:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1f, false);
                break;
            case 5:
                statManager.ModifyStatValue(EStatType.Damage, 5f, false);
                statManager.ModifyStatValue(EStatType.AttackCooldown, 0.3f, true);
                break;
            case 6:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1f, false);
                break;
            case 7:
                statManager.ModifyStatValue(EStatType.Damage, 5f, false);
                break;
            case 8:
                statManager.ModifyStatValue(EStatType.Damage, 5f, false);
                statManager.ModifyStatValue(EStatType.AttackCooldown, 0.3f, true);
                break;
            default:
                break;
        }
    }

    protected override void UpdateStatsHandled()
    {
        base.UpdateStatsHandled();
    }
}