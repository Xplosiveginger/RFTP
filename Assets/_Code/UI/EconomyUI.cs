using TMPro;
using UnityEngine;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text allowanceMoneyText;

    private EconomyManager economyManager;

    private void Start()
    {
        economyManager = PersistentObject.Instance.GetComponent<EconomyManager>();

        if (economyManager == null)
        {
            Debug.LogError("EconomyManager not found in PersistentObject.");
            enabled = false;
            return;
        }

        // Show current run money immediately.
        UpdateRunMoneyUI(economyManager.CurrentRunMoney);

        economyManager.OnRunMoneyChanged += UpdateRunMoneyUI;
    }

    private void OnDestroy()
    {
        if (economyManager != null)
            economyManager.OnRunMoneyChanged -= UpdateRunMoneyUI;
    }

    private void UpdateRunMoneyUI(long amount)
    {
        allowanceMoneyText.text = economyManager.GetFormattedRunMoney();
    }
}