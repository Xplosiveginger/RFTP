using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "CardData", menuName = "CardDataSO")]
public class CardDataSO : ScriptableObject
{
    [PreviewField(50f)]
    public Sprite Icon;

    public string Name;

    [TextArea]
    public string[] Description;

    public List<Sprite> levelImages;


    [Header("Card Info")]
    [PropertyOrder(0)]
    public EPriority cardPriority = EPriority.One;

    public ECardType cardType = ECardType.AffectsPlayer;


    // =========================================================
    // PLAYER
    // =========================================================
    [Header("Affects Player")]
    public ItemSO itemSO;
    public Item item;

    public bool affectsPlayer;

    public EStatType affectedPlayerStat;

    public float playerStatModifier;

    [Tooltip(
        "If enabled, playerStatModifier is treated as a percentage. " +
        "If disabled, it is treated as a flat value."
    )]
    public bool playerStatIsPercentage = true;

    // =========================================================
    // ENEMY
    // =========================================================

    [Header("Affects Enemy")]
    public bool affectsEnemy;

    public EStatType affectedEnemyStat;

    public float enemyStatModifier;


    // =========================================================
    // WEAPON LEVEL
    // =========================================================

    [Header("Weapon Upgrade Card")]
    public bool affectsWeaponLevel;

    public EWeaponName weaponName;


    // =========================================================
    // WEAPON STATS
    // =========================================================

    [Header("Affects Weapon Stats")]
    public bool affectsWeaponStat;

    public EStatType affectedWeaponStat;

    public float weaponStatModifier;

    [Tooltip(
        "If enabled, weaponStatModifier is treated as a percentage. " +
        "If disabled, it is treated as a flat value."
    )]
    public bool weaponStatIsPercentage = true;


    // =========================================================
    // ADD WEAPON
    // =========================================================

    [Header("Adds Weapon")]
    public WeaponDataSO weaponToAdd;


    // =========================================================
    // BUFF / DEBUFF
    // =========================================================

    public bool isBuffDebuff = false;

    public float time = 0f;
}