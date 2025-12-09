using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedWeaponManager : MonoBehaviour
{
    public List<WeaponDataSO> weapons;
    public List<WeaponBase> activeWeapons;
    public StatManager ownerStats;

    private void Awake()
    {
        InitializeWeapon();
    }

    public void UpdateWeaponStat(EStatType statName, float modifier)
    {
        foreach (var weapon in activeWeapons)
        {
            weapon.statManager.ModifyStat(statName, modifier);
        }
    }

    public WeaponBase GetWeapon(EWeaponName weaponName)
    {
        return activeWeapons.Find(weapon => weapon.weaponData.weaponName == weaponName);
    }

    private void Update()
    {
        foreach(var weapon in activeWeapons)
        {
            weapon.UpdateWeapon();
        }
    }

    private void InitializeWeapon()
    {
        foreach(var weapon in weapons)
        {
            WeaponBase weaponToAdd = weapon.SpawnWeapon(transform);
            AddActiveWeapon(weaponToAdd);
        }
    }

    public void AddActiveWeapon(WeaponBase weapon)
    {
        activeWeapons.Add(weapon);
    }
}