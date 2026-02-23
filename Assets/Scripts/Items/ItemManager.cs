using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all owned items.
/// Responsibilities:
/// - Track owned items and their levels
/// - Spawn / replace visual prefabs for owned items
/// - Apply and revert stat modifications
/// 
/// NOT responsible for:
/// - Dropping items
/// - Detecting pickups
/// </summary>
public class ItemManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StatManager statManager;

    [Header("Starting Items (Shop / Meta)")]
    public List<ItemSO> startingItems;

    private readonly List<ActiveItem> activeItems = new();

    public event Action<ItemSO, int> OnItemAddedOrUpgraded;

    /// <summary>
    /// Runtime representation of an owned item.
    /// </summary>
    private class ActiveItem
    {
        public ItemSO itemSO;
        public int currentLevel;
        public GameObject instance; // Visual instance
    }

    private void Awake()
    {
        statManager = GetComponent<StatManager>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        // Apply permanent shop/meta items at run start
        foreach (var itemSO in startingItems)
            AddItemFromSO(itemSO, 0);
    }

    /// <summary>
    /// Adds an item at a specific level.
    /// Used for shop items or loading saved data.
    /// </summary>
    public void AddItemFromSO(ItemSO itemSO, int levelIndex)
    {
        if (itemSO.levels == null || itemSO.levels.Count == 0)
            return;

        levelIndex = Mathf.Clamp(levelIndex, 0, itemSO.levels.Count - 1);
        ItemLevel level = itemSO.levels[levelIndex];

        GameObject instance = null;

        // Spawn visual prefab if present
        if (level.itemPrefab != null)
        {
            instance = Instantiate(
                level.itemPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }

        var newItem = new ActiveItem
        {
            itemSO = itemSO,
            currentLevel = levelIndex,
            instance = instance
        };

        activeItems.Add(newItem);

        // Apply stat effect
        itemSO.StatModify(statManager, levelIndex);
        OnItemAddedOrUpgraded?.Invoke(itemSO, levelIndex);
    }

    /// <summary>
    /// Upgrades an existing item by one level.
    /// </summary>
    public void UpgradeItem(ItemSO itemSO)
    {
        ActiveItem activeItem = activeItems.Find(i => i.itemSO == itemSO);
        if (activeItem == null) return;

        int nextLevel = activeItem.currentLevel + 1;
        if (nextLevel >= itemSO.levels.Count) return;

        // Replace visual prefab
        if (activeItem.instance != null)
            Destroy(activeItem.instance);

        ItemLevel nextLevelData = itemSO.levels[nextLevel];
        GameObject newInstance = null;

        if (nextLevelData.itemPrefab != null)
        {
            newInstance = Instantiate(
                nextLevelData.itemPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }

        activeItem.currentLevel = nextLevel;
        activeItem.instance = newInstance;

        // Apply new level stats
        itemSO.StatModify(statManager, nextLevel);
        OnItemAddedOrUpgraded?.Invoke(itemSO, nextLevel);
    }

    /// <summary>
    /// Entry point used by GameItem pickup.
    /// </summary>
    public void AddItem(ItemSO itemSO)
    {
        if (HasItem(itemSO))
            UpgradeItem(itemSO);
        else
            AddItemFromSO(itemSO, 0);
    }

    public bool HasItem(ItemSO itemSO) =>
        activeItems.Exists(i => i.itemSO == itemSO);

    /// <summary>
    /// Clears all run-only items.
    /// Called when a run ends.
    /// </summary>
    public void ClearRunItems()
    {
        foreach (var activeItem in activeItems)
        {
            if (activeItem.itemSO.itemType != ItemType.Game)
                continue;

            // Revert all applied levels
            for (int i = 0; i <= activeItem.currentLevel; i++)
            {
                ItemLevel level = activeItem.itemSO.levels[i];
                Stat stat = statManager.GetStat(level.targetStat);
                if (stat == null) continue;

                if (level.isPercentage)
                    stat.RevertModifier(level.modifierAmount);
                else
                    stat.AddFlat(-level.modifierAmount);
            }

            // Destroy visual
            if (activeItem.instance != null)
                Destroy(activeItem.instance);
        }

        activeItems.RemoveAll(i => i.itemSO.itemType == ItemType.Game);
    }

    private void AddItemToStartingItems(ItemSO itemSO)
    {
        if (startingItems.Contains(itemSO)) return;

        startingItems.Add(itemSO);
    }
}
