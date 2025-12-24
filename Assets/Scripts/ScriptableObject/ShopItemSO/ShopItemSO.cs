using UnityEngine;

/// <summary>
/// Shop-only extension of ItemSO.
/// Used for permanent upgrades bought before a run.
/// </summary>
[CreateAssetMenu(fileName = "NewShopItem", menuName = "Items/Shop Item")]
public class ShopItemSO : ItemSO
{
    [Header("Shop Data")]
    public int unlockCost; // Cost to unlock or upgrade in shop
}