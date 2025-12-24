using UnityEngine;

/// <summary>
/// Serializable stat data container for player attributes
/// Handles base values, multipliers, and permanent modifications
/// </summary>
[System.Serializable]
public class Stat
{
    [Header("Stat Identification")]
    public EStatType statName;

    [Header("Base Values")]
    public float baseValue = 0f;           // Original unmodified value
    public float maxValue = 0f;            // Maximum possible value (can scale)
    public float startValue = 0f;          // Pregame starting value (items/gear)

    [Header("Multipliers")]
    public float startMultiplier = 1f;     // Initial multiplier from gear
    public float currentMultiplier = 1f;   // Live multiplier from all sources

    [Header("Runtime")]
    public float currentValue;             // Live value used by game logic
    public bool customMaxValue;            // Use maxValue instead of baseValue
    public bool showCurrentValues;         // Show in UI/debug

    /// <summary>
    /// Initialize stat values on startup
    /// </summary>
    public void Init()
    {
        // Set max from base if not specified
        if (maxValue == 0) maxValue = baseValue;

        // Use start value or full max
        if (startValue > 0f) currentValue = startValue;
        else currentValue = maxValue;
    }

    /// <summary>
    /// Apply permanent percentage modifier (+10 = +10%)
    /// </summary>
    public void ApplyModifier(float modifier)
    {
        float tempMultiplier = modifier / 100f;
        float valueToAdd = baseValue * tempMultiplier;
        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;
    }

    /// <summary>
    /// Revert a previously applied modifier (called by StatManager)
    /// </summary>
    public void RevertModifier(float modifier)
    {
        float tempMultiplier = -modifier / 100f;
        float valueToAdd = baseValue * tempMultiplier;
        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;
    }
    /// <summary>
    /// Flat stat change (used for non-percentage stats).
    /// </summary>
    public void AddFlat(float value)
    {
        currentValue += value;
    }
    
}
