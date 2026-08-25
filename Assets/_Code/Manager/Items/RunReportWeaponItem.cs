using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunReportWeaponItem : MonoBehaviour
{
    public Image weaponLogo;
    public Image weaponBorder;

    public Sprite normalBorder;
    public Sprite maxLevelBorder;

    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponLevelText;
    public TextMeshProUGUI weaponDamageText;

    public void Setup(GameStat_SO.WeaponData weaponData)
    {
        weaponLogo.sprite = weaponData.weaponDataSO.weaponLogo;
        weaponNameText.text = weaponData.weaponDataSO.weaponName.ToString();

        // Get weapon
        WeaponBase weaponBase = null;

        if (weaponData.statManager != null)
        {
            weaponBase = weaponData.statManager.gameObject.GetComponent<WeaponBase>();
        }

        // Get weapon level
        int weaponLevel = 1;

        if (weaponBase != null)
        {
            weaponLevel = weaponBase.GetLevel;
        }

        weaponLevelText.text = $"Lv. {weaponLevel}";

        // Set border
        if (weaponBase != null && weaponLevel >= weaponBase.weaponData.maxLevel)
            weaponBorder.sprite = maxLevelBorder;
        else
            weaponBorder.sprite = normalBorder;

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