using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    public Button buyBtn;
    public TextMeshProUGUI buttonText;
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

        if (currentLevel >= maxLevel)
        {
            buttonText.text = "MAX";
            buyBtn.interactable = false;
            return;
        }

        int cost = item.unlockCost * (currentLevel + 1);

        if (selected)
        {
            buttonText.text =
                currentLevel == 0
                    ? "BUY $" + cost
                    : "UPGRADE $" + cost;

            buyBtn.interactable =
                Shop.instance.playerMoney >= cost;
        }
        else
        {
            buttonText.text =
                currentLevel == 0
                    ? "BUY"
                    : "UPGRADE";

            buyBtn.interactable = true;
        }
    }

    // =========================================================
    // BUY / UPGRADE
    // =========================================================

    public void AddItem()
    {
        if (!selected)
            return;

        // Gray dots define maximum level
        if (currentLevel >= grayDots.Length)
            return;

        // Make sure the ItemSO has data for this level
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

        // Index of the level being purchased
        int purchasedLevel = currentLevel;

        // Get the actual ItemLevel from ItemSO
        ItemLevel levelData = item.levels[purchasedLevel];

        // Unlock next level
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
            // Purchased levels are hidden
            // Remaining levels stay visible
            grayDots[i].SetActive(i >= currentLevel);
        }
    }

    // =========================================================
    // REFUND
    // =========================================================

    private void ResetItem()
    {
        currentLevel = 0;
        selected = false;

        UpdateDots();

        buttonText.text = "BUY";
        buyBtn.interactable = true;

        if (DescriptionUI.instance != null)
        {
            DescriptionUI.instance.ClearDescription();
        }
    }
}