using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStat", menuName = "ScriptableObjects/ProgressionSO")]
public class ProgressionSO : ScriptableObject
{
    [FoldoutGroup("Player Stats")]
    public int coins = 0;

    [FoldoutGroup("Player Stats")]
    public int XP = 0;

    [FoldoutGroup("XP System")]
    [Space(10f)]
    public int startXP = 100;

    [FoldoutGroup("XP System")]
    [ReadOnly] public int currentXP = 0;

    [FoldoutGroup("XP System")]
    [ReadOnly] public int currentLevel = 1;

    [FoldoutGroup("XP System")]
    [ReadOnly] public int currentXPRequired = 0;

    [FoldoutGroup("XP Scaling")]
    [Space(10f)]
    public float xpMultiplier = 1.2f;

    [FoldoutGroup("XP Scaling")]
    public int xpAddition = 50;

    [FoldoutGroup("Testing Zone")]
    [Space(10f)]
    public int testLevel = 0;

    [FoldoutGroup("Testing Zone")]
    public int testBaseXP = 100;

    [FoldoutGroup("Testing Zone")]
    [Button("Calculate XP")]
    private void CalculateTestXP()
    {
        if (testBaseXP <= 0) testBaseXP = 100;

        int calculatedXP = testBaseXP;

        if (testLevel > 0)
        {
            calculatedXP = testBaseXP;
            for (int i = 1; i < testLevel; i++)
            {
                calculatedXP = Mathf.RoundToInt(calculatedXP * xpMultiplier) + xpAddition;
            }
        }

        testedXP = calculatedXP;
    }

    [FoldoutGroup("Testing Zone")]
    [ReadOnly] public int testedXP = 0;

    private void OnEnable()
    {
        if (currentXPRequired == 0)
        {
            currentXPRequired = startXP;
            currentLevel = 1;
        }
    }

    public void AddXP(int xpAmount)
    {
        if (xpAmount <= 0) return;

        currentXP += xpAmount;
        XP += xpAmount;

        while (currentXP >= currentXPRequired)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= currentXPRequired;
        currentLevel++;
        CalculateNextXPRequired();
        OnLevelUp();
    }

    private void CalculateNextXPRequired()
    {
        currentXPRequired = Mathf.RoundToInt(currentXPRequired * xpMultiplier) + xpAddition;
    }

    public int GetCurrentXPRequired()
    {
        if (currentXPRequired == 0)
        {
            return startXP;
        }
        return currentXPRequired;
    }

    public int GetXPRequiredForLevel(int level)
    {
        if (level <= 1) return startXP;

        int xpRequired = startXP;
        for (int i = 1; i < level; i++)
        {
            xpRequired = Mathf.RoundToInt(xpRequired * xpMultiplier) + xpAddition;
        }
        return xpRequired;
    }

    private void OnLevelUp()
    {
        Debug.Log($"Level Up! Now at level {currentLevel}. Next level requires {currentXPRequired} XP.");
    }

    public void ResetProgression()
    {
        currentXP = 0;
        currentLevel = 1;
        currentXPRequired = startXP;
    }

    public float GetProgressPercentage()
    {
        if (currentXPRequired <= 0) return 0;
        return (float)currentXP / currentXPRequired;
    }
}