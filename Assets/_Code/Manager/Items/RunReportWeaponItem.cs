using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunReportWeaponItem : MonoBehaviour
{
    public Image weaponLogo;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponLevelText;
    public TextMeshProUGUI weaponDamageText;

    public void Setup(GameStat_SO.WeaponData weaponData)
    {
        weaponLogo.sprite = weaponData.weaponDataSO.weaponLogo;
        weaponNameText.text = weaponData.weaponDataSO.weaponName.ToString();

        // Temporary
        weaponLevelText.text = "Lv. 1";

        float damage = 0;

        if (weaponData.statManager != null)
        {
            Stat damageStat = weaponData.statManager.GetStat(EStatType.Damage);

            if (damageStat != null)
                damage = damageStat.currentValue;
        }

        weaponDamageText.text = damage.ToString("F0");
    }
}