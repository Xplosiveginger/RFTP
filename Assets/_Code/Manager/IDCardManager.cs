using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IDCardManager : MonoBehaviour
{
    public static IDCardManager instance;
    [Header("Game Data")]
    public GameStat_SO gameStat;

    [Header("Gameplay UI")]
    public GameObject idPanel;

    public Transform screenWeaponsContainer;
    public GameObject screenWeaponItemPrefab;

    public Transform screenSkillsContainer;
    public GameObject screenSkillItemPrefab;
    
    [Header("Pause Animation")]
    public Animator pauseUIAnimator;
    public float riseAnimationDuration = 0.5f;
    private Coroutine hideRoutine;

    [Header("Pause UI")]
    public GameObject pauseUIIDCard;

    public Transform pauseWeaponsContainer;
    public GameObject pauseWeaponItemPrefab;

    public Transform pauseSkillsContainer;
    public GameObject pauseSkillItemPrefab;

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
    public void UpdatePlayerPortrait()
    {
        if (HealthSystem.Instance == null)
            return;

        float maxHealth = HealthSystem.Instance.MaxHealth;
        float currentHealth = HealthSystem.Instance.CurrentHealth;

        // Prevent division by zero
        if (maxHealth <= 0f)
        {
            playerImage.sprite = health0Sprite;
            return;
        }

        // Clamp health between 0 and max health
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Calculate percentage
        float healthPercent = (currentHealth / maxHealth) * 100f;

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
    public void RefreshAll()
    {
        UpdatePlayerPortrait();

        PopulateWeapons(screenWeaponsContainer, screenWeaponItemPrefab);
        PopulateSkills(screenSkillsContainer, screenSkillItemPrefab);

        PopulateWeapons(pauseWeaponsContainer, pauseWeaponItemPrefab);
        PopulateSkills(pauseSkillsContainer, pauseSkillItemPrefab);
    }
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

        hideRoutine = StartCoroutine(HidePauseCardRoutine());
    }
    private IEnumerator HidePauseCardRoutine()
    {
        pauseUIAnimator.Play("Rise", 0, 0f);

        yield return new WaitForSecondsRealtime(riseAnimationDuration);

        pauseUIAnimator.Rebind();
        pauseUIAnimator.Update(0f);

        pauseUIIDCard.SetActive(false);
        idPanel.SetActive(true);

        hideRoutine = null;
    }
    private void PopulateWeapons(Transform container, GameObject prefab)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        var weapons = gameStat.GetAllActiveWeapons();

        foreach (var weapon in weapons)
        {
            GameObject item = Instantiate(prefab, container);
            item.GetComponent<IDCardWeaponItem>().Setup(weapon);
        }
    }

    private void PopulateSkills(Transform container, GameObject prefab)
    {
        foreach (Transform child in container)
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

            GameObject item = Instantiate(prefab, container);
            item.GetComponent<IDCardSkillItem>().Setup(skill);
        }
    }
}