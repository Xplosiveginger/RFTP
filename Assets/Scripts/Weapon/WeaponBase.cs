using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public StatManager statManager;
    public GameObject gfx;

    private float damage;
    private float projectileSpeed;
    private float projectileCount;
    private float AOESize;
    private float cooldown;
    private float duration;

    private float activeTimer;
    private float coolDownTimer;

    private bool isActive;
    private bool inCooldown;

    private void Start()
    {
        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
    }

    public void UpdateWeapon()
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
            gfx.SetActive(true);
            isActive = true;
            inCooldown = false;
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
        }
        else
        {
            coolDownTimer = cooldown;
            gfx.SetActive(false);
            isActive = false;
            inCooldown = true;
        }
    }
}
