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

   //========================================================
// Stats
//========================================================

public void UpdatePauseStats()
{
    // -----------------------------------------------------
    // HEALTH
    // -----------------------------------------------------
    // Health is the exception.
    // We want to show the actual maximum health value.
    // -----------------------------------------------------

    if (PlayerHealth != null && totalHealthText != null)
    {
        totalHealthText.text =
            Mathf.RoundToInt(PlayerHealth.maxHealth).ToString();
    }

    if (StatManager == null)
        return;


    // -----------------------------------------------------
    // DAMAGE
    // -----------------------------------------------------

    Stat damage = StatManager.GetStat(EStatType.Damage);

    if (damage != null && damageText != null)
    {
        damageText.text =
            FormatPercentageModifier(damage);
    }


    // -----------------------------------------------------
    // HEALTH REGEN
    // -----------------------------------------------------

    Stat healthRegen =
        StatManager.GetStat(EStatType.HealthRegen);

    if (healthRegen != null && healthRegenText != null)
    {
        healthRegenText.text =
            healthRegen.currentValue.ToString("0.##");
    }


    // -----------------------------------------------------
    // MOVE SPEED
    // -----------------------------------------------------

    Stat moveSpeed =
        StatManager.GetStat(EStatType.MoveSpeed);

    if (moveSpeed != null && moveSpeedText != null)
    {
        moveSpeedText.text =
            FormatPercentageModifier(moveSpeed);
    }


    // -----------------------------------------------------
    // AOE SIZE
    // -----------------------------------------------------

    Stat aoeSize =
        StatManager.GetStat(EStatType.AOESize);

    if (aoeSize != null && aoeText != null)
    {
        aoeText.text =
            FormatPercentageModifier(aoeSize);
    }


    // -----------------------------------------------------
    // PROJECTILE SPEED
    // -----------------------------------------------------

    Stat projectileSpeed =
        StatManager.GetStat(EStatType.ProjectileSpeed);

    if (projectileSpeed != null && speedOfWeaponText != null)
    {
        speedOfWeaponText.text =
            FormatPercentageModifier(projectileSpeed);
    }


    // -----------------------------------------------------
    // PROJECTILE COUNT
    // -----------------------------------------------------
    // Projectile count is different because it uses flat
    // modifiers (+1 projectile, +2 projectiles, etc.).
    //
    // Show the actual whole-number value.
    // -----------------------------------------------------

    Stat projectileCount =
        StatManager.GetStat(EStatType.ProjectileCount);

    if (projectileCount != null && numOfProjectilesText != null)
    {
        numOfProjectilesText.text =
            Mathf.RoundToInt(projectileCount.currentValue).ToString();
    }


    // -----------------------------------------------------
    // COOLDOWN
    // -----------------------------------------------------

    Stat cooldown =
        StatManager.GetStat(EStatType.AttackCooldown);

    if (cooldown != null && cooldownText != null)
    {
        cooldownText.text =
            FormatPercentageModifier(cooldown);
    }


    // -----------------------------------------------------
    // ACTIVE DURATION
    // -----------------------------------------------------

    Stat duration =
        StatManager.GetStat(EStatType.ActiveDuration);

    if (duration != null && durationText != null)
    {
        durationText.text =
            FormatPercentageModifier(duration);
    }
}


//========================================================
// STAT UI HELPERS
//========================================================

private string FormatPercentageModifier(Stat stat)
{
    if (stat == null)
        return "0%";


    /*
     * currentMultiplier represents the total multiplier.
     *
     * Example:
     *
     * 1.00 = 0%
     * 1.10 = +10%
     * 1.20 = +20%
     * 0.90 = -10%
     *
     * We compare against the stat's starting multiplier
     * rather than assuming it is always exactly 1.
     */

    float percentage =
        ((stat.currentMultiplier /
          Mathf.Max(stat.startMultiplier, 0.0001f)) - 1f)
        * 100f;


    // Avoid displaying things like +9.999998%
    percentage = Mathf.Round(percentage);


    if (percentage > 0f)
        return $"+{percentage:0}%";

    if (percentage < 0f)
        return $"{percentage:0}%";

    return "0%";
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