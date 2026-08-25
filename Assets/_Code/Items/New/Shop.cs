using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    private ShopItemUI currentSelected;

    [Header("Owned Items")]
    public List<ShopItemSO> startingItems = new List<ShopItemSO>();

    [Header("Animation")]
    public float btnScaleTime = 0.25f;

    [Header("Money")]
    public int startingMoney = 1000;
    public int playerMoney;

    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI refundText;
    // Track how much was spent
    private int spentMoney = 0;

    public static event Action<ItemSO> OnItemSelected;
    public static event Action<ItemSO> OnItemUnSelected;
    public static event Action OnItemUnlocked;
    public static event Action OnRefundAll;

    // ===============================
    // SINGLETON
    // ===============================

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnEnable()
    {
        ShopItemUI.OnItemAdded += AddStartingItem;
        GameSaveSystem.OnMoneyChanged += OnSavedMoneyChanged;
    }
    private void OnDisable()
    {
        ShopItemUI.OnItemAdded -= AddStartingItem;
        GameSaveSystem.OnMoneyChanged -= OnSavedMoneyChanged;
    }
    private void OnSavedMoneyChanged(int newMoney)
    {
        playerMoney = newMoney;

        UpdateMoneyUI();

        RefreshSelectedItem();
    }
    private void RefreshSelectedItem()
    {
        if (currentSelected != null)
        {
            currentSelected.SetSelected(true);
        }
    }
    private void Start()
    {
        LoadSavedMoney();
        UpdateMoneyUI();
        UpdateRefundUI();
    }
    private void LoadSavedMoney()
    {
        if (GameSaveSystem.instance != null)
        {
            playerMoney = GameSaveSystem.instance.GetMoney();

            Debug.Log(
                $"Shop loaded saved money: {playerMoney}"
            );
        }
        else
        {
            playerMoney = startingMoney;

            Debug.Log(
                $"No GameSaveSystem found. Using starting money: {playerMoney}"
            );
        }
    }
    private void UpdateRefundUI()
    {
        int refundAmount = 0;

        if (GameSaveSystem.instance != null)
        {
            foreach (GameSaveSystem.SavedShopItem savedItem
                     in GameSaveSystem.instance.GetSavedShopItems())
            {
                ShopItemSO shopItem = FindShopItemByID(savedItem.itemID);

                if (shopItem == null)
                    continue;

                for (int level = 0; level < savedItem.level; level++)
                {
                    refundAmount +=
                        shopItem.unlockCost * (level + 1);
                }
            }
        }

        if (refundText != null)
        {
            refundText.text =
                refundAmount > 0
                    ? "$" + CurrencyFormatter.Format(refundAmount, 2)
                    : "";
        }
    }
    // ===============================
    // EVENTS
    // ===============================
    
    // ===============================
    // SELECTION
    // ===============================

    public void SelectItem(ShopItemUI itemUI, bool selected)
    {
        if (currentSelected != null && currentSelected != itemUI)
        {
            currentSelected.SetSelected(false);

            AnimateUnselect(currentSelected);

            OnItemUnSelected?.Invoke(currentSelected.item);
        }

        currentSelected = selected ? itemUI : null;

        itemUI.SetSelected(selected);

        if (selected)
        {
            AnimateSelect(itemUI);

            OnItemSelected?.Invoke(itemUI.item);
        }
        else
        {
            AnimateUnselect(itemUI);
        }
    }

    // ===============================
    // ANIMATIONS
    // ===============================

    private void AnimateSelect(ShopItemUI itemUI)
    {
        itemUI.buyBtn.transform
            .DOScale(1.2f, btnScaleTime)
            .SetEase(Ease.OutBack);
    }

    private void AnimateUnselect(ShopItemUI itemUI)
    {
        itemUI.buyBtn.transform
            .DOScale(1f, btnScaleTime);
    }

    // ===============================
    // BUY
    // ===============================
    public void AddStartingItem(
        ShopItemSO item,
        int level,
        int cost)
    {
        if (playerMoney < cost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        // Subtract purchase cost ONCE
        playerMoney -= cost;
        spentMoney += cost;

        // Save remaining money
        if (GameSaveSystem.instance != null)
        {
            GameSaveSystem.instance.SetMoney(playerMoney);
        }

        UpdateMoneyUI();
        UpdateRefundUI();

        if (!startingItems.Contains(item))
        {
            startingItems.Add(item);

            UnlockItem(item);
        }

        Debug.Log(
            $"Purchased {item.name} at level {level} for ${cost}. " +
            $"Remaining money: ${playerMoney}"
        );
    }

    public void UnlockItem(ShopItemSO item)
    {
        OnItemUnlocked?.Invoke();
    }

    // ===============================
    // REFUND ALL
    // ===============================

    public void RefundAll()
    {
        if (GameSaveSystem.instance == null)
        {
            Debug.LogWarning("GameSaveSystem not found.");
            return;
        }

        int refundAmount = 0;

        // Calculate total money spent from saved shop levels
        foreach (GameSaveSystem.SavedShopItem savedItem
                 in GameSaveSystem.instance.GetSavedShopItems())
        {
            ShopItemSO shopItem = FindShopItemByID(savedItem.itemID);

            if (shopItem == null)
            {
                Debug.LogWarning(
                    $"Could not find ShopItemSO for saved item: {savedItem.itemID}"
                );

                continue;
            }

            for (int level = 0; level < savedItem.level; level++)
            {
                int cost = shopItem.unlockCost * (level + 1);
                refundAmount += cost;
            }
        }

        if (refundAmount <= 0)
        {
            Debug.Log("Nothing to refund.");
            return;
        }

        // Refund money
        playerMoney += refundAmount;

        // Reset all saved shop data
        GameSaveSystem.instance.ClearShopData();

        // Save refunded money
        GameSaveSystem.instance.SetMoney(playerMoney);

        // Reset runtime shop data
        startingItems.Clear();
        spentMoney = 0;

        UpdateMoneyUI();
        UpdateRefundUI();
        // Tell all ShopItemUI elements to reset
        OnRefundAll?.Invoke();

        Debug.Log(
            $"All purchases refunded! Refunded: ${refundAmount}"
        );
    }
    private ShopItemSO FindShopItemByID(string itemID)
    {
        ShopItemUI[] shopItems = FindObjectsOfType<ShopItemUI>();

        foreach (ShopItemUI shopItemUI in shopItems)
        {
            if (shopItemUI.item == null)
                continue;

            if (shopItemUI.item.name == itemID)
                return shopItemUI.item;
        }

        return null;
    }
    // ===============================
    // UI
    // ===============================

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = playerMoney.ToString();
    }
}