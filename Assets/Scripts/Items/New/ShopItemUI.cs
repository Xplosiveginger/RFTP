using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    public Button buyBtn;
    public ShopItemSO item;

    private bool selected;

    public static event Action<ShopItemSO> OnItemAdded;
   
    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;
        Shop.instance.SelectItem(this, selected);
    }

    public void AddItem()
    {
        OnItemAdded?.Invoke(item);
    }
}