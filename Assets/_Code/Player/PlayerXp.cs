using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

public class XPManager : MonoBehaviour
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

        if (progressionSO.currentLevel > previousLevel)
        {
            OnLevelUp();
        }

        UpdateUI();
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