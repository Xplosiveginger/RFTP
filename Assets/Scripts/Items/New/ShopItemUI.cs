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

    public GameObject[] grayDots;   // Only gray dots needed

    private bool selected;
    private int currentLevel = 0;

    public static event Action<ShopItemSO> OnItemAdded;

    void Start()
    {
        buyBtn.onClick.AddListener(AddItem);

        Shop.OnRefundAll += ResetItem;

        UpdateDots();
        SetSelected(false);
    }

    private void OnDestroy()
    {
        Shop.OnRefundAll -= ResetItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = !selected;
        Shop.instance.SelectItem(this, selected);
    }

    public void SetSelected(bool state)
    {
        selected = state;

        int maxLevel = item.levels.Count;

        if (currentLevel >= maxLevel)
        {
            buttonText.text = "MAX";
            buyBtn.interactable = false;
            return;
        }

        int cost = item.unlockCost * (currentLevel + 1);

        if (selected)
        {
            buttonText.text = currentLevel == 0 ? "BUY $" + cost : "UPGRADE $" + cost;
            buyBtn.interactable = Shop.instance.playerMoney >= cost;
        }
        else
        {
            buttonText.text = currentLevel == 0 ? "BUY" : "UPGRADE";
            buyBtn.interactable = true;
        }
    }

    public void AddItem()
    {
        if (!selected)
            return;

        if (currentLevel >= item.levels.Count)
            return;

        int cost = item.unlockCost * (currentLevel + 1);

        if (Shop.instance.playerMoney < cost)
            return;

        currentLevel++;

        UpdateDots();

        OnItemAdded?.Invoke(item);

        SetSelected(true);
    }

    void UpdateDots()
    {
        for (int i = 0; i < grayDots.Length; i++)
        {
            grayDots[i].SetActive(i >= currentLevel);
        }
    }

    private void ResetItem()
    {
        currentLevel = 0;
        selected = false;

        foreach (GameObject dot in grayDots)
        {
            dot.SetActive(true);
        }

        buttonText.text = "BUY";
        buyBtn.interactable = true;
    }
}