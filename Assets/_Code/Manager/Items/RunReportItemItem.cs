using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunReportItemItem : MonoBehaviour
{
    public Image itemImage;
    public Image itemBorder;

    public Sprite normalBorder;
    public Sprite maxLevelBorder;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;

    public void Setup(GameStat_SO.ItemData itemData)
    {
        if (itemData.cardDataSO == null)
            return;

        CardDataSO cardData = itemData.cardDataSO;

        // Image
        itemImage.sprite = cardData.cardSprite;

        // Name
        itemNameText.text = cardData.cardName;

        // Level
        itemLevelText.text = "Lv. " + itemData.level;

        // Border
        int maxLevel = cardData.levelImages.Count;

        if (itemData.level >= maxLevel)
            itemBorder.sprite = maxLevelBorder;
        else
            itemBorder.sprite = normalBorder;
    }
}