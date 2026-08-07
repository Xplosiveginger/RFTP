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

        // Get weapon level
        int weaponLevel = 1;

        if (weaponData.statManager != null)
        {
            WeaponBase weaponBase = weaponData.statManager.gameObject.GetComponent<WeaponBase>();

            if (weaponBase != null)
            {
                weaponLevel = weaponBase.GetLevel;
                Debug.Log(weaponLevel +" Level");
            }
        }

        weaponLevelText.text = $"Lv. {weaponLevel}";

        // Get weapon damage
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