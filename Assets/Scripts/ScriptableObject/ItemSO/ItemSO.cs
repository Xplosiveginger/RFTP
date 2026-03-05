using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines whether an item is permanent (Shop)
/// or temporary for a single run (Game).
/// </summary>
public enum ItemType
{
    Shop,   // Permanent, pre-run
    Game    // Temporary, run-only
}

/// <summary>
/// Represents ONE level/rank of an item.
/// Each level applies exactly one stat modification
/// and optionally has a visual prefab.
/// </summary>
[System.Serializable]

public class ItemLevel
{
    [Header("UI")]
    public string levelName;
    [TextArea(3, 5)]
    public string description;

    [Header("Visual Representation")]
    public GameObject itemPrefab;
    public Sprite cardSprite;

    [Header("Stat Effect")]
    public EStatType targetStat;
    public float modifierAmount;
    public bool isPercentage = true;
}

/// <summary>
/// ScriptableObject representing an item.
/// Used by both Shop items and Game items.
/// Contains ONLY data and stat application logic.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;

    [Header("Item Category")]
    public ItemType itemType;

    [Header("Level Configuration")]
    public List<ItemLevel> levels;

    /// <summary>
    /// Applies stat modification for the given level.
    /// Called by ItemManager when an item is added or upgraded.
    /// </summary>
    public void StatModify(StatManager statManager, int levelIndex)
    {
        if (statManager == null || levels == null || levels.Count == 0)
            return;

        levelIndex = Mathf.Clamp(levelIndex, 0, levels.Count - 1);
        ItemLevel level = levels[levelIndex];

        Stat stat = statManager.GetStat(level.targetStat);
        if (stat == null) return;

        if (level.isPercentage)
            stat.ApplyModifier(level.modifierAmount);
        else
            stat.AddFlat(level.modifierAmount);
    }
}
