using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public StatManager statManager;
    public GameObject gfx;

    public WeaponDataSO weaponData;

    protected float damage;
    protected float projectileSpeed;
    protected float projectileCount;
    protected float AOESize;
    protected float cooldown;
    protected float duration;

    protected float activeTimer;
    protected float coolDownTimer;

    protected bool isActive;
    protected bool inCooldown;

    public event Action<WeaponBase> OnWeaponCreated;

    public virtual void SpawnWeapon(Transform parent)
    {
        if(weaponData.weaponPrefab != null)
        {
            WeaponBase weapon = Instantiate(weaponData.weaponPrefab, parent.position + Vector3.zero, Quaternion.identity, parent).GetComponent<WeaponBase>();
            OnWeaponCreated?.Invoke(weapon);
        }
    }

    protected virtual void Awake()
    {
        statManager.statList = weaponData.GetAllWeaponStats();
        statManager.InitializeStats();
    }

    protected virtual void Start()
    {
        UpdateWeaponDamage();
    }

    public virtual void UpdateWeapon()
    {
        Debug.Log("Updating Weapon");

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
            Debug.Log("Weapon Activated");
            activeTimer = duration;
            ToggleGFXVisibility(true);
            isActive = true;
            inCooldown = false;
            return;
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
            Debug.Log("Ticking Active");
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
        gfx.SetActive(b);
    }

    public virtual void UpdateWeaponDamage()
    {
        damage = statManager.GetStat(EStatType.Damage).currentValue;
    }
}
