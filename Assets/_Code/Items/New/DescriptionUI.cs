using TMPro;
using Unity.VisualScripting;
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
            nameText.text = item.name;
            descriptionText.text = "MAX LEVEL REACHED";
            return;
        }

        var levelData = item.levels[level];

       // nameText.text = item.name + " Lv." + (level + 1);
        nameText.text = levelData.levelName;
        descriptionText.text = levelData.description;
    }
    public void ClearDescription()
    {
        nameText.text = "Level";
        descriptionText.text = "Description";
    }
}