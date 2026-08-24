using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("Sound")]
    public AudioSource  audioSource;
    public AudioClip pauseSound;
    public AudioClip buttonClickSound;
    
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

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     runReportPanel.SetActive(!runReportPanel.activeSelf);
        // }
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
        PlaySoundForPause(pauseSound);
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

    private void PlaySoundForPause(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void Resume()
    {
        // Stop any currently running animation
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }
        PlaySoundForPause(pauseSound);
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
            totalHealthText.text = PlayerHealth.maxHealth.ToString();
        }

        if (gameStatSO == null)
            return;

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

    public void OpenSettingsMenu()
    {
        pauseBoxPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
        PlaySoundForPause(buttonClickSound);
        displaySettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        PlaySoundForPause(buttonClickSound);

        Application.Quit();
    }

    //========================================================
    // Settings Menu
    //========================================================

    public void OpenDisplaySettings()
    {
        PlaySoundForPause(buttonClickSound);

        settingsMenuPanel.SetActive(false);
        displaySettingsPanel.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        PlaySoundForPause(buttonClickSound);

        settingsMenuPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        PlaySoundForPause(buttonClickSound);

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
        PlaySoundForPause(buttonClickSound);

        displaySettingsPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }

    //========================================================
    // Audio Settings
    //========================================================

    public void CloseAudioSettings()
    {
        PlaySoundForPause(buttonClickSound);

        audioSettingsPanel.SetActive(false);
        settingsMenuPanel.SetActive(true);
    }
}