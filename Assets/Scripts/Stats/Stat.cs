using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stat"), Serializable]
public class Stat : ScriptableObject
{
    public EStatType statName;
    public float baseValue = 0f;
    public float maxValue = 0f; // incase the max value also scales
    public float startValue = 0f; // pregame items
    public float startMultiplier = 1f;
    
    public float currentMultiplier = 1f;
    public float currentValue;

    public bool customMaxValue;
    public bool showCurrentValues;

    public event Action<Stat> OnCurrentValueChanged;
    public event Action OnMaxValueChanged;

    public void Init()
    {
        if(maxValue == 0)
            maxValue = baseValue;

        if(startValue > 0f)
            currentValue = startValue;
        else
        {
            currentValue = (statName == EStatType.Health)? maxValue : baseValue;
        }

        currentMultiplier = startMultiplier;
    }

    public void ApplyModifier(float modifier)
    {
        float tempMultiplier = modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;
        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;

        OnCurrentValueChanged?.Invoke(this);
    }

    public void ApplyHealthModifier(float modifier)
    {
        if (statName != EStatType.Health) return;

        float tempMultiplier = modifier / 100f;

        float valueToAdd = maxValue * tempMultiplier;
        currentValue += valueToAdd;
        OnCurrentValueChanged?.Invoke(this);
        maxValue += valueToAdd;
        OnMaxValueChanged.Invoke();
        currentMultiplier += tempMultiplier;
    }

    public void ApplyCooldownModifier(float modifier)
    {
        if(statName != EStatType.AttackCooldown) return;

        float tempMultiplier = modifier / 100f;

        float valueToRemove = baseValue * tempMultiplier;
        currentValue -= valueToRemove;
        OnCurrentValueChanged?.Invoke(this);
        currentMultiplier -= tempMultiplier;
    }
}

public enum EStatType
{
    MoveSpeed,
    Health,
    AttackCooldown,
    ActiveDuration,
    ProjectileCount,
    ProjectileSpeed,
    Damage,
    AOESize
}