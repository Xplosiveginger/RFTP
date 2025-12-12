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

    private void InitializeWeapon(WeaponDataSO weaponToAdd)
    {
        WeaponBase weapon = weaponToAdd.SpawnWeapon(transform);
        AddActiveWeapon(weapon);
    }

    public void AddActiveWeapon(WeaponBase weapon)
    {
        activeWeapons.Add(weapon);
    }

    public void AddNewWeapon(WeaponDataSO weaponToAdd)
    {
        InitializeWeapon(weaponToAdd);
    }
}