using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class XpUIManager : MonoBehaviour
{
    [Header("References")]
    private XpManager xpManager;

    [Header("UI Elements")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;

    [Header("Settings")]
    public float fillDuration = 0.5f;

    private void Start()
    {
        if (xpManager == null)
        {
            xpManager = SM.Instance.XPManager;
            return;
        }

        InitializeUI();
    }

    private void OnEnable()
    {
        XpManager.OnXPUpdated += UpdateXPUI;
        XpManager.OnCoinsUpdated += UpdateXPUI;
    }

    private void OnDisable()
    {
        XpManager.OnXPUpdated -= UpdateXPUI;
        XpManager.OnCoinsUpdated -= UpdateXPUI;
    }

    private void InitializeUI()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = 1f;
            xpBar.value = xpManager.GetProgressPercentage();
        }

        UpdateTextElements();
    }

    private void UpdateXPUI()
    {
        if (xpManager == null || xpManager.progressionSO == null) return;

        UpdateXPBar();
        UpdateTextElements();
    }

    private void UpdateXPBar()
    {
        if (xpBar != null)
        {
            float targetValue = xpManager.GetProgressPercentage();
            xpBar.DOValue(targetValue, fillDuration).SetEase(Ease.OutCubic);
        }
    }

    private void UpdateTextElements()
    {
        if (xpManager == null || xpManager.progressionSO == null) return;

        if (levelText != null)
        {
            levelText.text = xpManager.progressionSO.currentLevel.ToString();
        }

        if (xpText != null)
        {
            xpText.text = $"{xpManager.progressionSO.currentXP}/{xpManager.progressionSO.currentXPRequired}";
        }
    }

    private void OnValidate()
    {
        if (xpManager == null)
        {
            xpManager = GetComponent<XpManager>();
        }
    }
}