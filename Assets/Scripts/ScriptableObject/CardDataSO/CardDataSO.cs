using UnityEngine;
using System.Collections.Generic;

public enum EPriority
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5
}

[CreateAssetMenu(fileName = "CardData", menuName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    
    public List<Sprite> levelImages; 

    [Header("Priority (1 = common, 5 = rare)")]
    public EPriority cardPriority = EPriority.One;

    [Header("Affects Player")]
    public bool affectsPlayer;
    public EStatType affectedPlayerStat;
    public float playerStatModifier;

    [Header("Affects Enemy")]
    public bool affectsEnemy;
    public EStatType affectedEnemyStat;
    public float enemyStatModifier;

    [Header("Weapon Upgrade Card?")]
    public bool affectsWeaponLevel;
    public EWeaponName weaponName;

    [Header("Affects Weapon Stats")]
    public bool affectsWeaponStat;
    public EStatType affectedWeaponStat;
    public float weaponStatModifier;
}
