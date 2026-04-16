using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuningForkRefactored : WeaponBase
{
    protected override void Awake()
    {
        base.Awake();

        UpdateStatsHandled();
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        LevelUpTuningFork();
    }

    private void LevelUpTuningFork()
    {
        switch (level)
        {
            case 1:
                break;
            case 2:
                statManager.ModifyStat(EStatType.AOESize, 10f);
                break;
            case 3:
                statManager.ModifyStatValue(EStatType.AttackCooldown, -0.5f);
                break;
            case 4:
                statManager.ModifyStat(EStatType.AOESize, 10f);
                break;
            case 5:
                statManager.ModifyStatValue(EStatType.ActiveDuration, 1.5f);
                break;
            case 6:
                statManager.ModifyStatValue(EStatType.AttackCooldown, -0.5f);
                break;
            case 7:
                statManager.ModifyStat(EStatType.AOESize, 10f);
                break;
            case 8:
                statManager.ModifyStatValue(EStatType.ActiveDuration, 1.5f);
                break;
            default:
                Debug.Log($"Max Level Reached for {weaponData.weaponName}");
                break;
        }
    }
}
