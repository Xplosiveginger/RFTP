using System;
using TMPro;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("Pause UI")]
    public GameObject gameScreenCanvas;

    // Root object that contains the entire pause UI
    public GameObject pauseRoot;

    // Resume / Settings / Exit box
    public GameObject pauseBoxPanel;

    // Display / Audio / Exit box
    public GameObject settingsMenuPanel;

    // Actual settings pages
    public GameObject displaySettingsPanel;
    public GameObject audioSettingsPanel;

    [Header("Stats")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI totalHealthText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI aoeText;
    public TextMeshProUGUI speedOfWeaponText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI numOfProjectilesText;
    public TextMeshProUGUI moveSpeedText;

    [Header("Run Report Test")]
    public GameObject runReportPanel;

    public bool isPaused;


    private HealthSystem PlayerHealth;
    private StatManager StatManager;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        PlayerHealth = SM.Instance.Player.GetComponent<HealthSystem>();
        StatManager = SM.Instance.Player.GetComponent<StatManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                UpdatePauseStats();

                pauseRoot.SetActive(true);

                pauseBoxPanel.SetActive(true);
                settingsMenuPanel.SetActive(false);

                displaySettingsPanel.SetActive(false);
                audioSettingsPanel.SetActive(false);
                IDCardManager.instance.ShowPauseUI();
                Time.timeScale = 0f;
            }
            else
            {
                Resume();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            runReportPanel.SetActive(!runReportPanel.activeSelf);
            RunReportManager.Instance.UpdateRunReportStats();
        }
    }

    public void UpdatePauseStats()
    {
        var stats = GameStatManager.instance.gameStats;
        if (PlayerHealth != null)
        {
            totalHealthText.text = PlayerHealth.currentHealth.ToString();
        }
        if (StatManager != null)
        {
            if((StatManager.GetStat(EStatType.Damage)!=null))
                damageText.text = (StatManager.GetStat(EStatType.Damage).currentValue).ToString();
            
            
            if(StatManager.GetStat(EStatType.HealthRegen)!=null)
                healthRegenText.text = (StatManager.GetStat(EStatType.HealthRegen).currentValue).ToString();
        
            if((StatManager.GetStat(EStatType.MoveSpeed)!=null))
                moveSpeedText.text = ((StatManager.GetStat(EStatType.MoveSpeed).currentValue)).ToString();
            
            if((StatManager.GetStat(EStatType.AOESize)!=null))
                aoeText.text = (StatManager.GetStat(EStatType.AOESize).currentValue).ToString();
            
            if(StatManager.GetStat(EStatType.ProjectileSpeed)!=null)
                speedOfWeaponText.text = (StatManager.GetStat(EStatType.ProjectileSpeed).currentValue).ToString();
            
            if((StatManager.GetStat(EStatType.ProjectileCount)!=null))
                numOfProjectilesText.text = (StatManager.GetStat(EStatType.ProjectileCount)).currentValue.ToString();
            
            if((StatManager.GetStat(EStatType.AttackCooldown)!=null))
                cooldownText.text = (StatManager.GetStat(EStatType.AttackCooldown)).currentValue.ToString();  
            
            if((StatManager.GetStat(EStatType.ActiveDuration)!=null))
                durationText.text = (StatManager.GetStat(EStatType.ActiveDuration)).currentValue.ToString();
            
            
        }
    }

    //========================================================
    // Pause Menu
    //========================================================

    public void Resume()
    {
        IDCardManager.instance.ShowGameplayUI();
        isPaused = false;
        
        pauseRoot.SetActive(false);

        pauseBoxPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);

        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenSettingsMenu()
    {
        pauseBoxPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);

        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //========================================================
    // Settings Menu
    //========================================================

    public void OpenDisplaySettings()
    {
        settingsMenuPanel.SetActive(false);
        displaySettingsPanel.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        settingsMenuPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        settingsMenuPanel.SetActive(false);

        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);

        pauseBoxPanel.SetActive(true);
    }

    //========================================================
    // Display Settings
    //========================================================

    public void CloseDisplaySettings()
    {
        displaySettingsPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    //========================================================
    // Audio Settings
    //========================================================

    public void CloseAudioSettings()
    {
        audioSettingsPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }
}