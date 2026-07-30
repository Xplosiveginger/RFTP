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
        Debug.Log("Weapon: " + weaponData.weaponDataSO.weaponName);

        if (weaponData.statManager == null)
        {
            Debug.Log("StatManager is NULL");
            return;
        }

        Debug.Log("Stat count: " + weaponData.statManager.statList.Count);

        Stat damageStat = weaponData.statManager.GetStat(EStatType.Damage);

        if (damageStat == null)
        {
            Debug.Log("Damage stat NOT FOUND");
        }
        else
        {
            Debug.Log("Damage = " + damageStat.currentValue);
        }
    }
}