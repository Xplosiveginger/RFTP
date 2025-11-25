using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public StatManager statManager;

    private float damage;
    private float projectileSpeed;
    private float projectileCount;
    private float AOESize;
    private float cooldown;
    private float duration;

    private void Start()
    {
        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
    }
}
