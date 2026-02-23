using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    private ShopItemUI currentSelected;

    [Header("Animation")]
    public float btnScaleTime = 0.25f;

    [Header("Owned Items")]
    public List<ShopItemSO> startingItems = new List<ShopItemSO>();

    public static event Action<ItemSO> OnItemSelected;
    public static event Action<ItemSO> OnItemUnSelected;
    public static event Action OnItemUnlocked;

    // ===============================
    // Singleton
    // ===============================
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
    // Selection Logic
    // ===============================
    public void SelectItem(ShopItemUI itemUI, bool selected)
    {
        // Unselect previous item
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
    // Animations (DOTween)
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
    // Buying / Unlock
    // ===============================
    public void AddStartingItem(ShopItemSO item)
    {
        if (startingItems.Contains(item)) return;

        startingItems.Add(item);

        UnlockItem(item);
    }

    public void UnlockItem(ShopItemSO item)
    {
        OnItemUnlocked?.Invoke();
    }
}