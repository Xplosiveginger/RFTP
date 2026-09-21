using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStat", menuName = "ScriptableObjects/ProgressionSO")]
public class ProgressionSO : ScriptableObject
{
    // --------------------------------------------------
    // Player Stats
    // --------------------------------------------------

    [FoldoutGroup("Player Stats")]
    public int coins = 0;

    [FoldoutGroup("Player Stats")]
    public int XP = 0;

    // --------------------------------------------------
    // XP System
    // --------------------------------------------------

    [FoldoutGroup("XP System")]
    [Space(10f)]
    public int startXP = 100;

    [FoldoutGroup("XP System")]
    [ReadOnly]
    public int currentXP = 0;

    [FoldoutGroup("XP System")]
    [ReadOnly]
    public int currentLevel = 1;

    [FoldoutGroup("XP System")]
    [ReadOnly]
    public int currentXPRequired = 0;

    // --------------------------------------------------
    // XP Scaling
    // --------------------------------------------------

    [FoldoutGroup("XP Scaling")]
    [Space(10f)]
    [ReadOnly]
    [LabelText("Level Gap Per Range")]
    public int levelGap = 10;

    [FoldoutGroup("XP Scaling")]
    [LabelText("XP Multipliers")]
    [InfoBox(
        "Each multiplier is used for one level range.\n" +
        "Example:\n" +
        "[0] = Levels 1-10\n" +
        "[1] = Levels 11-20\n" +
        "[2] = Levels 21-30")]
    public List<float> xpMultipliers = new List<float>()
    {
        0.4f,
        0.5f,
        0.6f,
        0.7f
    };

    [FoldoutGroup("XP Scaling")]
    [LabelText("XP Constant Additions")]
    [InfoBox(
        "Each addition value is used for one level range.\n" +
        "Example:\n" +
        "[0] = Levels 1-10\n" +
        "[1] = Levels 11-20\n" +
        "[2] = Levels 21-30")]
    public List<int> xpAdditions = new List<int>()
    {
        20,
        50,
        100,
        150

    };

    // --------------------------------------------------
    // Testing Zone
    // --------------------------------------------------

    [FoldoutGroup("Testing Zone")]
    [Space(10f)]
    [MinValue(1)]
    public int testLevel = 1;

    [FoldoutGroup("Testing Zone")]
    [Button("Calculate XP")]
    private void CalculateTestXP()
    {
        testedXP = GetXPRequiredForLevel(testLevel);
    }

    [FoldoutGroup("Testing Zone")]
    [ReadOnly]
    public int testedXP = 0;

    // --------------------------------------------------
    // Initialization
    // --------------------------------------------------

    private void OnEnable()
    {
        if (currentXPRequired == 0)
        {
            currentXPRequired = startXP;
            currentLevel = 1;
        }
    }

    // --------------------------------------------------
    // XP Logic
    // --------------------------------------------------

    public void AddXP(int xpAmount)
    {
        if (xpAmount <= 0)
            return;

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
        // Use NEXT level's range values
        float multiplier = GetMultiplierForLevel(currentLevel + 1);
        int addition = GetAdditionForLevel(currentLevel + 1);

        currentXPRequired =
            Mathf.RoundToInt(currentXPRequired * multiplier) + addition;
    }

    // --------------------------------------------------
    // Range Logic
    // --------------------------------------------------

    private int GetRangeIndex(int level)
    {
        int index = Mathf.FloorToInt((level - 1) / levelGap);

        int maxIndex = Mathf.Max(
            xpMultipliers.Count - 1,
            xpAdditions.Count - 1
        );

        index = Mathf.Clamp(index, 0, maxIndex);

        return index;
    }

    private float GetMultiplierForLevel(int level)
    {
        if (xpMultipliers == null || xpMultipliers.Count == 0)
            return 1f;

        int index = GetRangeIndex(level);

        index = Mathf.Clamp(index, 0, xpMultipliers.Count - 1);

        return xpMultipliers[index];
    }

    private int GetAdditionForLevel(int level)
    {
        if (xpAdditions == null || xpAdditions.Count == 0)
            return 0;

        int index = GetRangeIndex(level);

        index = Mathf.Clamp(index, 0, xpAdditions.Count - 1);

        return xpAdditions[index];
    }

    // --------------------------------------------------
    // Public Getters
    // --------------------------------------------------

    public int GetCurrentXPRequired()
    {
        if (currentXPRequired == 0)
            return startXP;

        return currentXPRequired;
    }

    public int GetXPRequiredForLevel(int level)
    {
        return GetXPRequiredForLevel(level, startXP);
    }

    private int GetXPRequiredForLevel(int level, int baseXP)
    {
        if (level <= 1)
            return baseXP;

        int xpRequired = baseXP;

        for (int i = 1; i < level; i++)
        {
            // Use TARGET level's range values
            float multiplier = GetMultiplierForLevel(i + 1);
            int addition = GetAdditionForLevel(i + 1);

            xpRequired =
                Mathf.RoundToInt(xpRequired * multiplier) + addition;
        }

        return xpRequired;
    }

    // --------------------------------------------------
    // Utility
    // --------------------------------------------------

    private void OnLevelUp()
    {
        Debug.Log(
            $"Level Up! Now at level {currentLevel}. " +
            $"Next level requires {currentXPRequired} XP."
        );
    }

    public void ResetProgression()
    {
        currentXP = 0;
        currentLevel = 1;
        currentXPRequired = startXP;
    }

    public float GetProgressPercentage()
    {
        if (currentXPRequired <= 0)
            return 0;

        return (float)currentXP / currentXPRequired;
    }
}