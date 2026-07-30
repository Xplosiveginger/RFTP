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
            Debug.LogError("EconomyManager not found in the scene.");
            enabled = false;
            return;
        }

        UpdateAllowanceMoneyUI(economyManager.AllowanceMoney);

        economyManager.OnAllowanceMoneyChanged += UpdateAllowanceMoneyUI;
    }

    private void OnDestroy()
    {
        if (economyManager != null)
            economyManager.OnAllowanceMoneyChanged -= UpdateAllowanceMoneyUI;
    }

    private void UpdateAllowanceMoneyUI(long amount)
    {
        allowanceMoneyText.text = economyManager.GetFormattedMoney();
    }
}