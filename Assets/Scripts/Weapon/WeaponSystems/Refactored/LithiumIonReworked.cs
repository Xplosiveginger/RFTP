using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LithiumIonReworked : WeaponBase
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
        else if(firedProjectileCount >= projectileCount)
        {
            coolDownTimer = cooldown;
        }
    }

    private void ShootProjectiles()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        firedProjectileCount++;
        Vector3 shootDir = new Vector2(20, 30); // Change this later to detect enemies and fire in their direction.
        projectile.GetComponent<Rigidbody2D>().velocity = (shootDir - transform.position).normalized * projectileSpeed;
    }
}