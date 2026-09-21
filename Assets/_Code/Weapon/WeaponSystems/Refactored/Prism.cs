using UnityEngine;

public class Prism : WeaponBase
{
    public GameObject projectilePrefab;
    private float timeToFire;
    private int firedProjectileCount;

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();
    }

    public override void SpawnWeapon(Transform parent)
    {
        base.SpawnWeapon(parent);
    }

    public override void ToggleGFXVisibility(bool b)
    {
        base.ToggleGFXVisibility(b);
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

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();
    }

    private void ShootProjectiles()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().damage = damage;
        firedProjectileCount++;
        Vector3 shootAt = enemyDetector.GetPositionOfNearestEnemy(); // Change this later to detect enemies and fire in their direction.
        projectile.GetComponent<Rigidbody2D>().linearVelocity = (shootAt - transform.position).normalized * projectileSpeed;
    }

    protected override void Awake()
    {
        base.Awake();

        damage = statManager.TryGetStat(EStatType.Damage).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
    }
    

    protected override void Start()
    {
        base.Start();
    }

    protected override void UpdateStatsHandled()
    {
        base.UpdateStatsHandled();
    }
}
