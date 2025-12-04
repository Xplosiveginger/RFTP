using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemLevel
{
    [Header("Visuals / Prefab")]
    public GameObject itemPrefab;
    public Sprite cardSprite;

    [Header("Stat Effect")]
    public EStatType targetStat;     // Which stat this level affects
    public float modifierAmount;     // e.g. 10 = +10%
    public bool isPercentage = true; // If true, use Stat.ApplyModifier
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;

    [Header("Per Level Settings")]
    public List<ItemLevel> levels;

    /// <summary>
    /// Applies the stat modification for the given level index.
    /// Called by ItemManager when this item becomes active / levels up.
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
        {
            stat.ApplyModifier(level.modifierAmount);
        }
        else
        {
            stat.currentValue += level.modifierAmount;
        }
    }
}
