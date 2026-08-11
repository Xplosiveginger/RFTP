using System;
using TMPro;
using UnityEngine;

public class RunReport_UI : MonoBehaviour
{
    public RunReportManager runReportManager;

    [Header("Run Report Animation")]
    public Animator runReportAnimator;

    private GameStat_SO gameStat;

    [Header("Stats")]
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI moneyEarnedText;
    public TextMeshProUGUI enemyKilledText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI damageGivenText;
    public TextMeshProUGUI breakableDestroyedText;

    public GameObject weaponItemContainer;
    public GameObject weaponItemPrefab;

    public GameObject skillItemContainer;
    public GameObject skillItemPrefab;


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
            "Time Survived : " + gameStat.runTime.ToString("F2");

        moneyEarnedText.text =
            "Money Earned : " + gameStat.AllowanceMoney.ToString("F2");

        enemyKilledText.text =
            gameStat.EnemiesKilled.ToString("F2");

        damageTakenText.text =
            gameStat.damageTaken.ToString("F2");

        damageGivenText.text =
            gameStat.damageGiven.ToString("F2");

        breakableDestroyedText.text =
            gameStat.breakablesDestroyed.ToString("F2");

        PopulateWeapons();
        PopulateSkills();
    }


    private void PopulateWeapons()
    {
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

            ui.Setup(weapon);
        }
    }


    private void PopulateSkills()
    {
        foreach (Transform child in skillItemContainer.transform)
        {
            Destroy(child.gameObject);
        }

        GameStat_SO.SkillData[] skills =
        {
            gameStat.skill1,
            gameStat.skill2,
            gameStat.skill3,
            gameStat.skill4
        };

        foreach (var skill in skills)
        {
            if (skill.image == null)
                continue;

            GameObject item = Instantiate(
                skillItemPrefab,
                skillItemContainer.transform
            );

            RunReportSkillItem ui =
                item.GetComponent<RunReportSkillItem>();

            ui.Setup(skill);
        }
    }
}