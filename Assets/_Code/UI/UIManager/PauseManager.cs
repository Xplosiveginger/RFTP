using System;
using System.Collections;
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
    public GameObject statusPanel;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI totalHealthText;
    public TextMeshProUGUI healthRegenText;
    public TextMeshProUGUI cooldownText;
    public TextMeshProUGUI aoeText;
    public TextMeshProUGUI speedOfWeaponText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI numOfProjectilesText;
    public TextMeshProUGUI moveSpeedText;

    [Header("Status Panel Animation")]
    public Animator statusPanelAnimator;
    public float statusAnimationDuration = 0.5f;

    [Header("Run Report Test")]
    public GameObject runReportPanel;

    [Header("Game Data")]
    [SerializeField] private GameStat_SO gameStatSO;

    public bool isPaused;

    private HealthSystem PlayerHealth;
    private StatManager StatManager;

    private Coroutine statusPanelRoutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        PlayerHealth = SM.Instance.Player.GetComponent<HealthSystem>();
        StatManager = SM.Instance.Player.GetComponent<StatManager>();

        // Make sure the animation can play while the game is paused
        if (statusPanelAnimator != null)
        {
            statusPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            runReportPanel.SetActive(!runReportPanel.activeSelf);
            RunReportManager.Instance.UpdateRunReportStats();
        }
    }

    //========================================================
    // Pause / Resume
    //========================================================

    public void Pause()
    {
        // Stop any currently running pause/resume animation
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }

        isPaused = true;

        UpdatePauseStats();

        // Enable pause UI
        pauseRoot.SetActive(true);

        pauseBoxPanel.SetActive(true);
        settingsMenuPanel.SetActive(false);

        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);

        // Show ID card
        IDCardManager.instance.ShowPauseUI();

        // Enable status panel
        if (statusPanel != null)
        {
            statusPanel.SetActive(true);

            if (statusPanelAnimator != null)
            {
                statusPanelAnimator.Play("SlideIn", 0, 0f);
            }
        }

        // Pause game AFTER setting up the UI
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        // Stop any currently running animation
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }

        isPaused = false;

        // Start resume animation
        statusPanelRoutine = StartCoroutine(ResumeRoutine());
    }

    private IEnumerator ResumeRoutine()
    {
        // Play ID card hide animation
        IDCardManager.instance.ShowGameplayUI();

        // Play status panel slide out
        if (statusPanel != null && statusPanelAnimator != null)
        {
            statusPanelAnimator.Play("SlideOut", 0, 0f);

            // Give the animation time to finish
            yield return new WaitForSecondsRealtime(statusAnimationDuration);

            statusPanel.SetActive(false);
        }
        else
        {
            if (statusPanel != null)
                statusPanel.SetActive(false);
        }

        // Now hide the pause UI
        pauseRoot.SetActive(false);

        pauseBoxPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);

        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        statusPanelRoutine = null;
    }

    //========================================================
    // Stats
    //========================================================

    public void UpdatePauseStats()
    {
        if (PlayerHealth != null)
        {
            totalHealthText.text = PlayerHealth.currentHealth.ToString();
        }

        if (gameStatSO == null)
            return;

        damageText.text = gameStatSO.damage.ToString();
        aoeText.text = gameStatSO.areaOfEffect.ToString();
        healthRegenText.text = gameStatSO.healthRegen.ToString();
        moveSpeedText.text = gameStatSO.moveSpeed.ToString();
        speedOfWeaponText.text = gameStatSO.projectileSpeed.ToString();
        numOfProjectilesText.text = gameStatSO.numberOfProjectiles.ToString();
        cooldownText.text = gameStatSO.cooldown.ToString();
        durationText.text = gameStatSO.duration.ToString();
    }

    //========================================================
    // Pause Menu
    //========================================================

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