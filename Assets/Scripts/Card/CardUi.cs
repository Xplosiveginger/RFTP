using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelUpInfoText; // ✅ New: displays level-up info
    public Image cardImage;
    public Button selectButton;

    private WeaponSO weaponData;
    private ItemSO itemData;
    private WeaponManager weaponManager;
    private ItemManager itemManager;
    private CardSpawner cardSpawner;
    private bool isWeaponCard;
    private bool isInitialized;

    private void Awake()
    {
        if (selectButton == null)
            selectButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnCardClicked);
        }

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveAllListeners();
    }

    // ✅ Initialize for Weapon
    public void InitializeWeapon(WeaponSO weapon, WeaponManager manager, CardSpawner spawner)
    {
        weaponData = weapon;
        weaponManager = manager;
        cardSpawner = spawner;
        isWeaponCard = true;
        isInitialized = true;

        titleText.text = weapon.weaponName;

        // Determine correct level
        int currentLevel = weaponManager.HasWeapon(weapon)
            ? weaponManager.GetWeaponLevel(weapon) + 1
            : 0;

        currentLevel = Mathf.Clamp(currentLevel, 0, weapon.levels.Count - 1);

        // ✅ Apply card sprite
        if (cardImage != null && weapon.levels[currentLevel].cardSprite != null)
            cardImage.sprite = weapon.levels[currentLevel].cardSprite;

        // ✅ Apply level-up info text
        if (levelUpInfoText != null)
            levelUpInfoText.text = weapon.levels[currentLevel].levelUpInfo;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCardClicked);
    }

    // ✅ Initialize for Item
    public void InitializeItem(ItemSO item, ItemManager manager, CardSpawner spawner)
    {
        itemData = item;
        itemManager = manager;
        cardSpawner = spawner;
        isWeaponCard = false;
        isInitialized = true;

        titleText.text = item.itemName;

        int levelIndex = 0;
        if (cardImage != null && item.levels.Count > 0 && item.levels[levelIndex].cardSprite != null)
            cardImage.sprite = item.levels[levelIndex].cardSprite;

        // ✅ Apply level-up info text
        if (levelUpInfoText != null && item.levels.Count > 0)
            levelUpInfoText.text = item.levels[levelIndex].levelUpInfo;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        if (isWeaponCard)
        {
            if (weaponManager.HasWeapon(weaponData))
                weaponManager.UpgradeWeapon(weaponData);
            else
                weaponManager.AddWeaponFromSO(weaponData, 0);
        }
        else
        {
            itemManager?.AddItem(itemData);
        }

        cardSpawner.OnCardSelected();
    }
}
