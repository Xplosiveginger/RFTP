using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class XpManager : MonoBehaviour
{
    [Header("Progression")]
    public ProgressionSO progressionSO;
    
    public static event Action OnPlayerLeveledUp;
    public static event Action OnXPUpdated;
    public static event Action OnCoinsUpdated;

    [FoldoutGroup("Testing")]
    [Space(10f)]
    [Tooltip("Amount of XP to add for testing")]
    public int testXPAmount = 100;

    [FoldoutGroup("Testing")]
    [Button("Add Test XP", ButtonSizes.Medium)]
    [GUIColor(0.5f, 1f, 0.5f)]
    private void AddTestXP()
    {
        if (testXPAmount <= 0)
        {
            return;
        }

        AddXP(testXPAmount);
    }

    [FoldoutGroup("Testing")]
    public int testCoinAmount = 50;

    [FoldoutGroup("Testing")]
    [Button("Add Test Coins", ButtonSizes.Medium)]
    [GUIColor(0.5f, 0.8f, 1f)]
    private void AddTestCoins()
    {
        if (testCoinAmount <= 0)
        {
            return;
        }

        AddCoins(testCoinAmount);
    }

    [FoldoutGroup("Testing")]
    [Button("Reset Progression", ButtonSizes.Medium)]
    [GUIColor(1f, 0.5f, 0.5f)]
    private void ResetTestProgression()
    {
        ResetProgression();
    }

    [FoldoutGroup("Testing/Info")]
    [ReadOnly]
    [DisplayAsString]
    public string CurrentLevelInfo => $"Level: {progressionSO?.currentLevel ?? 0}";

    [FoldoutGroup("Testing/Info")]
    [ReadOnly]
    [DisplayAsString]
    public string CurrentXPInfo => $"XP: {progressionSO?.currentXP ?? 0}/{progressionSO?.currentXPRequired ?? 0}";

    [FoldoutGroup("Testing/Info")]
    [ReadOnly]
    [DisplayAsString]
    public string TotalXPInfo => $"Total XP Earned: {progressionSO?.XP ?? 0}";

    [FoldoutGroup("Testing/Info")]
    [ReadOnly]
    [DisplayAsString]
    public string TotalCoinsInfo => $"Total Coins: {progressionSO?.coins ?? 0}";

    [FoldoutGroup("Testing/Info")]
    [ReadOnly]
    [DisplayAsString]
    public string ProgressPercentInfo => $"Progress: {(progressionSO?.GetProgressPercentage() ?? 0) * 100:F1}%";

    private void Start()
    {
        if (progressionSO == null)
        {
            return;
        }
        ResetProgression();
        // Notify UI of initial state
        OnXPUpdated?.Invoke();
        OnCoinsUpdated?.Invoke();
    }

    public void AddXP(int amount)
    {
        if (progressionSO == null) return;

        int previousLevel = progressionSO.currentLevel;

        progressionSO.AddXP(amount);

        OnXPUpdated?.Invoke();

        if (progressionSO.currentLevel > previousLevel)
        {
            OnLevelUp();
        }
    }

    public void AddCoins(int amount)
    {
        if (progressionSO == null) return;

        progressionSO.coins += amount;
        OnCoinsUpdated?.Invoke();
    }

    private void OnLevelUp()
    {

        OnXPUpdated?.Invoke();
        OnPlayerLeveledUp?.Invoke();
    }

    [Button("Reset Progression", ButtonSizes.Medium)]
    public void ResetProgression()
    {
        if (progressionSO == null) return;

        progressionSO.ResetProgression();
        OnXPUpdated?.Invoke();
        OnCoinsUpdated?.Invoke();
    }

    public float GetProgressPercentage()
    {
        if (progressionSO == null) return 0f;
        return progressionSO.GetProgressPercentage();
    }
}