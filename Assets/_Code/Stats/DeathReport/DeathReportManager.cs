using TMPro;
using UnityEngine;

public class DeathReportManager : MonoBehaviour
{
    [Header("References")]
    private HealthSystemRefactor health;
    [SerializeField] private GameObject deathPanel;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeSurvivedText;
    [SerializeField] private TextMeshProUGUI allowanceMoneyEarnedText;
    [SerializeField] private TextMeshProUGUI biomesCompletedText;
    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI highestLevelText;
    [SerializeField] private TextMeshProUGUI totalDamageDealtText;
    [SerializeField] private TextMeshProUGUI totalDamageTakenText;


    public GameStat_SO gameStatSO;
     GameStatManager gameStatManager;

    private void Start()
    {
        gameStatManager = PersistentObject.Instance.GetComponent<GameStatManager>();
        gameStatManager.OnStatUpdated += UpdateUI;
    }
    private void UpdateUI()
    {
        // 
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDeath += HandlePlayerDeath;


    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
       // SetData();

        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    public void SetData()
    {
        timeSurvivedText.text = "03:42";
        allowanceMoneyEarnedText.text = "2500";
        biomesCompletedText.text = "4";
        enemiesKilledText.text = "37";
        highestLevelText.text = "12";
        totalDamageDealtText.text = "1840";
        totalDamageTakenText.text = "920";
    }

    private void OnDestroy()
    {
        gameStatManager.OnStatUpdated -= UpdateUI;
    }
}