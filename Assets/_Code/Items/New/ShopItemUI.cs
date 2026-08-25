using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public static class CurrencyFormatter
{
    public static string Format(double amount, int decimalPlaces = 2)
    {
        double absAmount = Math.Abs(amount);

        string suffix = "";
        double value = amount;

        if (absAmount >= 1_000_000_000)
        {
            value = amount / 1_000_000_000d;
            suffix = "B";
        }
        else if (absAmount >= 1_000_000)
        {
            value = amount / 1_000_000d;
            suffix = "M";
        }
        else if (absAmount >= 1_000)
        {
            value = amount / 1_000d;
            suffix = "K";
        }

        return value.ToString($"F{decimalPlaces}")
                   .TrimEnd('0')
                   .TrimEnd('.')
               + suffix;
    }
}

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    public Button buyBtn;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI costText;

    public ShopItemSO item;

    [Header("Levels")]
    public GameObject[] grayDots;

    private bool selected;
    private int currentLevel = 0;

    public static event Action<ShopItemSO, int, int> OnItemAdded;

    private void Start()
    {
        buyBtn.onClick.AddListener(AddItem);

        Shop.OnRefundAll += ResetItem;

        LoadSavedLevel();

        UpdateDots();

        // Nothing selected at game start
        SetSelected(false);
    }

    private void OnDestroy()
    {
        buyBtn.onClick.RemoveListener(AddItem);
        Shop.OnRefundAll -= ResetItem;
    }

    // =========================================================
    // LOAD SAVED LEVEL
    // =========================================================

    private void LoadSavedLevel()
    {
        if (GameSaveSystem.instance == null)
        {
            currentLevel = 0;
            return;
        }

        currentLevel = GameSaveSystem.instance.GetItemLevel(item);

        Debug.Log(
            $"Loaded {item.itemName} at level {currentLevel}"
        );
    }

    // =========================================================
    // SELECT
    // =========================================================

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;

        Shop.instance.SelectItem(this, selected);

        if (selected)
        {
            DescriptionUI.instance.ShowItem(
                item,
                currentLevel
            );
        }
    }

    // =========================================================
    // SELECTION / BUTTON
    // =========================================================

    public void SetSelected(bool state)
    {
        selected = state;

        int maxLevel = grayDots.Length;

        // =====================================================
        // MAX LEVEL
        // =====================================================

        if (currentLevel >= maxLevel)
        {
            buttonText.text = "MAX";

            // Don't show cost for maxed item
            costText.text = "";

            buyBtn.interactable = false;
            return;
        }

        // =====================================================
        // NOTHING SELECTED
        // =====================================================

        if (!selected)
        {
            buttonText.text =
                currentLevel == 0
                    ? "BUY"
                    : "UPGRADE";

            // Hide cost until this item is selected
            costText.text = "";

            buyBtn.interactable = true;
            return;
        }

        // =====================================================
        // SELECTED
        // =====================================================

        int cost = item.unlockCost * (currentLevel + 1);

        buttonText.text =
            currentLevel == 0
                ? "BUY"
                : "UPGRADE";

        costText.text =
            "$" + CurrencyFormatter.Format(cost, 2);

        buyBtn.interactable =
            Shop.instance.playerMoney >= cost;
    }

    // =========================================================
    // BUY / UPGRADE
    // =========================================================

    public void AddItem()
    {
        if (!selected)
            return;

        if (currentLevel >= grayDots.Length)
            return;

        if (currentLevel >= item.levels.Count)
        {
            Debug.LogError(
                $"{item.itemName} has {grayDots.Length} shop levels " +
                $"but only {item.levels.Count} ItemLevel entries."
            );

            return;
        }

        int cost = item.unlockCost * (currentLevel + 1);

        if (Shop.instance.playerMoney < cost)
            return;

        int purchasedLevel = currentLevel;

        ItemLevel levelData = item.levels[purchasedLevel];

        currentLevel++;

        UpdateDots();

        // =====================================================
        // SAVE PURCHASE DATA
        // =====================================================

        if (GameSaveSystem.instance != null)
        {
            GameSaveSystem.instance.SaveShopItem(
                item,
                currentLevel,
                levelData.modifierAmount,
                levelData.targetStat,
                levelData.isPercentage
            );
        }

        // =====================================================
        // TELL SHOP ABOUT PURCHASE
        // =====================================================

        OnItemAdded?.Invoke(
            item,
            purchasedLevel,
            cost
        );

        SetSelected(true);

        DescriptionUI.instance.ShowItem(
            item,
            currentLevel
        );
    }

    // =========================================================
    // UPDATE LEVEL DOTS
    // =========================================================

    private void UpdateDots()
    {
        for (int i = 0; i < grayDots.Length; i++)
        {
            grayDots[i].SetActive(i >= currentLevel);
        }
    }

    // =========================================================
    // REFUND RESET
    // =========================================================

    private void ResetItem()
    {
        currentLevel = 0;
        selected = false;

        UpdateDots();

        buttonText.text = "BUY";

        // Nothing selected after refund
        costText.text = "";

        buyBtn.interactable = true;

        if (DescriptionUI.instance != null)
        {
            DescriptionUI.instance.ClearDescription();
        }
    }
}