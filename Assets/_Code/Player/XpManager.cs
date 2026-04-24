using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Sirenix.OdinInspector;

public class XpManager : MonoBehaviour
{
    [Header("Progression")]
    public ProgressionSO progressionSO;

    [Header("UI")]
    public Slider xpBar;
    public float fillDuration = 0.5f;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    [Header("Card Spawner")]
    public CardSpawner cardSpawner;

    public static event Action OnPlayerLeveledUp;

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
            Debug.LogWarning("Test XP amount must be greater than 0!");
            return;
        }

        AddXP(testXPAmount);
        Debug.Log($"Added {testXPAmount} test XP. Total XP: {progressionSO.XP}");
    }

    [FoldoutGroup("Testing")]
    [Button("Add Test Coins", ButtonSizes.Medium)]
    [GUIColor(0.5f, 0.8f, 1f)]
    private void AddTestCoins()
    {
        if (testCoinAmount <= 0)
        {
            Debug.LogWarning("Test coin amount must be greater than 0!");
            return;
        }

        AddCoins(testCoinAmount);
        Debug.Log($"Added {testCoinAmount} test coins. Total coins: {progressionSO.coins}");
    }

    [FoldoutGroup("Testing")]
    public int testCoinAmount = 50;

    [FoldoutGroup("Testing")]
    [Button("Reset Progression", ButtonSizes.Medium)]
    [GUIColor(1f, 0.5f, 0.5f)]
    private void ResetTestProgression()
    {
        ResetProgression();
        Debug.Log("Progression has been reset!");
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
            Debug.LogError("ProgressionSO is not assigned!");
            return;
        }

        if (xpBar != null)
        {
            xpBar.maxValue = 1f;
            xpBar.value = progressionSO.GetProgressPercentage();
        }

        UpdateUI();
    }

    public void AddXP(int amount)
    {
        if (progressionSO == null) return;

        int previousLevel = progressionSO.currentLevel;

        progressionSO.AddXP(amount);

        UpdateUI();

        if (progressionSO.currentLevel > previousLevel)
        {
            OnLevelUp();
        }

    }

    public void AddCoins(int amount)
    {
        if (progressionSO == null) return;

        progressionSO.coins += amount;
        UpdateUI();
    }

    private void OnLevelUp()
    {
        Debug.Log($"Level Up! You are now level {progressionSO.currentLevel}. Next level requires {progressionSO.currentXPRequired} XP.");

        UpdateUI();

        OnPlayerLeveledUp?.Invoke();
    }

    private void UpdateUI()
    {
        if (progressionSO == null) return;

        if (xpBar != null)
        {
            float targetValue = progressionSO.GetProgressPercentage();
            xpBar.DOValue(targetValue, fillDuration).SetEase(Ease.OutCubic);
        }

        if (levelText != null)
        {
            levelText.text = progressionSO.currentLevel.ToString();
        }

        if (xpText != null)
        {
            xpText.text = $"{progressionSO.currentXP}/{progressionSO.currentXPRequired}";
        }
    }
    [Button("Reset Progression", ButtonSizes.Medium)]
    public void ResetProgression()
    {
        if (progressionSO == null) return;

        progressionSO.ResetProgression();
        UpdateUI();
    }

    public float GetProgressPercentage()
    {
        if (progressionSO == null) return 0f;
        return progressionSO.GetProgressPercentage();
    }
}