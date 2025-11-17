using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/Stat")]
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

    public void Init()
    {
        if(maxValue == 0)
            maxValue = baseValue;

        if(startValue > 0f)
            currentValue = startValue;
        else
            currentValue = maxValue;
    }

    public void ApplyModifier(float modifier)
    {
        float tempMultiplier = modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;
        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;
    }
}

public enum EStatType
{
    MoveSpeed,
    Health,
    AttackSpeed,
    AttackCooldown,
}