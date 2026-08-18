using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IDCardManager : MonoBehaviour
{
    public static IDCardManager instance;

    [Header("Game Data")]
    public GameStat_SO gameStat;

    [Header("Gameplay UI")]
    public GameObject idPanel;

    public Transform screenWeaponsContainer;
    public GameObject screenWeaponItemPrefab;

    public Transform screenItemsContainer;
    public GameObject screenItemPrefab;

    [Header("Pause Animation")]
    public Animator pauseUIAnimator;
    public float riseAnimationDuration = 0.5f;

    private Coroutine hideRoutine;

    [Header("Pause UI")]
    public GameObject pauseUIIDCard;

    public Transform pauseWeaponsContainer;
    public GameObject pauseWeaponItemPrefab;

    public Transform pauseItemsContainer;
    public GameObject pauseItemPrefab;

    [Header("Player Portrait")]
    public Image playerImage;

    public Sprite health100Sprite;
    public Sprite health75Sprite;
    public Sprite health50Sprite;
    public Sprite health25Sprite;
    public Sprite health5Sprite;
    public Sprite health0Sprite;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
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
            Debug.LogError("IDCardManager: GameStat_SO is not assigned.");
            return;
        }

        UpdatePlayerPortrait();

        // Gameplay UI
        PopulateWeapons(
            screenWeaponsContainer,
            screenWeaponItemPrefab
        );

        PopulateItems(
            screenItemsContainer,
            screenItemPrefab
        );

        // Pause UI
        PopulateWeapons(
            pauseWeaponsContainer,
            pauseWeaponItemPrefab
        );

        PopulateItems(
            pauseItemsContainer,
            pauseItemPrefab
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

        // Make sure the information is current
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

        // Refresh gameplay card after returning
        RefreshAll();

        hideRoutine = null;
    }


    // =========================================================
    // WEAPONS
    // =========================================================

    private void PopulateWeapons(
        Transform container,
        GameObject prefab)
    {
        if (container == null || prefab == null)
            return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        var weapons = gameStat.GetAllActiveWeapons();

        foreach (var weapon in weapons)
        {
            GameObject item = Instantiate(
                prefab,
                container
            );

            IDCardWeaponItem weaponItem =
                item.GetComponent<IDCardWeaponItem>();

            if (weaponItem != null)
            {
                weaponItem.Setup(weapon);
            }
        }
    }


    // =========================================================
    // ITEMS
    // =========================================================

    private void PopulateItems(
        Transform container,
        GameObject prefab)
    {
        if (container == null || prefab == null)
            return;

        // Remove old UI entries
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (gameStat.items == null)
        {
            Debug.LogWarning(
                "GameStat_SO items list is null."
            );

            return;
        }

        Debug.Log(
            $"Populating {gameStat.items.Count} items."
        );

        foreach (GameStat_SO.ItemData itemData in gameStat.items)
        {
            if (itemData.cardDataSO == null)
                continue;

            GameObject item = Instantiate(
                prefab,
                container
            );

            IDCardItem itemUI =
                item.GetComponent<IDCardItem>();

            if (itemUI != null)
            {
                itemUI.Setup(itemData);
            }
            else
            {
                Debug.LogError(
                    $"The item prefab {prefab.name} does not have an IDCardItem component."
                );
            }
        }
    }
}