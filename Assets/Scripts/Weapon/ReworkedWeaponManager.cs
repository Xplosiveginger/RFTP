using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedWeaponManager : MonoBehaviour
{
    public List<WeaponBase> weapons;
    public StatManager ownerStats;

    public void UpdateWeaponStat(EStatType statName, float modifier)
    {
        foreach (var weapon in weapons)
        {
            weapon.statManager.ModifyStat(statName, modifier);
        }
    }

    private void Update()
    {
        foreach(var weapon in weapons)
        {
            weapon.UpdateWeapon();
        }
    }
}