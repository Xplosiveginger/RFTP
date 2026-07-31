using UnityEngine;
using UnityEngine.UI;

public class IDCardSkillItem : MonoBehaviour
{
    [Header("UI")]
    public Image skillIcon;

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

    private const int MaxSkillLevel = 8;

    public void Setup(GameStat_SO.SkillData skillData)
    {
        skillIcon.sprite = skillData.image;

        // Temporary until skill levels are added
        int currentLevel = Random.Range(1, MaxSkillLevel + 1);

        PopulateLevels(currentLevel);

        borderImage.sprite = currentLevel >= MaxSkillLevel
            ? maxLevelBorderSprite
            : defaultBorderSprite;
    }

    private void PopulateLevels(int currentLevel)
    {
        foreach (Transform child in levelIconContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < MaxSkillLevel; i++)
        {
            GameObject icon = Instantiate(levelIconPrefab, levelIconContainer);

            LevelIconUI levelIcon = icon.GetComponent<LevelIconUI>();

            if (levelIcon != null)
            {
                levelIcon.background.color =
                    i < currentLevel ? unlockedColor : lockedColor;
            }
        }
    }
}