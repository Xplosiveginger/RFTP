using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public EWeaponName weaponName;
    public Sprite weaponLogo;
    public List<StatDataSO> weaponStatData;
    private List<Stat> weaponStats;
    public GameObject weaponPrefab;

    public List<StatDataSO> GetAllWeaponStatDatas()
    {
        return weaponStatData;
    }

    public WeaponBase SpawnWeapon(Transform parent)
    {
        GameObject go = Instantiate(weaponPrefab, parent.position, Quaternion.identity, parent);
        return go.GetComponent<WeaponBase>();
    }
}