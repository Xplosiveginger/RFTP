using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameStat", menuName = "ScriptableObjects/Game Stats")]
public class GameStat_SO : ScriptableObject
{
    #region ===== STATS =====

    [Title("Offensive Stats")]
    [BoxGroup("Stats/Offense")]
    public float damage = 100f;

    [BoxGroup("Stats/Offense")]
    public float areaOfEffect = 100f;

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

    #endregion


    #region ===== SKILL DATA =====

    [System.Serializable]
    public struct SkillData
    {
        public Sprite image;
        public int level;
    }

    [Title("Skills")]
    [BoxGroup("Loadout/Skills")]
    public SkillData skill1;

    [BoxGroup("Loadout/Skills")]
    public SkillData skill2;

    [BoxGroup("Loadout/Skills")]
    public SkillData skill3;

    [BoxGroup("Loadout/Skills")]
    public SkillData skill4;

    #endregion


    #region ===== EVENTS =====

    public event Action<int> OnWeaponUpdated;
    public event Action<int> OnSkillUpdated;
    public event Action OnWeaponDataReset;

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

    public void ResetWeaponData()
    {
        weapon1 = default;
        weapon2 = default;
        weapon3 = default;
        weapon4 = default;
        
        OnWeaponDataReset?.Invoke();
        Debug.Log("Weapon data has been reset");
    }

    #endregion


    #region ===== SKILL FUNCTIONS =====

    public void SetSkillData(int index, Sprite image, int level)
    {
        SkillData data = new SkillData { image = image, level = level };

        switch (index)
        {
            case 1: skill1 = data; break;
            case 2: skill2 = data; break;
            case 3: skill3 = data; break;
            case 4: skill4 = data; break;
            default:
                Debug.LogError("Invalid Skill Index");
                return;
        }
        
        OnSkillUpdated?.Invoke(index);
    }

    public SkillData GetSkillData(int index)
    {
        return index switch
        {
            1 => skill1,
            2 => skill2,
            3 => skill3,
            4 => skill4,
            _ => throw new System.IndexOutOfRangeException("Invalid Skill Index")
        };
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

        // Skills
        skill1 = default;
        skill2 = default;
        skill3 = default;
        skill4 = default;
        
        Debug.Log("All data has been reset");
    }

    #endregion
}