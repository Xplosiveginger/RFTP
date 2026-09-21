using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IDCardManager : MonoBehaviour
{
    public static IDCardManager instance;

    [Header("Game Data")]
    public GameStat_SO gameStat;

    [Header("Gameplay UI")]
    public GameObject idPanel;

    public Transform screenWeaponsContainer;
    public GameObject screenWeaponItemPrefab;
    public GameObject screenWeaponPlaceholderPrefab;

    public Transform screenItemsContainer;
    public GameObject screenItemPrefab;
    public GameObject screenItemPlaceholderPrefab;

    [Header("Pause Animation")]
    public Animator pauseUIAnimator;
    public float riseAnimationDuration = 0.5f;

    private Coroutine hideRoutine;

    [Header("Pause UI")]
    public GameObject pauseUIIDCard;

    public Transform pauseWeaponsContainer;
    public GameObject pauseWeaponItemPrefab;
    public GameObject pauseWeaponPlaceholderPrefab;

    public Transform pauseItemsContainer;
    public GameObject pauseItemPrefab;
    public GameObject pauseItemPlaceholderPrefab;

    [Header("Player Portrait")]
    public Image playerImage;

    public Sprite health100Sprite;
    public Sprite health75Sprite;
    public Sprite health50Sprite;
    public Sprite health25Sprite;
    public Sprite health5Sprite;
    public Sprite health0Sprite;


    // =========================================================
    // UI SLOT REFERENCES
    // =========================================================

    private const int MAX_WEAPON_SLOTS = 4;
    private const int MAX_ITEM_SLOTS = 4;

    private List<GameObject> screenWeaponSlots =
        new List<GameObject>();

    private List<GameObject> screenItemSlots =
        new List<GameObject>();

    private List<GameObject> pauseWeaponSlots =
        new List<GameObject>();

    private List<GameObject> pauseItemSlots =
        new List<GameObject>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        // Spawn all placeholder slots once.
        InitializeSlots();

        RefreshAll();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            RefreshAll();
        }
    }


    // =========================================================
    // INITIALIZE SLOTS
    // =========================================================

    private void InitializeSlots()
    {
        CreatePlaceholderSlots(
            screenWeaponsContainer,
            screenWeaponPlaceholderPrefab,
            screenWeaponSlots,
            MAX_WEAPON_SLOTS
        );

        CreatePlaceholderSlots(
            screenItemsContainer,
            screenItemPlaceholderPrefab,
            screenItemSlots,
            MAX_ITEM_SLOTS
        );

        CreatePlaceholderSlots(
            pauseWeaponsContainer,
            pauseWeaponPlaceholderPrefab,
            pauseWeaponSlots,
            MAX_WEAPON_SLOTS
        );

        CreatePlaceholderSlots(
            pauseItemsContainer,
            pauseItemPlaceholderPrefab,
            pauseItemSlots,
            MAX_ITEM_SLOTS
        );
    }


    private void CreatePlaceholderSlots(
        Transform container,
        GameObject placeholderPrefab,
        List<GameObject> slotList,
        int slotCount)
    {
        if (container == null || placeholderPrefab == null)
            return;

        slotList.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject placeholder = Instantiate(
                placeholderPrefab,
                container
            );

            placeholder.transform.SetSiblingIndex(i);

            slotList.Add(placeholder);
        }
    }


    // =========================================================
    // PLAYER PORTRAIT
    // =========================================================

    public void UpdatePlayerPortrait()
    {
        if (HealthSystem.Instance == null)
            return;

        float maxHealth = HealthSystem.Instance.MaxHealth;
        float currentHealth = HealthSystem.Instance.CurrentHealth;

        if (maxHealth <= 0f)
        {
            playerImage.sprite = health0Sprite;
            return;
        }

        currentHealth = Mathf.Clamp(
            currentHealth,
            0f,
            maxHealth
        );

        float healthPercent =
            (currentHealth / maxHealth) * 100f;

        if (healthPercent >= 87.5f)
        {
            playerImage.sprite = health100Sprite;
        }
        else if (healthPercent >= 62.5f)
        {
            playerImage.sprite = health75Sprite;
        }
        else if (healthPercent >= 37.5f)
        {
            playerImage.sprite = health50Sprite;
        }
        else if (healthPercent >= 15f)
        {
            playerImage.sprite = health25Sprite;
        }
        else if (healthPercent > 0f)
        {
            playerImage.sprite = health5Sprite;
        }
        else
        {
            playerImage.sprite = health0Sprite;
        }
    }


    // =========================================================
    // REFRESH
    // =========================================================

    public void RefreshAll()
    {
        if (gameStat == null)
        {
            Debug.LogError(
                "IDCardManager: GameStat_SO is not assigned."
            );

            return;
        }

        UpdatePlayerPortrait();

        // Gameplay UI
        PopulateWeapons(
            screenWeaponsContainer,
            screenWeaponItemPrefab,
            screenWeaponPlaceholderPrefab,
            screenWeaponSlots
        );

        PopulateItems(
            screenItemsContainer,
            screenItemPrefab,
            screenItemPlaceholderPrefab,
            screenItemSlots
        );

        // Pause UI
        PopulateWeapons(
            pauseWeaponsContainer,
            pauseWeaponItemPrefab,
            pauseWeaponPlaceholderPrefab,
            pauseWeaponSlots
        );

        PopulateItems(
            pauseItemsContainer,
            pauseItemPrefab,
            pauseItemPlaceholderPrefab,
            pauseItemSlots
        );
    }


    // =========================================================
    // PAUSE UI
    // =========================================================

    public void ShowPauseUI()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        pauseUIAnimator.StopPlayback();
        pauseUIAnimator.Rebind();
        pauseUIAnimator.Update(0f);

        idPanel.SetActive(false);
        pauseUIIDCard.SetActive(true);

        RefreshAll();

        pauseUIAnimator.Play("Drop", 0, 0f);
    }


    public void ShowGameplayUI()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        pauseUIAnimator.StopPlayback();

        hideRoutine = StartCoroutine(
            HidePauseCardRoutine()
        );
    }


    private IEnumerator HidePauseCardRoutine()
    {
        pauseUIAnimator.Play("Rise", 0, 0f);

        yield return new WaitForSecondsRealtime(
            riseAnimationDuration
        );

        pauseUIAnimator.Rebind();
        pauseUIAnimator.Update(0f);

        pauseUIIDCard.SetActive(false);
        idPanel.SetActive(true);

        RefreshAll();

        hideRoutine = null;
    }


    // =========================================================
    // WEAPONS
    // =========================================================

    private void PopulateWeapons(
        Transform container,
        GameObject prefab,
        GameObject placeholderPrefab,
        List<GameObject> slots)
    {
        if (container == null ||
            prefab == null ||
            placeholderPrefab == null)
            return;

        var weapons = gameStat.GetAllActiveWeapons();

        int weaponCount = Mathf.Min(
            weapons.Count,
            MAX_WEAPON_SLOTS
        );

        for (int i = 0; i < MAX_WEAPON_SLOTS; i++)
        {
            // -------------------------------------------------
            // SLOT HAS A WEAPON
            // -------------------------------------------------

            if (i < weaponCount)
            {
                GameStat_SO.WeaponData weapon = weapons[i];

                // If this slot is still a placeholder,
                // replace it with the real weapon UI.
                if (slots[i] == null ||
                    slots[i].GetComponent<IDCardWeaponItem>() == null)
                {
                    if (slots[i] != null)
                    {
                        Destroy(slots[i]);
                    }

                    GameObject item = Instantiate(
                        prefab,
                        container
                    );

                    item.transform.SetSiblingIndex(i);

                    slots[i] = item;

                    IDCardWeaponItem weaponItem =
                        item.GetComponent<IDCardWeaponItem>();

                    if (weaponItem != null)
                    {
                        weaponItem.Setup(weapon);
                    }
                }
                else
                {
                    // Already a weapon slot.
                    IDCardWeaponItem weaponItem =
                        slots[i].GetComponent<IDCardWeaponItem>();

                    if (weaponItem != null)
                    {
                        weaponItem.Setup(weapon);
                    }
                }
            }

            // -------------------------------------------------
            // SLOT IS EMPTY
            // -------------------------------------------------

            else
            {
                // If somehow a real weapon is in this slot,
                // replace it with the placeholder.
                if (slots[i] != null &&
                    slots[i].GetComponent<IDCardWeaponItem>() != null)
                {
                    Destroy(slots[i]);

                    GameObject placeholder = Instantiate(
                        placeholderPrefab,
                        container
                    );

                    placeholder.transform.SetSiblingIndex(i);

                    slots[i] = placeholder;
                }
            }
        }
    }


    // =========================================================
    // ITEMS
    // =========================================================

    private void PopulateItems(
        Transform container,
        GameObject prefab,
        GameObject placeholderPrefab,
        List<GameObject> slots)
    {
        if (container == null ||
            prefab == null ||
            placeholderPrefab == null)
            return;

        if (gameStat.items == null)
        {
            Debug.LogWarning(
                "GameStat_SO items list is null."
            );

            return;
        }

        int itemIndex = 0;

        foreach (GameStat_SO.ItemData itemData in gameStat.items)
        {
            if (itemData.cardDataSO == null)
                continue;

            // We only have 4 item slots.
            if (itemIndex >= MAX_ITEM_SLOTS)
                break;

            // -------------------------------------------------
            // REPLACE PLACEHOLDER AT THIS INDEX
            // -------------------------------------------------

            if (slots[itemIndex] == null ||
                slots[itemIndex].GetComponent<IDCardItem>() == null)
            {
                if (slots[itemIndex] != null)
                {
                    Destroy(slots[itemIndex]);
                }

                GameObject item = Instantiate(
                    prefab,
                    container
                );

                item.transform.SetSiblingIndex(itemIndex);

                slots[itemIndex] = item;

                IDCardItem itemUI =
                    item.GetComponent<IDCardItem>();

                if (itemUI != null)
                {
                    itemUI.Setup(itemData);
                }
            }
            else
            {
                // Already an item in this slot.
                IDCardItem itemUI =
                    slots[itemIndex].GetComponent<IDCardItem>();

                if (itemUI != null)
                {
                    itemUI.Setup(itemData);
                }
            }

            itemIndex++;
        }

        // -----------------------------------------------------
        // RESTORE PLACEHOLDERS FOR EMPTY SLOTS
        // -----------------------------------------------------

        for (int i = itemIndex; i < MAX_ITEM_SLOTS; i++)
        {
            if (slots[i] != null &&
                slots[i].GetComponent<IDCardItem>() != null)
            {
                Destroy(slots[i]);

                GameObject placeholder = Instantiate(
                    placeholderPrefab,
                    container
                );

                placeholder.transform.SetSiblingIndex(i);

                slots[i] = placeholder;
            }
        }
    }
}