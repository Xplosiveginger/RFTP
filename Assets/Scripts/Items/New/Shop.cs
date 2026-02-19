using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public float btnScaleTime = 0.5f;

    public List<ShopItemSO> startingItems = new List<ShopItemSO>();

    public static event Action<ItemSO> OnItemSelected;
    public static event Action<ItemSO> OnItemUnSelected;
    public static event Action OnItemUnlocked;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        ShopItemUI.OnItemAdded += AddStartingItem;
    }

    private void OnDisable()
    {
        ShopItemUI.OnItemAdded -= AddStartingItem;
    }

    public void SelectItem(ShopItemUI itemUI, bool selected)
    {
        if (selected)
            SelectItem(itemUI);
        else
            UnSelectItem(itemUI);
    }

    public void SelectItem(ShopItemUI itemUI)
    {
        itemUI.buyBtn.GetComponent<RectTransform>().DOScaleY(1.2f, btnScaleTime);
        //item.unlocked;
        //OnItemSelected?.Invoke(item);
    }

    public void UnSelectItem(ShopItemUI itemUI)
    {
        itemUI.buyBtn.GetComponent<RectTransform>().DOScaleY(1f, btnScaleTime);
        //OnItemUnSelected?.Invoke(item);
    }

    public void AddStartingItem(ShopItemSO item)
    {
        if (startingItems.Contains(item)) return;
            
        startingItems.Add(item);
    }

    public void UnlockItem(ShopItemSO item)
    {
        OnItemUnlocked?.Invoke();
    }
}
