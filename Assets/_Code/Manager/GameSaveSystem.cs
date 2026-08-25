using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveSystem : MonoBehaviour
{
    public static GameSaveSystem instance;

    private string savePath;
    public static event Action<int> OnMoneyChanged;
    [Serializable]
    public class SavedShopItem
    {
        public string itemID;
        public int level;

        public float modifier;
        public EStatType affectedStat;
        public bool isPercentage;
    }

    [Serializable]
    public class SaveData
    {
        public int playerMoney;

        public List<SavedShopItem> shopItems = new List<SavedShopItem>();
    }

    private SaveData saveData;

    public List<SavedShopItem> GetSavedShopItems()
    {
        return saveData.shopItems;
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );
        Debug.Log(savePath);
        LoadGame();
    }

    // =========================================================
    // MONEY
    // =========================================================

    public void SetMoney(int money)
    {
        saveData.playerMoney = money;

        SaveGame();

        OnMoneyChanged?.Invoke(saveData.playerMoney);
    }
// =========================================================
// TESTING
// =========================================================

    [ContextMenu("Testing/Add 1000 Money")]
    private void AddTestMoney()
    {
        SetMoney(saveData.playerMoney + 1000);

        Debug.Log(
            $"Testing: Added 1000 money. Current money: {saveData.playerMoney}"
        );
    }
    public int GetMoney()
    {
        return saveData.playerMoney;
    }

    // =========================================================
    // SHOP ITEM
    // =========================================================

    public void SaveShopItem(
        ShopItemSO item,
        int level,
        float modifier,
        EStatType affectedStat,
        bool isPercentage)
    {
        SavedShopItem savedItem = saveData.shopItems.Find(
            x => x.itemID == item.name
        );

        if (savedItem == null)
        {
            savedItem = new SavedShopItem
            {
                itemID = item.name
            };

            saveData.shopItems.Add(savedItem);
        }

        savedItem.level = level;
        savedItem.modifier = modifier;
        savedItem.affectedStat = affectedStat;
        savedItem.isPercentage = isPercentage;

        SaveGame();
    }
    // =========================================================
    // GET ITEM LEVEL
    // =========================================================

    public int GetItemLevel(ShopItemSO item)
    {
        SavedShopItem savedItem = saveData.shopItems.Find(
            x => x.itemID == item.name
        );

        if (savedItem == null)
            return 0;

        return savedItem.level;
    }

    // =========================================================
    // GET SAVED ITEM
    // =========================================================

    public SavedShopItem GetSavedItem(ShopItemSO item)
    {
        return saveData.shopItems.Find(
            x => x.itemID == item.name
        );
    }

    // =========================================================
    // RESET ONE ITEM
    // =========================================================

    public void ResetShopItem(ShopItemSO item)
    {
        SavedShopItem savedItem = saveData.shopItems.Find(
            x => x.itemID == item.name
        );

        if (savedItem == null)
            return;

        savedItem.level = 0;
        savedItem.modifier = 0f;

        SaveGame();
    }

    // =========================================================
    // RESET ALL SHOP ITEMS
    // =========================================================

    public void ClearShopData()
    {
        foreach (SavedShopItem item in saveData.shopItems)
        {
            item.level = 0;
            item.modifier = 0f;
            item.affectedStat = default;
            item.isPercentage = true;
        }

        SaveGame();

        Debug.Log("All shop modifiers reset.");
    }
    // =========================================================
    // SAVE
    // =========================================================

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved:\n" + json);
    }

    // =========================================================
    // LOAD
    // =========================================================

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            saveData = new SaveData();
            return;
        }

        string json = File.ReadAllText(savePath);

        saveData = JsonUtility.FromJson<SaveData>(json);

        if (saveData == null)
            saveData = new SaveData();

        if (saveData.shopItems == null)
            saveData.shopItems = new List<SavedShopItem>();

        Debug.Log("Game Loaded:\n" + json);
    }

    // =========================================================
    // DELETE ENTIRE SAVE
    // =========================================================

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        saveData = new SaveData();

        Debug.Log("Save deleted.");
    }
    // =========================================================
// TESTING
// =========================================================

    [ContextMenu("Testing/Delete Save File")]
    private void DeleteSaveFileForTesting()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Testing: Save file deleted.");
        }
        else
        {
            Debug.Log("Testing: No save file found.");
        }

        // Reset the in-memory data as well
        saveData = new SaveData();
    }
}