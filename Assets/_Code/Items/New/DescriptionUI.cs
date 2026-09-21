using TMPro;
using UnityEngine;

public class DescriptionUI : MonoBehaviour
{
    public static DescriptionUI instance;

    [Header("Description UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ClearDescription();
    }

    public void ShowItem(ShopItemSO item, int level)
    {
        ShowUI();

        if (level >= item.levels.Count)
        {
            nameText.text =
                "Level " + item.levels.Count + " - " + item.itemName;

            descriptionText.text = "MAX LEVEL REACHED";
            return;
        }

        var levelData = item.levels[level];

        nameText.text =
            "Level " + (level + 1) + " - " + item.itemName;

        descriptionText.text =
            levelData.description;
    }

    public void ShowUI()
    {
        if (nameText != null)
            nameText.gameObject.SetActive(true);

        if (descriptionText != null)
            descriptionText.gameObject.SetActive(true);
    }

    public void ClearDescription()
    {
        if (nameText != null)
            nameText.gameObject.SetActive(false);

        if (descriptionText != null)
            descriptionText.gameObject.SetActive(false);
    }
}