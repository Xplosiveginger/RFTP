using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    private ShopItemUI currentSelected;

    [Header("Owned Items")]
    public List<ShopItemSO> startingItems = new List<ShopItemSO>();

    [Header("Money")]
    public int startingMoney = 1000;
    public int playerMoney;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI refundText;

    [Header("Shop UI")]
    public GameObject buyButton;
    public GameObject refundButton;
    public TextMeshProUGUI buyCostText;

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

    private void Start()
    {
        LoadSavedMoney();
        UpdateMoneyUI();
        UpdateRefundUI();

        // Nothing selected at start
        UpdateSelectionUI();
    }

    // ===============================
    // MONEY
    // ===============================

    private void OnSavedMoneyChanged(int newMoney)
    {
        playerMoney = newMoney;

        UpdateMoneyUI();
        RefreshSelectedItem();
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

    private void RefreshSelectedItem()
    {
        if (currentSelected != null)
        {
            currentSelected.SetSelected(true);
        }
    }

    // ===============================
    // SELECTION
    // ===============================

    public void SelectItem(ShopItemUI itemUI, bool selected)
    {
        // Unselect previous item
        if (currentSelected != null && currentSelected != itemUI)
        {
            currentSelected.SetSelected(false);

            OnItemUnSelected?.Invoke(currentSelected.item);
        }

        currentSelected = selected ? itemUI : null;

        // Nothing selected
        if (currentSelected == null)
        {
            UpdateSelectionUI();
            return;
        }

        currentSelected.SetSelected(true);

        if (selected)
        {
            OnItemSelected?.Invoke(currentSelected.item);
        }

        UpdateSelectionUI();
    }

    // ===============================
    // SHOP UI
    // ===============================
    public void UpdateSelectionUI()
    {
        bool hasSelection = currentSelected != null;

        // ---------------------------------
        // BUY BUTTON
        // ---------------------------------

        if (buyButton != null)
            buyButton.SetActive(hasSelection);

        // ---------------------------------
        // REFUND BUTTON
        // ---------------------------------

        if (refundButton != null)
            refundButton.SetActive(hasSelection);

        // ---------------------------------
        // BUY COST
        // ---------------------------------

        if (buyCostText != null)
        {
            if (hasSelection)
            {
                buyCostText.gameObject.SetActive(true);

                int cost =
                    currentSelected.item.unlockCost *
                    (currentSelected.CurrentLevel + 1);

                buyCostText.text =
                    "$" + CurrencyFormatter.Format(cost, 2);
            }
            else
            {
                buyCostText.gameObject.SetActive(false);
                buyCostText.text = "";
            }
        }

        // ---------------------------------
        // REFUND AMOUNT
        // ---------------------------------

        if (refundText != null)
        {
            if (hasSelection)
            {
                refundText.gameObject.SetActive(true);

                // IMPORTANT:
                // Restore the refund amount after re-selecting
                UpdateRefundUI();
            }
            else
            {
                refundText.gameObject.SetActive(false);
                refundText.text = "";
            }
        }

        // ---------------------------------
        // DESCRIPTION
        // ---------------------------------

        if (DescriptionUI.instance != null)
        {
            if (hasSelection)
            {
                DescriptionUI.instance.ShowItem(
                    currentSelected.item,
                    currentSelected.CurrentLevel
                );
            }
            else
            {
                DescriptionUI.instance.ClearDescription();
            }
        }
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

        playerMoney -= cost;
        spentMoney += cost;

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

        playerMoney += refundAmount;

        GameSaveSystem.instance.ClearShopData();

        GameSaveSystem.instance.SetMoney(playerMoney);

        startingItems.Clear();
        spentMoney = 0;

        UpdateMoneyUI();
        UpdateRefundUI();

        // Clear selection and hide all selection UI
        currentSelected = null;
        UpdateSelectionUI();

        // Reset all ShopItemUI elements
        OnRefundAll?.Invoke();

        Debug.Log(
            $"All purchases refunded! Refunded: ${refundAmount}"
        );
    }

    // ===============================
    // REFUND UI
    // ===============================

    private void UpdateRefundUI()
    {
        int refundAmount = 0;

        if (GameSaveSystem.instance != null)
        {
            foreach (GameSaveSystem.SavedShopItem savedItem
                     in GameSaveSystem.instance.GetSavedShopItems())
            {
                ShopItemSO shopItem =
                    FindShopItemByID(savedItem.itemID);

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

    private ShopItemSO FindShopItemByID(string itemID)
    {
        ShopItemUI[] shopItems =
            FindObjectsOfType<ShopItemUI>();

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