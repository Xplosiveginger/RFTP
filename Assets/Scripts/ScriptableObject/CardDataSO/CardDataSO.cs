using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardData", menuName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    public List<Sprite> levelImages; 

    [Header("Priority (1 = common, 5 = rare)")]
    public EPriority cardPriority = EPriority.One;
    public ECardType cardType = ECardType.AffectsPlayer;

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

    [Header("Adds Weapon")]
    public WeaponDataSO weaponToAdd;

    public bool isBuffDebuff = false;
    public float time = 0f;
}
