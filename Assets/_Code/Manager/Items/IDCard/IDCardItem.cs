using UnityEngine;
using UnityEngine.UI;

public class IDCardItem : MonoBehaviour
{
    [Header("UI")]
    public Image itemIcon;

    [Header("Display")]
    public bool showLevelIcons = true;

    [Header("Border")]
    public Image borderImage;
    public Sprite defaultBorderSprite;
    public Sprite maxLevelBorderSprite;

    [Header("Level")]
    public Transform levelIconContainer;
    public GameObject levelIconPrefab;

    [Header("Colors")]
    public Color unlockedColor = Color.yellow;
    public Color lockedColor = new Color(0.55f, 0.3f, 0.75f);


    public void Setup(GameStat_SO.ItemData itemData)
    {
        if (itemData.cardDataSO == null)
        {
            Debug.LogWarning(
                "IDCardItem received ItemData with no CardDataSO."
            );

            return;
        }

        // =====================================================
        // ICON
        // =====================================================

        itemIcon.sprite = itemData.cardDataSO.cardSprite;


        // =====================================================
        // LEVEL
        // =====================================================

        int currentLevel = itemData.level;

        // Number of levels available for this card
        int maxLevel = 1;

        if (itemData.cardDataSO.levelImages != null &&
            itemData.cardDataSO.levelImages.Count > 0)
        {
            maxLevel = itemData.cardDataSO.levelImages.Count;
        }

        currentLevel = Mathf.Clamp(
            currentLevel,
            1,
            maxLevel
        );


        // =====================================================
        // LEVEL ICONS
        // =====================================================

        if (showLevelIcons)
        {
            PopulateLevels(
                currentLevel,
                maxLevel
            );

            levelIconContainer.gameObject.SetActive(true);
        }
        else
        {
            ClearLevelIcons();

            levelIconContainer.gameObject.SetActive(false);
        }


        // =====================================================
        // BORDER
        // =====================================================

        if (borderImage != null)
        {
            borderImage.sprite =
                currentLevel >= maxLevel
                    ? maxLevelBorderSprite
                    : defaultBorderSprite;
        }
    }


    private void PopulateLevels(
        int currentLevel,
        int maxLevel)
    {
        ClearLevelIcons();

        for (int i = 0; i < maxLevel; i++)
        {
            GameObject icon = Instantiate(
                levelIconPrefab,
                levelIconContainer
            );

            LevelIconUI levelIcon =
                icon.GetComponent<LevelIconUI>();

            if (levelIcon != null)
            {
                levelIcon.background.color =
                    i < currentLevel
                        ? unlockedColor
                        : lockedColor;
            }
        }
    }


    private void ClearLevelIcons()
    {
        if (levelIconContainer == null)
            return;

        foreach (Transform child in levelIconContainer)
        {
            Destroy(child.gameObject);
        }
    }
}