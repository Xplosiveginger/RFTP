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

    [Title("Current Run")]
    [SerializeField, Min(0)]
    [ReadOnly]
    private int currentRunMoney;

    [Title("Overall Money")]
    [SerializeField, Min(0)]
    [ReadOnly]
    private int overallMoney;

    public event Action<long> OnRunMoneyChanged;
    public event Action<long> OnOverallMoneyChanged;

    public long CurrentRunMoney => currentRunMoney;
    public long OverallMoney => overallMoney;

    private void Start()
    {
        // Load permanent money from the save system.
        if (GameSaveSystem.instance == null)
        {
            Debug.LogError("GameSaveSystem not found.");
            return;
        }

        overallMoney = GameSaveSystem.instance.GetMoney();

        // Every new run starts with 0 earned money.
        currentRunMoney = 0;

        NotifyRunMoneyChanged();
        NotifyOverallMoneyChanged();
    }

    // =========================================================
    // ADD MONEY
    // =========================================================

    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        // Money earned during this run.
        currentRunMoney += amount;

        // Add the same money to permanent player money.
        overallMoney += amount;

        // Save the new permanent balance.
        GameSaveSystem.instance.SetMoney(overallMoney);

        NotifyRunMoneyChanged();
        NotifyOverallMoneyChanged();
    }

    // =========================================================
    // OVERALL MONEY
    // =========================================================

    public bool CanAfford(int cost)
    {
        if (cost <= 0)
            return true;

        return overallMoney >= cost;
    }

    public bool Purchase(int cost)
    {
        if (!CanAfford(cost))
            return false;

        overallMoney -= cost;

        // Save the new permanent balance.
        GameSaveSystem.instance.SetMoney(overallMoney);

        NotifyOverallMoneyChanged();

        return true;
    }

    public void SetOverallMoney(int amount)
    {
        overallMoney = Math.Max(0, amount);

        GameSaveSystem.instance.SetMoney(overallMoney);

        NotifyOverallMoneyChanged();
    }

    // =========================================================
    // CURRENT RUN
    // =========================================================

    public void ResetRunMoney()
    {
        currentRunMoney = 0;

        NotifyRunMoneyChanged();
    }

    // =========================================================
    // FORMATTING
    // =========================================================

    public string GetFormattedRunMoney()
    {
        return FormatCurrency(currentRunMoney);
    }

    public string GetFormattedOverallMoney()
    {
        return FormatCurrency(overallMoney);
    }

    private string FormatCurrency(int value)
    {
        if (value < 1000)
            return value.ToString();

        string[] suffixes =
        {
            "K",
            "M",
            "B",
            "T",
            "Qa",
            "Qi"
        };

        double number = value;
        int suffixIndex = -1;

        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;
        }

        return $"{number:0.#}{suffixes[suffixIndex]}";
    }

    // =========================================================
    // EVENTS
    // =========================================================

    private void NotifyRunMoneyChanged()
    {
        OnRunMoneyChanged?.Invoke(currentRunMoney);
    }

    private void NotifyOverallMoneyChanged()
    {
        OnOverallMoneyChanged?.Invoke(overallMoney);
    }

#if UNITY_EDITOR

    [Title("Testing")]

    [Button(ButtonSizes.Medium)]
    private void Add100()
    {
        AddMoney(100);
    }

    [Button(ButtonSizes.Medium)]
    private void ResetRun()
    {
        ResetRunMoney();
    }

    [Button(ButtonSizes.Medium)]
    private void ResetOverallMoney()
    {
        SetOverallMoney(0);
    }

#endif
}