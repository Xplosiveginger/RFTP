using TMPro;
using UnityEngine;

public class RunReport_UI : MonoBehaviour
{
    public RunReportManager runReportManager;

    [Header("Run Report Animation")]
    public Animator runReportAnimator;

    private GameStat_SO gameStat;

    [Header("Audio")]
    public AudioClip runOverSound;

    [Header("Stats")]
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI moneyEarnedText;
    public TextMeshProUGUI enemyKilledText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI damageGivenText;
    public TextMeshProUGUI highestLevelText;

    [Header("Weapons")]
    public GameObject weaponItemContainer;
    public GameObject weaponItemPrefab;

    [Header("Items")]
    public GameObject itemItemContainer;
    public GameObject itemItemPrefab;

    [Header("Scroll Views")]
    public ScrollViewController scrollViewControllerWeapon;
    public ScrollViewController scrollViewControllerItems;


    private void Awake()
    {
        if (runReportManager == null)
        {
            Debug.LogError(
                "RunReport_UI: RunReportManager is not assigned!",
                this
            );

            return;
        }

        gameStat = runReportManager.gameStat;
    }


    private void OnEnable()
    {
        if (gameStat == null)
            return;

        Time.timeScale = 1;

        if (GlobalAudioPlayer.Instance != null)
        {
            GlobalAudioPlayer.Instance.isGameOver = true;
            GlobalAudioPlayer.Instance.PlayAudio(runOverSound, transform);
        }

        scrollViewControllerWeapon?.UpdateScrollState();
        scrollViewControllerItems?.UpdateScrollState();

        UpdateRunReportStats();
    }


    public void ShowRunReport()
    {
        // Get the latest GameStat
        if (runReportManager != null)
        {
            gameStat = runReportManager.gameStat;
        }

        if (gameStat == null)
        {
            Debug.LogError(
                "RunReport_UI: Cannot show report because gameStat is null!",
                this
            );

            return;
        }

        UpdateRunReportStats();

        if (runReportAnimator != null)
        {
            runReportAnimator.Play("SlideIn", 0, 0f);
        }
    }


    public void UpdateRunReportStats()
    {
        if (gameStat == null)
            return;

        timeSurvivedText.text =
            gameStat.runTime.ToString("F2");

        moneyEarnedText.text =
            gameStat.AllowanceMoney.ToString("0");

        enemyKilledText.text =
            gameStat.EnemiesKilled.ToString("0");

        damageTakenText.text =
            gameStat.damageTaken.ToString("F2");

        damageGivenText.text =
            gameStat.damageGiven.ToString("F2");

        highestLevelText.text =
            gameStat.playerLevel.ToString("0");


        PopulateWeapons();
        PopulateItems();
    }


    // ============================================================
    // WEAPONS
    // ============================================================

    private void PopulateWeapons()
    {
        if (weaponItemContainer == null || weaponItemPrefab == null)
            return;

        // Clear existing UI
        foreach (Transform child in weaponItemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        var weapons = gameStat.GetAllActiveWeapons();

        foreach (var weapon in weapons)
        {
            GameObject item = Instantiate(
                weaponItemPrefab,
                weaponItemContainer.transform
            );

            RunReportWeaponItem ui =
                item.GetComponent<RunReportWeaponItem>();

            if (ui != null)
            {
                ui.Setup(weapon);
            }
            else
            {
                Debug.LogError(
                    "RunReport_UI: weaponItemPrefab does not have a RunReportWeaponItem component.",
                    item
                );
            }
        }
    }


    // ============================================================
    // ITEMS
    // ============================================================

    private void PopulateItems()
    {
        if (itemItemContainer == null || itemItemPrefab == null)
            return;

        // Clear existing UI
        foreach (Transform child in itemItemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Get items stored in GameStat_SO
        foreach (GameStat_SO.ItemData itemData in gameStat.items)
        {
            // Safety check
            if (itemData.cardDataSO == null)
                continue;

            GameObject item = Instantiate(
                itemItemPrefab,
                itemItemContainer.transform
            );

            RunReportItemItem ui =
                item.GetComponent<RunReportItemItem>();

            if (ui != null)
            {
                ui.Setup(itemData);
            }
            else
            {
                Debug.LogError(
                    "RunReport_UI: itemItemPrefab does not have a RunReportItemItem component.",
                    item
                );
            }
        }
    }
}