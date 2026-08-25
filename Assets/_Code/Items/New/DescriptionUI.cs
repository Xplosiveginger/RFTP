using TMPro;
using UnityEngine;

public class DescriptionUI : MonoBehaviour
{
    public static DescriptionUI instance;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    void Awake()
    {
        instance = this;
    }

    public void ShowItem(ShopItemSO item, int level)
    {
        if (level >= item.levels.Count)
        {
            nameText.text = item.itemName + " - Level " + item.levels.Count;
            descriptionText.text = "MAX LEVEL REACHED";
            return;
        }

        var levelData = item.levels[level];

        nameText.text = item.itemName + " - Level " + (level + 1);
        descriptionText.text = levelData.description;
    }

    public void ClearDescription()
    {
        nameText.text = "Level";
        descriptionText.text = "Description";
    }
}