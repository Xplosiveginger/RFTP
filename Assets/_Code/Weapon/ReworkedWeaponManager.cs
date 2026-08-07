using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedWeaponManager : MonoBehaviour
{
    [Header("References")]
    public Transform WeaponSpawnParentTransform;
    public StatManager ownerStats;
    public EnemyDetection enemyDetector;
    
    [Header("Game Stat SO")]
    [SerializeField] private GameStat_SO gameStatSO;
    
    [Header("Default Weapons")]
    [SerializeField] private WeaponDataSO[] defaultWeapons;
    
    [Header("Runtime Data")]
    public List<WeaponBase> activeWeapons;

    public event Action<EWeaponName> OnWeaponLeveledUp;

    private void Awake()
    {
        // Clear any existing active weapons list
        if (activeWeapons == null)
            activeWeapons = new List<WeaponBase>();
        else
            activeWeapons.Clear();
    }

    private void Start()
    {
        // Reset weapon data first
        gameStatSO.ResetWeaponData();
        
        // Initialize default weapons
        InitializeDefaultWeapons();
    }

    private void InitializeDefaultWeapons()
    {
        if (defaultWeapons == null || defaultWeapons.Length == 0) 
        {
            Debug.LogWarning("No default weapons assigned to WeaponManager");
            return;
        }
        
        foreach (var defaultWeapon in defaultWeapons)
        {
            if (defaultWeapon != null)
            {
                int availableSlot = gameStatSO.GetFirstAvailableWeaponSlot();
                if (availableSlot != -1)
                {
                    SpawnAndRegisterWeapon(defaultWeapon, availableSlot);
                    Debug.Log($"Spawned default weapon: {defaultWeapon.weaponName} in slot {availableSlot}");
                }
                else
                {
                    Debug.LogWarning("No available weapon slots for default weapon: " + defaultWeapon.weaponName);
                }
            }
        }
    }

    private void SpawnAndRegisterWeapon(WeaponDataSO weaponDataSO, int slotIndex)
    {
        WeaponBase weapon = SpawnWeapon(weaponDataSO);
        if (weapon != null)
        {
            // Wait for stat manager to initialize, then register
            if (weapon != null && weapon.statManager != null)
            {
                gameStatSO.SetWeaponData(slotIndex, weaponDataSO, weapon.statManager);
            }
            //StartCoroutine(RegisterWeaponAfterFrame(weaponDataSO, weapon, slotIndex));
        }
    }

    private IEnumerator RegisterWeaponAfterFrame(WeaponDataSO weaponDataSO, WeaponBase weapon, int slotIndex)
    {
        // Wait one frame to ensure stat manager is initialized
        //yield return null;
        
        if (weapon != null && weapon.statManager != null)
        {
            gameStatSO.SetWeaponData(slotIndex, weaponDataSO, weapon.statManager);
        }
        else
        {
            Debug.LogError($"Failed to register weapon {weaponDataSO.weaponName} - StatManager is null");
        }
        yield return null;
    }

    private WeaponBase SpawnWeapon(WeaponDataSO weaponDataSO)
    {
        if (weaponDataSO == null)
        {
            Debug.LogError("Cannot spawn null weapon data");
            return null;
        }
        
        WeaponBase weapon = weaponDataSO.SpawnWeapon(WeaponSpawnParentTransform);
        if (weapon != null)
        {
            weapon.enemyDetector = this.enemyDetector;
            AddActiveWeapon(weapon);
        }
        return weapon;
    }

    public void UpdateStatForAllWeapons(EStatType statName, float modifier)
    {
        foreach (var weapon in activeWeapons)
        {
            if (weapon != null && weapon.statManager != null)
            {
                weapon.statManager.ModifyStat(statName, modifier);
            }
        }
    }

    public void UpdateWeaponStat(EWeaponName weaponName, EStatType statName, float modifier)
    {
        WeaponBase weapon = GetWeapon(weaponName);
        if (weapon != null && weapon.statManager != null)
        {
            weapon.statManager.ModifyStat(statName, modifier);
        }
    }

    public WeaponBase GetWeapon(EWeaponName weaponName)
    {
        return activeWeapons.Find(weapon => weapon != null && weapon.weaponData != null && weapon.weaponData.weaponName == weaponName);
    }

    private void Update()
    {
        foreach(var weapon in activeWeapons)
        {
            if (weapon != null)
            {
                weapon.UpdateWeapon();
            }
        }
    }

    public void AddActiveWeapon(WeaponBase weapon)
    {
        if (weapon != null && !activeWeapons.Contains(weapon))
        {
            activeWeapons.Add(weapon);
        }
    }

    public void AddNewWeapon(WeaponDataSO weaponToAdd)
    {
        if (weaponToAdd == null) return;
        
        int availableSlot = gameStatSO.GetFirstAvailableWeaponSlot();
        if (availableSlot != -1)
        {
            SpawnAndRegisterWeapon(weaponToAdd, availableSlot);
            Debug.Log($"Added new weapon: {weaponToAdd.weaponName} in slot {availableSlot}");
        }
        else
        {
            Debug.LogWarning("No available weapon slots! Cannot add weapon: " + weaponToAdd.weaponName);
        }
    }

    public void LevelUpWeapon(EWeaponName weaponName)
    {
        WeaponBase weapon = GetWeapon(weaponName);
        if (weapon != null)
        {
            weapon.LevelUpWeapon();
            OnWeaponLeveledUp?.Invoke(weaponName);
            
            UpdateWeaponInGameStat(weaponName);
        }
    }

    private void UpdateWeaponInGameStat(EWeaponName weaponName)
    {
        for (int i = 1; i <= 4; i++)
        {
            try
            {
                var weaponData = gameStatSO.GetWeaponData(i);
                if (weaponData.weaponDataSO != null && weaponData.weaponDataSO.weaponName == weaponName)
                {
                    WeaponBase weapon = GetWeapon(weaponName);
                    if (weapon != null && weapon.statManager != null)
                    {
                        gameStatSO.SetWeaponData(i, weaponData.weaponDataSO, weapon.statManager);
                    }
                    break;
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                continue;
            }
        }
    }

    public StatManager GetWeaponStatManager(EWeaponName weaponName)
    {
        var weaponData = gameStatSO.GetWeaponByType(weaponName);
        return weaponData.statManager;
    }

    private void OnDestroy()
    {
        if (gameStatSO != null)
        {
            //gameStatSO.ResetWeaponData();                               //If necessary uncomment this line, 
            Debug.Log("WeaponManager destroyed - weapon data reset");
        }
        if (activeWeapons != null)
        {
            activeWeapons.Clear();
        }
    }
}