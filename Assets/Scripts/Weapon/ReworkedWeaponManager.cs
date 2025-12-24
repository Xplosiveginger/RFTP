using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedWeaponManager : MonoBehaviour
{
    public List<WeaponDataSO> weapons;
    public List<WeaponBase> activeWeapons;
    public StatManager ownerStats;
    public EnemyDetection enemyDetector;

    public event Action<EWeaponName> OnWeaponLeveledUp; 

    private void Awake()
    {
        InitializeWeapon();
    }

    private void OnEnable()
    {
        OnWeaponLeveledUp += LevelUpWeaponHandled;
    }

    private void OnDisable()
    {
        OnWeaponLeveledUp -= LevelUpWeaponHandled;
    }

    public void UpdateStatForAllWeapons(EStatType statName, float modifier, bool subtract)
    {
        foreach (var weapon in activeWeapons)
        {
            weapon.statManager.ModifyStat(statName, modifier, subtract);
        }
    }

    public void UpdateWeaponStat(EWeaponName weaponName, EStatType statName, float modifier, bool subtract)
    {
        GetWeapon(weaponName).statManager.GetStat(statName).ApplyModifier(modifier, subtract);
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
            weaponToAdd.enemyDetector = this.enemyDetector;
            AddActiveWeapon(weaponToAdd);
        }
    }

    private void InitializeWeapon(WeaponDataSO weaponToAdd)
    {
        WeaponBase weapon = weaponToAdd.SpawnWeapon(transform);
        weapon.enemyDetector = this.enemyDetector;
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

    private void LevelUpWeaponHandled(EWeaponName weaponName)
    {
        GetWeapon(weaponName).LevelUpWeapon();
    }
}