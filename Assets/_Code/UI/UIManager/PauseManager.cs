using System;
using TMPro;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public GameObject gameScreenCanvas;
    
    public GameObject pausePanel;
    public GameObject settingPanel;
    
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
            UpdatePauseStats();
            pausePanel.SetActive(isPaused);
            settingPanel.SetActive(!isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
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
    
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SettingsMenu()
    {
        settingPanel.SetActive(true);
    }

    public void SettingsDone()
    {
        settingPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
