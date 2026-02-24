using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    private ShopItemUI currentSelected;

    [Header("Animation")]
    public float btnScaleTime = 0.25f;

    [Header("Owned Items")]
    public List<ShopItemSO> startingItems = new List<ShopItemSO>();

    // ===============================
    // 💰 MONEY
    // ===============================
    [Header("Money")]
    public int startingMoney = 1000;
    public int playerMoney;

    public TextMeshProUGUI moneyText;

    // track how much spent
    private int spentMoney = 0;

    public static event Action<ItemSO> OnItemSelected;
    public static event Action<ItemSO> OnItemUnSelected;
    public static event Action OnItemUnlocked;
    public static event Action OnRefundAll;

    // ===============================
    // Singleton
    // ===============================
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        playerMoney = startingMoney;
    }

    private void Start()
    {
        UpdateMoneyUI();
    }

    // ===============================
    // Events
    // ===============================
    private void OnEnable()
    {
        ShopItemUI.OnItemAdded += AddStartingItem;
    }

    private void OnDisable()
    {
        ShopItemUI.OnItemAdded -= AddStartingItem;
    }

    // ===============================
    // Selection
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
    // Animations
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
    public void AddStartingItem(ShopItemSO item)
    {
        if (startingItems.Contains(item))
            return;

        if (playerMoney < item.unlockCost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        playerMoney -= item.unlockCost;
        spentMoney += item.unlockCost;

        startingItems.Add(item);

        UpdateMoneyUI();

        UnlockItem(item);
    }

    public void UnlockItem(ShopItemSO item)
    {
        OnItemUnlocked?.Invoke();
    }

    // ===============================
    // 💥 REFUND ALL
    // ===============================
    public void RefundAll()
    {
        if (spentMoney <= 0)
            return;

        // return money
        playerMoney += spentMoney;

        spentMoney = 0;

        // clear owned items
        startingItems.Clear();

        UpdateMoneyUI();

        // tell all UI to reset
        OnRefundAll?.Invoke();

        Debug.Log("All purchases refunded!");
    }

    // ===============================
    // UI
    // ===============================
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = "Total Money " + playerMoney;
    }
}