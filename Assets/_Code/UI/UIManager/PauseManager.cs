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

    private void Awake()
    {
        if (instance == null)
            instance = this;
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

        damageText.text = stats.damage.ToString();
        totalHealthText.text = stats.totalHealth.ToString();
        healthRegenText.text = stats.healthRegen.ToString();
        cooldownText.text = stats.cooldown.ToString();
        aoeText.text = stats.areaOfEffect.ToString();
        speedOfWeaponText.text = stats.projectileSpeed.ToString();
        durationText.text = stats.duration.ToString();
        numOfProjectilesText.text = stats.numberOfProjectiles.ToString();
        moveSpeedText.text = stats.moveSpeed.ToString();
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