using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "ScriptableObjects/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public EWeaponName weaponName;
    public List<Stat> weaponStats;
    public GameObject weaponPrefab;

    public List<Stat> GetAllWeaponStats()
    {
        return weaponStats;
    }

    public WeaponBase SpawnWeapon(Transform parent)
    {
        GameObject go = Instantiate(weaponPrefab, parent.position, Quaternion.identity, parent);
        return go.GetComponent<WeaponBase>();
    }
}

public enum EWeaponName
{
    AcidRainClouds,
    Bacteria,
    Laser,
    LithiumIon,
    Magnet,
    Prism,
    SolarSystem,
    TuningFork,
}