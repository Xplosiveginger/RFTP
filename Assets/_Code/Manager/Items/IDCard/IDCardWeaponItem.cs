using UnityEngine;
using UnityEngine.UI;

public class IDCardWeaponItem : MonoBehaviour
{
    [Header("UI")]
    public Image weaponIcon;

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

    private const int MaxWeaponLevel = 8;

    public void Setup(GameStat_SO.WeaponData weaponData)
    {
        weaponIcon.sprite = weaponData.weaponDataSO.weaponLogo;

        // Get actual weapon level
        int currentLevel = 1;

        if (weaponData.statManager != null)
        {
            WeaponBase weaponBase =
                weaponData.statManager.gameObject.GetComponent<WeaponBase>();

            if (weaponBase != null)
            {
                currentLevel = weaponBase.GetLevel;
            }
        }

        // Maximum weapon level is always 8
        int maxLevel = MaxWeaponLevel;

        if (showLevelIcons)
        {
            PopulateLevels(currentLevel);
        }
        else
        {
            foreach (Transform child in levelIconContainer)
                Destroy(child.gameObject);

            levelIconContainer.gameObject.SetActive(false);
        }

        // Use level 8 as max level
        borderImage.sprite = currentLevel >= maxLevel
            ? maxLevelBorderSprite
            : defaultBorderSprite;
    }

    private void PopulateLevels(int currentLevel)
    {
        levelIconContainer.gameObject.SetActive(true);

        // Clear old icons
        foreach (Transform child in levelIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Always display 8 level icons
        for (int i = 0; i < MaxWeaponLevel; i++)
        {
            GameObject icon = Instantiate(
                levelIconPrefab,
                levelIconContainer
            );

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