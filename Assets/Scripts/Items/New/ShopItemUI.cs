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

    private bool selected;
    private bool owned;

    public static event Action<ShopItemSO> OnItemAdded;

    void Start()
    {
        buyBtn.onClick.AddListener(AddItem);

        Shop.OnRefundAll += ResetItem;

        SetSelected(false);
    }

    private void OnDestroy()
    {
        Shop.OnRefundAll -= ResetItem;
    }

    // ===============================
    // Select (✅ allow owned too)
    // ===============================
    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;

        Shop.instance.SelectItem(this, selected);
    }

    // ===============================
    // Visual
    // ===============================
    public void SetSelected(bool state)
    {
        selected = state;

        if (owned)
        {
            buttonText.text = "OWNED";
            buyBtn.interactable = false;
            return;
        }

        if (selected)
        {
            buttonText.text = "BUY $" + item.unlockCost;
            buyBtn.interactable = Shop.instance.playerMoney >= item.unlockCost;
        }
        else
        {
            buttonText.text = "BUY";
            buyBtn.interactable = true;
        }
    }

    // ===============================
    // Buy (❌ block only here)
    // ===============================
    public void AddItem()
    {
        if (!selected || owned)
            return;

        owned = true;

        buttonText.text = "OWNED";
        buyBtn.interactable = false;

        OnItemAdded?.Invoke(item);
    }

    // ===============================
    // Refund reset
    // ===============================
    private void ResetItem()
    {
        owned = false;
        selected = false;

        buttonText.text = "BUY";
        buyBtn.interactable = true;
    }
}