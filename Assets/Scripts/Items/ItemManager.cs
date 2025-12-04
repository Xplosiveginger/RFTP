using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StatManager statManager;

    [Header("Starting Items")]
    public List<ItemSO> startingItems;

    private readonly List<ActiveItem> activeItems = new List<ActiveItem>();

    public event Action<ItemSO, int> OnItemAddedOrUpgraded;

    private class ActiveItem
    {
        public ItemSO itemSO;
        public int currentLevel;
        public GameObject instance;
    }

    private void Awake()
    {
        if (statManager == null)
            statManager = StatManager.Instance;
    }

    private void Start()
    {
        foreach (var itemSO in startingItems)
        {
            AddItemFromSO(itemSO, 0);
        }
    }

    public void AddItemFromSO(ItemSO itemSO, int levelIndex)
    {
        if (itemSO.levels == null || itemSO.levels.Count == 0)
        {
            Debug.LogWarning($"[ItemManager] ItemSO '{itemSO.itemName}' has no levels defined!");
            return;
        }

        levelIndex = Mathf.Clamp(levelIndex, 0, itemSO.levels.Count - 1);
        var levelData = itemSO.levels[levelIndex];

        GameObject itemInstance =
            Instantiate(levelData.itemPrefab, transform.position, Quaternion.identity, transform);

        var newItem = new ActiveItem
        {
            itemSO = itemSO,
            currentLevel = levelIndex,
            instance = itemInstance
        };

        activeItems.Add(newItem);

        // Apply this level's stat effect
        itemSO.StatModify(statManager, levelIndex);

        OnItemAddedOrUpgraded?.Invoke(itemSO, levelIndex);
    }

    public void UpgradeItem(ItemSO itemSO)
    {
        ActiveItem activeItem = activeItems.Find(i => i.itemSO == itemSO);
        if (activeItem == null)
        {
            Debug.LogWarning($"[ItemManager] Tried to upgrade '{itemSO.itemName}' but player doesn't have it!");
            return;
        }

        int nextLevel = activeItem.currentLevel + 1;
        if (nextLevel >= itemSO.levels.Count)
        {
            Debug.Log($"[ItemManager] '{itemSO.itemName}' is already at max level!");
            return;
        }

        if (activeItem.instance != null)
            Destroy(activeItem.instance);

        var newLevelData = itemSO.levels[nextLevel];
        GameObject newInstance =
            Instantiate(newLevelData.itemPrefab, transform.position, Quaternion.identity, transform);

        activeItem.currentLevel = nextLevel;
        activeItem.instance = newInstance;

        // Apply the new level's stat effect
        itemSO.StatModify(statManager, nextLevel);

        OnItemAddedOrUpgraded?.Invoke(itemSO, nextLevel);
    }

    public bool HasItem(ItemSO itemSO) =>
        activeItems.Exists(i => i.itemSO == itemSO);

    public int GetItemLevel(ItemSO itemSO)
    {
        ActiveItem activeItem = activeItems.Find(i => i.itemSO == itemSO);
        return activeItem != null ? activeItem.currentLevel : -1;
    }

    public void AddItem(ItemSO itemSO)
    {
        if (HasItem(itemSO))
            UpgradeItem(itemSO);
        else
            AddItemFromSO(itemSO, 0);
    }
}
