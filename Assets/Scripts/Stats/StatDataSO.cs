using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stat"), Serializable]
public class StatDataSO : ScriptableObject
{
    public EStatType statName;
    public float baseValue = 0f;
    public float maxValue = 0f; // incase the max value also scales
    public float startValue = 0f; // pregame items
    public float startMultiplier = 1f;

    public Stat Init()
    {
        return new Stat(statName, baseValue, maxValue, startValue, startMultiplier);
    }
}

public enum EStatType
{
    MoveSpeed,
    Health,
    HealthRegen,
    AttackCooldown,
    ActiveDuration,
    ProjectileCount,
    ProjectileSpeed,
    Damage,
    AOESize,
    FireRate
}