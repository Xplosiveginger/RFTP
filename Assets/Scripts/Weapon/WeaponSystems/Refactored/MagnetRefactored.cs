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

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        LevelUpMagnet();
    }

    private void LevelUpMagnet()
    {
        switch (level)
        {
            case 1:
                break;
            case 2:
                statManager.ModifyStatValue(EStatType.Damage, 4f, false);
                break;
            case 3:
                statManager.ModifyStatValue(EStatType.Damage, 4f, false);
                statManager.ModifyStat(EStatType.AOESize, 15f, false);
                break;
            case 4:
                statManager.ModifyStatValue(EStatType.Damage, 4f, false);
                statManager.ModifyStatValue(EStatType.AttackCooldown, 0.5f, true);
                break;
            case 5:
                statManager.ModifyStatValue(EStatType.Damage, 4f, false);
                statManager.ModifyStat(EStatType.AOESize, 15f, false);
                break;
            case 6:
                statManager.ModifyStatValue(EStatType.Damage, 4f, false);
                statManager.ModifyStatValue(EStatType.AttackCooldown, 0.5f, true);
                break;
            case 7:
                statManager.ModifyStatValue(EStatType.Damage, 5f, false);
                break;
            case 8:
                statManager.ModifyStatValue(EStatType.Damage, 6f, false);
                break;
            default:
                Debug.Log($"Max Level Reached for {weaponData.weaponName}");
                break;
        }
    }
}