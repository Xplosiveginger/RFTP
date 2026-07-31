using UnityEngine;
using UnityEngine.UI;

public class IDCardWeaponItem : MonoBehaviour
{
    [Header("UI")]
    public Image weaponIcon;
    
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

    private const int MaxWeaponLevel = 8;
    private const int MaxSkillLevel = 8;

    public void Setup(GameStat_SO.WeaponData weaponData)
    {
        weaponIcon.sprite = weaponData.weaponDataSO.weaponLogo;

        // TODO: Replace with actual level from GameStat later
        int currentLevel = Random.Range(1, MaxSkillLevel + 1);
        int maxLevel = weaponData.weaponDataSO.weaponStatData.Count;

        PopulateLevels(currentLevel);

        borderImage.sprite = currentLevel >= maxLevel
            ? maxLevelBorderSprite
            : defaultBorderSprite;
    }
    private void PopulateLevels(int currentLevel)
    {
        // Clear old icons
        foreach (Transform child in levelIconContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < MaxWeaponLevel; i++)
        {
            GameObject icon = Instantiate(levelIconPrefab, levelIconContainer);

            LevelIconUI levelIcon = icon.GetComponent<LevelIconUI>();

            if (levelIcon != null)
            {
                levelIcon.background.color = (i < currentLevel)
                    ? unlockedColor
                    : lockedColor;
            }
        }
    }
}