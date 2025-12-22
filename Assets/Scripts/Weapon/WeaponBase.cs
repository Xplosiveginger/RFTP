using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public StatManager statManager;
    public GameObject gfx;

    public WeaponDataSO weaponData;
    public EnemyDetection enemyDetector;

    protected float damage;
    protected float projectileSpeed;
    protected float projectileCount;
    protected float AOESize;
    protected float cooldown;
    protected float duration;
    protected float fireRate;

    protected float activeTimer;
    protected float coolDownTimer;

    [SerializeField, DisplayOnly] protected bool isActive;
    [SerializeField, DisplayOnly] protected bool inCooldown;

    public event Action<WeaponBase> OnWeaponCreated;

    public virtual void SpawnWeapon(Transform parent)
    {
        if(weaponData.weaponPrefab != null)
        {
            WeaponBase weapon = Instantiate(weaponData.weaponPrefab, parent.position + Vector3.zero, Quaternion.identity, parent).GetComponent<WeaponBase>();
            OnWeaponCreated?.Invoke(weapon);
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    protected virtual void Awake()
    {
        statManager = GetComponent<StatManager>();

        statManager.statList = weaponData.GetAllWeaponStats();
        statManager.InitializeStats();

        statManager.OnStatChanged += UpdateStatsHandled;
    }

    protected virtual void Start()
    {
        UpdateWeaponDamage();
    }

    public virtual void UpdateWeapon()
    {
        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;
            inCooldown = true;
            isActive = false;
            return; // exit out since the weapon is in cooldown
        }

        // cooldown done. Activate weapon if it is not active.
        if (!isActive)
        {
            activeTimer = duration;
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
            coolDownTimer = cooldown;
            ToggleGFXVisibility(false);
            isActive = false;
            inCooldown = true;
        }
    }

    public virtual void ToggleGFXVisibility(bool b)
    {
        if (gfx == null) return;
        gfx.SetActive(b);
    }

    public virtual void UpdateWeaponDamage()
    {
        damage = statManager.GetStat(EStatType.Damage).currentValue;
    }

    protected void UpdateStatsHandled()
    {
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        damage = statManager.GetStat(EStatType.Damage).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        fireRate = statManager.GetStat(EStatType.FireRate).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
    }

    public virtual void LevelUpWeapon()
    {

    }
}
