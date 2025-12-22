using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetRefactored : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();

        damage = statManager.GetStat(EStatType.Damage).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
    }

    protected override void Start()
    {
        base.Start();
    }

    public float GetStatValue(EStatType type)
    {
        switch (type)
        {
            case EStatType.Damage:
                return damage;
            case EStatType.AOESize:
                return AOESize;
            default:
                Debug.LogWarning("You are trying to access a stat that is not defined in the function.");
                return -1.0f;
        }
    }
}