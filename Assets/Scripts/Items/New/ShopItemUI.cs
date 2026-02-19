using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    public Button buyBtn;
    public ShopItemSO item;

    public TextMeshProUGUI buttonText; // 👈 assign this

    private bool selected;

    public static event Action<ShopItemSO> OnItemAdded;

    void Start()
    {
        SetSelected(false); // default = BUY
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;
        Shop.instance.SelectItem(this, selected);
    }

    public void SetSelected(bool state)
    {
        selected = state;

        if (selected)
            buttonText.text = "$" + item.unlockCost; // show price
        else
            buttonText.text = "BUY"; // show buy
    }

    public void AddItem()
    {
        OnItemAdded?.Invoke(item);
    }
}

