using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{

    public static EconomyManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public GameStat_SO GameStat_SO;
    
    [Title("Economy")]
    [SerializeField, Min(0)]
    [ReadOnly]
    private int allowanceMoney;

    public event Action<long> OnAllowanceMoneyChanged;

    public long AllowanceMoney => allowanceMoney;

    private void Start()
    {
        allowanceMoney = GameStat_SO.AllowanceMoney;
    }

    void UpdateCurrencyInGameStat()
    {
        GameStat_SO.UpdateAllowanceMoney(allowanceMoney);
    }
    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        allowanceMoney += amount;
        NotifyCurrencyChanged();
    }

    public bool CanAfford(int cost)
    {
        if (cost <= 0)
            return true;

        return allowanceMoney >= cost;
    }

    public bool Purchase(int cost)
    {
        if (!CanAfford(cost))
            return false;

        allowanceMoney -= cost;
        NotifyCurrencyChanged();

        return true;
    }

    public void SetMoney(int amount)
    {
        allowanceMoney = Math.Max(0, amount);
        NotifyCurrencyChanged();
    }

    public string GetFormattedMoney()
    {
        return FormatCurrency(allowanceMoney);
    }

    string FormatCurrency(int value)
    {
        if (value < 1000)
            return value.ToString();

        string[] suffixes = { "K", "M", "B", "T", "Qa", "Qi" };

        double number = value;
        int suffixIndex = -1;

        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;
        }

        return $"{number:0.#}{suffixes[suffixIndex]}";
    }

    private void NotifyCurrencyChanged()
    {
        UpdateCurrencyInGameStat();
        OnAllowanceMoneyChanged?.Invoke(allowanceMoney);
    }

#if UNITY_EDITOR

    [Title("Testing")]

    [Button(ButtonSizes.Medium)]
    private void Add100()
    {
        AddMoney(100);
    }

    [Button(ButtonSizes.Medium)]
    private void ResetMoney()
    {
        SetMoney(0);
    }

#endif
}