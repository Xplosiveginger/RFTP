using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameStat", menuName = "ScriptableObjects/Game Stats")]
public class GameStat_SO : ScriptableObject
{
    
    #region Economy
    [BoxGroup("Economy")]
    public int AllowanceMoney;
    public void UpdateAllowanceMoney(int amount)
    {
        AllowanceMoney = amount;
    }
    #endregion
    
    #region ===== STATS =====

    [Title("Offensive Stats")]
    [BoxGroup("Stats/Offense")]
    public float damage = 100f;

    [BoxGroup("Stats/Offense")]
    public float areaOfEffect = 100f;
    
    [BoxGroup("Highest Level Reached For Run Report")]
    public float highestLevel = 100f;

    [BoxGroup("Stats/Offense")]
    public float projectileSpeed = 100f;

    [BoxGroup("Stats/Offense")]
    public float numberOfProjectiles = 100f;

    [BoxGroup("Stats/Offense")]
    public float duration = 100f;


    [Title("Defensive Stats")]
    [BoxGroup("Stats/Defense")]
    public float totalHealth = 100f;

    [BoxGroup("Stats/Defense")]
    public float healthRegen = 100f;


    [Title("Utility Stats")]
    [BoxGroup("Stats/Utility")]
    public float cooldown = 100f;

    [BoxGroup("Stats/Utility")]
    public float moveSpeed = 100f;

    #endregion  

    #region ===== WEAPON DATA =====

    [System.Serializable]
    public struct WeaponData
    {
        public WeaponDataSO weaponDataSO;
        public StatManager statManager;
    }

    [Title("Weapons")]
    [BoxGroup("Loadout/Weapons")]
    public WeaponData weapon1;

    [BoxGroup("Loadout/Weapons")]
    public WeaponData weapon2;

    [BoxGroup("Loadout/Weapons")]
    public WeaponData weapon3;

    [BoxGroup("Loadout/Weapons")]
    public WeaponData weapon4;

    [Title("Equipped Weapon Names")]
    [BoxGroup("Loadout/Weapons")]
    [ShowInInspector, ReadOnly]
    [SerializeField] private List<EWeaponName> equippedWeaponNames = new List<EWeaponName>();

    #endregion

    #region ===== ITEM DATA =====

    [System.Serializable]
    public struct ItemData
    {
        public CardDataSO cardDataSO;
        public int level;

        public ItemData(CardDataSO cardDataSO, int level)
        {
            this.cardDataSO = cardDataSO;
            this.level = level;
        }
    }
    
    [Title("Items")]
    [BoxGroup("Loadout/Items")]
    public List<ItemData> items = new List<ItemData>();

    #endregion


    #region ===== EVENTS =====

    public event Action<int> OnWeaponUpdated;
    public event Action<int> OnSkillUpdated;
    public event Action OnWeaponDataReset;
    public event Action<List<EWeaponName>> OnEquippedWeaponNamesUpdated;

    #endregion


    #region ===== WEAPON FUNCTIONS =====

    public void SetWeaponData(int index, WeaponDataSO weaponDataSO, StatManager statManager)
    {
        WeaponData data = new WeaponData 
        { 
            weaponDataSO = weaponDataSO, 
            statManager = statManager 
        };

        switch (index)
        {
            case 1: weapon1 = data; break;
            case 2: weapon2 = data; break;
            case 3: weapon3 = data; break;
            case 4: weapon4 = data; break;
            default:
                Debug.LogError("Invalid Weapon Index");
                return;
        }
        
        // Update equipped weapon names list
        UpdateEquippedWeaponNames();
        
        OnWeaponUpdated?.Invoke(index);
    }

    public WeaponData GetWeaponData(int index)
    {
        return index switch
        {
            1 => weapon1,
            2 => weapon2,
            3 => weapon3,
            4 => weapon4,
            _ => throw new System.IndexOutOfRangeException("Invalid Weapon Index")
        };
    }

    public List<WeaponData> GetAllActiveWeapons()
    {
        List<WeaponData> activeWeapons = new List<WeaponData>();
        
        if (weapon1.weaponDataSO != null) activeWeapons.Add(weapon1);
        if (weapon2.weaponDataSO != null) activeWeapons.Add(weapon2);
        if (weapon3.weaponDataSO != null) activeWeapons.Add(weapon3);
        if (weapon4.weaponDataSO != null) activeWeapons.Add(weapon4);
        
        return activeWeapons;
    }

    public int GetFirstAvailableWeaponSlot()
    {
        if (weapon1.weaponDataSO == null) return 1;
        if (weapon2.weaponDataSO == null) return 2;
        if (weapon3.weaponDataSO == null) return 3;
        if (weapon4.weaponDataSO == null) return 4;
        
        return -1; // No available slots
    }

    public WeaponData GetWeaponByType(EWeaponName weaponName)
    {
        if (weapon1.weaponDataSO != null && weapon1.weaponDataSO.weaponName == weaponName) return weapon1;
        if (weapon2.weaponDataSO != null && weapon2.weaponDataSO.weaponName == weaponName) return weapon2;
        if (weapon3.weaponDataSO != null && weapon3.weaponDataSO.weaponName == weaponName) return weapon3;
        if (weapon4.weaponDataSO != null && weapon4.weaponDataSO.weaponName == weaponName) return weapon4;
        
        return default;
    }

