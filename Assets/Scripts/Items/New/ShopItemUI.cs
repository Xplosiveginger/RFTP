using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Button buyBtn;
    public TextMeshProUGUI buttonText;
    public ShopItemSO item;

    private bool selected;

    public static event Action<ShopItemSO> OnItemAdded;

    void Start()
    {
        // ✅ connect BUY button
        buyBtn.onClick.AddListener(AddItem);

        // default state
        SetSelected(false);
    }

    // ===============================
    // Item card clicked (select item)
    // ===============================
    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;

        // tell shop manager
        Shop.instance.SelectItem(this, selected);
    }

    // ===============================
    // Visual selection state
    // ===============================
    public void SetSelected(bool state)
    {
        selected = state;

        if (selected)
            buttonText.text = "BUY $" + item.unlockCost;
        else
            buttonText.text = "BUY";
    }

    // ===============================
    // Buy button pressed
    // ===============================
    public void AddItem()
    {
        // safety: only buy if selected
        if (!selected) return;

        OnItemAdded?.Invoke(item);
    }
}