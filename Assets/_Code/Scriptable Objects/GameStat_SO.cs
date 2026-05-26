using UnityEngine;
using Sirenix.OdinInspector;

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


    #region ===== WEAPON & SKILL DATA =====

    [System.Serializable]
    public struct WeaponData
    {
        public Sprite image;
        public int level;
    }

    [System.Serializable]
    public struct SkillData
    {
        public Sprite image;
        public int level;
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


    #region ===== WEAPON FUNCTIONS =====

    public void SetWeaponData(int index, Sprite image, int level)
    {
        WeaponData data = new WeaponData { image = image, level = level };

        switch (index)
        {
            case 1: weapon1 = data; break;
            case 2: weapon2 = data; break;
            case 3: weapon3 = data; break;
            case 4: weapon4 = data; break;
            default:
                Debug.LogError("Invalid Weapon Index");
                break;
        }
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
                break;
        }
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


    #region ===== RESET =====

    public void ResetValues()
    {
        // Stats
        damage = default;
        areaOfEffect = default;
        projectileSpeed = default;
        numberOfProjectiles = default;
        duration = default;

        totalHealth = default;
        healthRegen = default;

        cooldown = default;
        moveSpeed = default;

        // Weapons
        weapon1 = default;
        weapon2 = default;
        weapon3 = default;
        weapon4 = default;

        // Skills
        skill1 = default;
        skill2 = default;
        skill3 = default;
        skill4 = default;
    }

    #endregion
}