    /// <summary>
    /// Updates the equipped weapon names list based on current weapon data
    /// </summary>
    private void UpdateEquippedWeaponNames()
    {
        equippedWeaponNames.Clear();
        
        var activeWeapons = GetAllActiveWeapons();
        
        foreach (var weaponData in activeWeapons)
        {
            if (weaponData.weaponDataSO != null)
            {
                equippedWeaponNames.Add(weaponData.weaponDataSO.weaponName);
            }
        }
        
        OnEquippedWeaponNamesUpdated?.Invoke(equippedWeaponNames);
        
        Debug.Log($"Equipped weapon names updated: [{string.Join(", ", equippedWeaponNames)}]");
    }

    /// <summary>
    /// Returns the current list of equipped weapon names
    /// </summary>
    public List<EWeaponName> GetEquippedWeaponNames()
    {
        return new List<EWeaponName>(equippedWeaponNames);
    }

    /// <summary>
    /// Checks if a specific weapon is currently equipped
    /// </summary>
    public bool IsWeaponEquipped(EWeaponName weaponName)
    {
        return equippedWeaponNames.Contains(weaponName);
    }
    
    public bool IsWeaponUnderMaxLevel(CardDataSO weapon)
    {
        WeaponBase baseWeapon = null;
        if (weapon1.weaponDataSO == weapon)
            baseWeapon = weapon1.statManager.GetComponent<WeaponBase>();
        else if(weapon2.weaponDataSO==weapon)
            baseWeapon = weapon1.statManager.GetComponent<WeaponBase>();
        else if(weapon3.weaponDataSO==weapon)
            baseWeapon = weapon1.statManager.GetComponent<WeaponBase>();
        else if(weapon4.weaponDataSO==weapon)
            baseWeapon = weapon1.statManager.GetComponent<WeaponBase>();
       
        if (baseWeapon == null) return false;
        return (baseWeapon.level <= 8);
        return true;
    }

    public void ResetWeaponData()
    {
        weapon1 = default;
        weapon2 = default;
        weapon3 = default;
        weapon4 = default;
        
        // Reset equipped weapon names list
        equippedWeaponNames.Clear();
        
        OnWeaponDataReset?.Invoke();
        Debug.Log("Weapon data has been reset");
    }

    #endregion


    #region ===== ITEM FUNCTIONS =====

    /// <summary>
    /// Updates GameStat_SO item data from ItemManager.
    /// GameStat_SO mirrors ItemManager.currentItems.
    /// </summary>
    public void UpdateItemsFromItemManager(List<ItemManager.CurrentItem> currentItems)
    {
        items.Clear();

        foreach (ItemManager.CurrentItem currentItem in currentItems)
        {
            if (currentItem.cardData == null)
                continue;

            items.Add(new ItemData(
                currentItem.cardData,
                currentItem.level
            ));
        }

        Debug.Log($"GameStat_SO updated. Current items: {items.Count}");
    }

    /// <summary>
    /// Clears all item data.
    /// </summary>
    public void ResetItemData()
    {
        items.Clear();

        Debug.Log("GameStat_SO item data has been reset.");
    }

    /// <summary>
    /// Gets the data for a specific item.
    /// </summary>
    public ItemData GetItemData(CardDataSO cardDataSO)
    {
        foreach (ItemData item in items)
        {
            if (item.cardDataSO == cardDataSO)
                return item;
        }

        return default;
    }

    /// <summary>
    /// Checks if an item is currently owned.
    /// </summary>
    public bool IsItemOwned(CardDataSO cardDataSO)
    {
        foreach (ItemData item in items)
        {
            if (item.cardDataSO == cardDataSO)
                return true;
        }

        return false;
    }

    
    public bool isItemUnderMaxLevel(CardDataSO card)
    {
        foreach(ItemData item in items)
        {
            if(item.cardDataSO == card)
            {
                return item.level < 8;
            }
        }
        return true;
    }

    #endregion

    
    #region Realtime Stats

    [Title("Realtime Stats")] 
    [ReadOnly] public float runTime = 0;
    public float damageGiven;
    public float damageTaken;
    public int EnemiesKilled;

    public int breakablesDestroyed;

    public void UpdateRuntime(float value)
    {
        runTime = value;
    }
    public void RegisterDamageTaken(float value)
    {
        if (value <= 0) return;
        damageTaken += value;
    }
    public void RegisterDamageGiven(float value)
    {
        if (value <= 0) return;
        damageGiven += value;
    }

    public void RegisterEnemyKilled(int value = 1)
    {
        EnemiesKilled += value;
    }

    public void RegisterBreakablesDestroyed(int value = 1)
    {
        breakablesDestroyed += value;
    }
    #endregion

    #region ===== RESET ALL =====

    [Button("Reset Weapon Data")]
    public void ResetWeaponDataButton()
    {
        ResetWeaponData();
    }

    [Button("Reset All Data")]
    public void ResetAllValues()
    {
        // Stats
        damage = 100f;
        areaOfEffect = 100f;
        projectileSpeed = 100f;
        numberOfProjectiles = 100f;
        duration = 100f;

        totalHealth = 100f;
        healthRegen = 100f;

        cooldown = 100f;
        moveSpeed = 100f;

        // Weapons
        ResetWeaponData();
        
        runTime = 0f;
        damageGiven = 0f;
        damageTaken = 0f;
        EnemiesKilled = 0;
        breakablesDestroyed = 0;
        
        ResetItemData();
        Debug.Log("All data has been reset");
    }

    #endregion
}