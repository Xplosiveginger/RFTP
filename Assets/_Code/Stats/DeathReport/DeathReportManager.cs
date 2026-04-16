// using TMPro;
// using UnityEngine;

// public class DeathReportManager : MonoBehaviour
// {
//     [Header("References")]
//     [SerializeField] private HealthSystem health;
//     [SerializeField] private StatManager statManager;
//     [SerializeField] private GameObject deathPanel;
//     [Header("UI")]
//     [SerializeField] private TextMeshProUGUI timeSurvivedText;
//     [SerializeField] private TextMeshProUGUI allowanceMoneyEarnedText;
//     [SerializeField] private TextMeshProUGUI biomesCompletedText;
//     [SerializeField] private TextMeshProUGUI enemiesKilledText;
//     [SerializeField] private TextMeshProUGUI highestLevelText;
//     [SerializeField] private TextMeshProUGUI totalDamageDealtText;
//     [SerializeField] private TextMeshProUGUI totalDamageTakenText;

//     private void OnEnable()
//     {
//         if (health != null)
//             health.OnDeath += HandlePlayerDeath;
//     }

//     private void OnDisable()
//     {
//         if (health != null)
//             health.OnDeath -= HandlePlayerDeath;
//     }

//     private void HandlePlayerDeath()
//     {
//        // SetData();

//         if (deathPanel != null)
//             deathPanel.SetActive(true);
//     }

//     public void SetData()
//     {
//         if (health == null) return;

//         // HealthSystem stats
//         timeSurvivedText.text = health.timeSurvived.ToString("F2") + " s";
//         allowanceMoneyEarnedText.text = health.allowanceMoneyEarned.ToString("F0");
//         biomesCompletedText.text = health.biomesCompleted.ToString();
//         enemiesKilledText.text = health.enemiesKilled.ToString();
//         highestLevelText.text = health.highestLevel.ToString();
//         totalDamageTakenText.text = health.totalDamageTaken.ToString("F0");

//         // StatManager stat example
//         if (statManager != null)
//         {
//             Stat damageStat = statManager.GetStat(EStatType.Damage);

//             if (damageStat != null)
//                 totalDamageDealtText.text = damageStat.currentValue.ToString("F0");
//         }
//     }
// }