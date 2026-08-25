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
            nameText.text = "Level - " + item.levels.Count + " " + item.itemName;
            descriptionText.text = "MAX LEVEL REACHED";
            return;
        }

        var levelData = item.levels[level];

        nameText.text =  "Level - " + (level + 1) + " " + item.itemName;
        descriptionText.text = levelData.description;
    }

    public void ClearDescription()
    {
        nameText.text = "Level";
        descriptionText.text = "Description";
    }
}