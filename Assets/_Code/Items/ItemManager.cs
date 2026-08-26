using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all owned items/cards.
///
/// Responsibilities:
/// - Track owned items and their levels
/// - Spawn / replace visual prefabs for owned items
/// - Apply player stat modifications
/// - Apply weapon stat modifications
/// - Apply weapon level upgrades
/// - Reapply existing weapon modifiers to newly equipped weapons
///
/// NOT responsible for:
/// - Dropping items
/// - Detecting pickups
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    // =========================================================
    // DEPENDENCIES
    // =========================================================

    [Header("Dependencies")]
    [SerializeField] private StatManager statManager;

    [SerializeField] private ReworkedWeaponManager weaponManager;


    // =========================================================
    // STARTING ITEMS
    // =========================================================

    [Header("Starting Items (Shop / Meta)")]
    public List<ItemSO> startingItems;


    // =========================================================
    // CURRENT CARD ITEMS
    // =========================================================

    [Serializable]
    public struct CurrentItem
    {
        public CardDataSO cardData;
        public int level;

        public CurrentItem(CardDataSO cardData, int level)
        {
            this.cardData = cardData;
            this.level = level;
        }
    }

    public List<CurrentItem> currentItems =
        new List<CurrentItem>();


    // =========================================================
    // ACTIVE ITEM DATA
    // =========================================================

    private readonly List<ActiveItem> activeItems = new();

    [Serializable]
    private class ActiveItem
    {
        public ItemSO itemSO;
        public int currentLevel;
        public GameObject instance;
    }


    // =========================================================
    // GAME DATA
    // =========================================================

    [Header("Game Data")]
    [SerializeField] private GameStat_SO gameStatSO;


    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<ItemSO, int> OnItemAddedOrUpgraded;


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    private void Awake()
    {
        Instance = this;

        if (statManager == null)
            statManager = GetComponent<StatManager>();

        if (weaponManager == null)
            weaponManager = GetComponent<ReworkedWeaponManager>();

        if (statManager == null)
        {
            Debug.LogError(
                "ItemManager: Player StatManager is missing."
            );
        }

        if (weaponManager == null)
        {
            Debug.LogWarning(
                "ItemManager: ReworkedWeaponManager is missing. " +
                "Weapon cards will not be applied."
            );
        }
    }

    private void Start()
    {
        // Apply permanent shop/meta items at run start.
        foreach (ItemSO itemSO in startingItems)
        {
            if (itemSO != null)
            {
                AddItemFromSO(itemSO, 0);
            }
        }

        // Starting ItemSO objects are separate from
        // the run card system.
        currentItems.Clear();
    }


    // =========================================================
    // SHOP / META ITEM SYSTEM
    // =========================================================

    public void AddItemFromSO(
        ItemSO itemSO,
        int levelIndex)
    {
        if (itemSO == null)
            return;

        if (itemSO.levels == null ||
            itemSO.levels.Count == 0)
        {
            return;
        }

        levelIndex = Mathf.Clamp(
            levelIndex,
            0,
            itemSO.levels.Count - 1
        );

        ItemLevel level =
            itemSO.levels[levelIndex];

        GameObject instance = null;

        // Item visual spawning intentionally disabled.
        // Items currently apply their stat effects directly.

        ActiveItem newItem =
            new ActiveItem
            {
                itemSO = itemSO,
                currentLevel = levelIndex,
                instance = instance
            };

        activeItems.Add(newItem);

        // Apply ItemSO's player stat effect.
        if (statManager != null)
        {
            itemSO.StatModify(
                statManager,
                levelIndex
            );
        }

        OnItemAddedOrUpgraded?.Invoke(
            itemSO,
            levelIndex
        );
    }


    public void UpgradeItem(ItemSO itemSO)
    {
        if (itemSO == null)
            return;

        ActiveItem activeItem =
            activeItems.Find(
                i => i.itemSO == itemSO
            );

        if (activeItem == null)
            return;

        int nextLevel =
            activeItem.currentLevel + 1;

        if (nextLevel >= itemSO.levels.Count)
            return;

        if (activeItem.instance != null)
        {
            Destroy(activeItem.instance);
        }

        ItemLevel nextLevelData =
            itemSO.levels[nextLevel];

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

        // Apply new level stats.
        if (statManager != null)
        {
            itemSO.StatModify(
                statManager,
                nextLevel
            );
        }

        OnItemAddedOrUpgraded?.Invoke(
            itemSO,
            nextLevel
        );
    }


    public void AddItem(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            Debug.LogError(
                "ItemSO is null. Cannot add item."
            );

            return;
        }

        if (HasItem(itemSO))
        {
            UpgradeItem(itemSO);
        }
        else
        {
            AddItemFromSO(
                itemSO,
                0
            );
        }
    }


    public bool HasItem(ItemSO itemSO)
    {
        return activeItems.Exists(
            i => i.itemSO == itemSO
        );
    }


    // =========================================================
    // CARD EFFECT ROUTING
    // =========================================================

    /// <summary>
    /// Applies the appropriate effect from a CardDataSO.
    ///
    /// A card can affect:
    /// - Player
    /// - A specific weapon
    /// - All weapons
    /// - A weapon's level
    ///
    /// Player cards modify the player's StatManager.
    /// Weapon cards modify the weapon's OWN StatManager.
    /// </summary>
    private void ApplyCardEffect(
        CardDataSO cardDataSO)
    {
        if (cardDataSO == null)
            return;

        // -----------------------------------------------------
        // PLAYER
        // -----------------------------------------------------

        if (cardDataSO.affectsPlayer)
        {
            ApplyPlayerCardEffect(
                cardDataSO
            );
        }

        // -----------------------------------------------------
        // WEAPON STAT
        // -----------------------------------------------------

        if (cardDataSO.affectsWeaponStat)
        {
            ApplyWeaponCardEffect(
                cardDataSO
            );
        }

        // -----------------------------------------------------
        // WEAPON LEVEL
        // -----------------------------------------------------

        if (cardDataSO.affectsWeaponLevel)
        {
            ApplyWeaponLevelEffect(
                cardDataSO
            );
        }
    }


    // =========================================================
    // PLAYER CARD
    // =========================================================

    private void ApplyPlayerCardEffect(
        CardDataSO cardDataSO)
    {
        if (statManager == null)
        {
            Debug.LogError(
                "ItemManager: Player StatManager is missing."
            );

            return;
        }

        statManager.ModifyStat(
            cardDataSO.affectedPlayerStat,
            cardDataSO.playerStatModifier
        );

        Debug.Log(
            $"Applied PLAYER card '{cardDataSO.Name}': " +
            $"{cardDataSO.affectedPlayerStat} " +
            $"+{cardDataSO.playerStatModifier}%"
        );
    }


    // =========================================================
    // WEAPON CARD
    // =========================================================

    private void ApplyWeaponCardEffect(
        CardDataSO cardDataSO)
    {
        if (weaponManager == null)
        {
            Debug.LogError(
                "ItemManager: ReworkedWeaponManager is missing."
            );

            return;
        }

        // -----------------------------------------------------
        // SPECIFIC WEAPON
        // -----------------------------------------------------

        if (cardDataSO.cardType ==
            ECardType.AffectsSpecificWeaponStat)
        {
            WeaponBase targetWeapon =
                weaponManager.GetWeapon(
                    cardDataSO.weaponName
                );

            if (targetWeapon == null)
            {
                Debug.Log(
                    $"Weapon card '{cardDataSO.Name}' " +
                    $"stored for {cardDataSO.weaponName}. " +
                    "Weapon is not currently equipped."
                );

                // IMPORTANT:
                // Do not apply the modifier to another weapon.
                // The card remains in currentItems and will be
                // applied when the correct weapon is equipped.
                return;
            }

            ApplyModifierToWeapon(
                targetWeapon,
                cardDataSO
            );

            Debug.Log(
                $"Applied WEAPON card '{cardDataSO.Name}' " +
                $"to {cardDataSO.weaponName}: " +
                $"{cardDataSO.affectedWeaponStat} " +
                $"+{cardDataSO.weaponStatModifier}%"
            );

            return;
        }

        // -----------------------------------------------------
        // ALL WEAPONS
        // -----------------------------------------------------

        if (cardDataSO.cardType ==
            ECardType.AffectsAllWeaponsStat)
        {
            ApplyModifierToAllWeapons(
                cardDataSO
            );

            Debug.Log(
                $"Applied ALL WEAPONS card '{cardDataSO.Name}': " +
                $"{cardDataSO.affectedWeaponStat} " +
                $"+{cardDataSO.weaponStatModifier}%"
            );

            return;
        }

        Debug.LogWarning(
            $"Weapon card '{cardDataSO.Name}' has " +
            $"affectsWeaponStat enabled but its CardType is " +
            $"{cardDataSO.cardType}. No weapon modifier applied."
        );
    }


    // =========================================================
    // APPLY MODIFIER TO ONE WEAPON
    // =========================================================

    private void ApplyModifierToWeapon(
        WeaponBase weapon,
        CardDataSO cardDataSO)
    {
        if (weapon == null)
            return;

        if (weapon.statManager == null)
        {
            Debug.LogWarning(
                $"Weapon {weapon.name} has no StatManager."
            );

            return;
        }

        weapon.statManager.ModifyStat(
            cardDataSO.affectedWeaponStat,
            cardDataSO.weaponStatModifier
        );
    }


    // =========================================================
    // APPLY MODIFIER TO ALL WEAPONS
    // =========================================================

    private void ApplyModifierToAllWeapons(
        CardDataSO cardDataSO)
    {
        if (weaponManager == null)
            return;

        weaponManager.UpdateStatForAllWeapons(
            cardDataSO.affectedWeaponStat,
            cardDataSO.weaponStatModifier
        );
    }


    // =========================================================
    // WEAPON LEVEL CARD
    // =========================================================

    private void ApplyWeaponLevelEffect(
        CardDataSO cardDataSO)
    {
        if (weaponManager == null)
        {
            Debug.LogError(
                "ItemManager: ReworkedWeaponManager is missing."
            );

            return;
        }

        WeaponBase weapon =
            weaponManager.GetWeapon(
                cardDataSO.weaponName
            );

        if (weapon == null)
        {
            Debug.LogWarning(
                $"Cannot level up {cardDataSO.weaponName}. " +
                "Weapon is not currently equipped."
            );

            return;
        }

        weaponManager.LevelUpWeapon(
            cardDataSO.weaponName
        );

        Debug.Log(
            $"Leveled up weapon " +
            $"{cardDataSO.weaponName} using " +
            $"card '{cardDataSO.Name}'."
        );
    }


    // =========================================================
    // CURRENT CARD ITEMS
    // =========================================================

    public void AddCurrentItems(
        CardDataSO cardDataSO)
    {
        if (cardDataSO == null)
        {
            Debug.LogError(
                "CardDataSO is null. Cannot add item."
            );

            return;
        }

        // -----------------------------------------------------
        // EXISTING CARD -> LEVEL UP
        // -----------------------------------------------------

        for (int i = 0;
             i < currentItems.Count;
             i++)
        {
            if (currentItems[i].cardData != cardDataSO)
                continue;

            CurrentItem currentItem =
                currentItems[i];

            int maxLevel =
                cardDataSO.levelImages != null &&
                cardDataSO.levelImages.Count > 0
                    ? cardDataSO.levelImages.Count
                    : int.MaxValue;

            if (currentItem.level >= maxLevel)
            {
                Debug.Log(
                    $"{cardDataSO.Name} " +
                    "is already at max level."
                );

                return;
            }

            currentItem.level++;
            currentItems[i] = currentItem;

            Debug.Log(
                $"Upgraded {cardDataSO.Name} " +
                $"to Level {currentItem.level}"
            );

            // Apply this selection's effect.
            ApplyCardEffect(
                cardDataSO
            );

            UpdateGameStatItems();

            return;
        }

        // -----------------------------------------------------
        // NEW CARD
        // -----------------------------------------------------

        currentItems.Add(
            new CurrentItem(
                cardDataSO,
                1
            )
        );

        Debug.Log(
            $"Added {cardDataSO.Name} " +
            "at Level 1"
        );

        // Apply first-level effect.
        ApplyCardEffect(
            cardDataSO
        );

        UpdateGameStatItems();
    }


    // =========================================================
    // APPLY EXISTING WEAPON MODIFIERS TO NEW WEAPON
    // =========================================================

    /// <summary>
    /// Called after a new weapon is equipped.
    ///
    /// Reapplies all previously acquired weapon modifier
    /// cards that are relevant to this weapon.
    ///
    /// IMPORTANT:
    /// This modifies the weapon's OWN StatManager.
    /// It does NOT modify the player's StatManager.
    /// </summary>
    public void ApplyCurrentWeaponModifiersToWeapon(
        WeaponBase weapon)
    {
        if (weapon == null)
            return;

        if (weapon.statManager == null)
        {
            Debug.LogWarning(
                $"Cannot apply existing modifiers to " +
                $"{weapon.name}. StatManager is missing."
            );

            return;
        }

        foreach (CurrentItem currentItem in currentItems)
        {
            CardDataSO card =
                currentItem.cardData;

            if (card == null)
                continue;

            // -------------------------------------------------
            // ONLY WEAPON STAT CARDS
            // -------------------------------------------------

            if (!card.affectsWeaponStat)
                continue;

            // -------------------------------------------------
            // SPECIFIC WEAPON CARD
            // -------------------------------------------------

            if (card.cardType ==
                ECardType.AffectsSpecificWeaponStat)
            {
                if (weapon.weaponData == null)
                    continue;

                if (weapon.weaponData.weaponName !=
                    card.weaponName)
                {
                    continue;
                }

                ApplyWeaponModifierForOwnedLevels(
                    weapon,
                    card,
                    currentItem.level
                );

                continue;
            }

            // -------------------------------------------------
            // ALL WEAPONS CARD
            // -------------------------------------------------

            if (card.cardType ==
                ECardType.AffectsAllWeaponsStat)
            {
                ApplyWeaponModifierForOwnedLevels(
                    weapon,
                    card,
                    currentItem.level
                );
            }
        }
    }


    /// <summary>
    /// Applies a stored weapon card's modifier according
    /// to the number of times the card has been selected.
    ///
    /// Example:
    /// Card = +10% Damage
    /// Level = 3
    ///
    /// Weapon receives:
    /// +10%
    /// +10%
    /// +10%
    /// </summary>
    private void ApplyWeaponModifierForOwnedLevels(
        WeaponBase weapon,
        CardDataSO card,
        int level)
    {
        if (weapon == null || card == null)
            return;

        if (level <= 0)
            return;

        for (int i = 0; i < level; i++)
        {
            ApplyModifierToWeapon(
                weapon,
                card
            );
        }
    }


    // =========================================================
    // GAME STAT SO
    // =========================================================

    private void UpdateGameStatItems()
    {
        if (gameStatSO == null)
        {
            Debug.LogWarning(
                "GameStat_SO reference is missing " +
                "from ItemManager."
            );

            return;
        }

        gameStatSO.UpdateItemsFromItemManager(
            currentItems
        );
    }


    // =========================================================
    // REMOVE CURRENT ITEM
    // =========================================================

    public void RemoveCurrentItem(
        CardDataSO cardDataSO)
    {
        if (cardDataSO == null)
            return;

        for (int i = currentItems.Count - 1;
             i >= 0;
             i--)
        {
            if (currentItems[i].cardData !=
                cardDataSO)
            {
                continue;
            }

            currentItems.RemoveAt(i);

            Debug.Log(
                $"Removed current item: " +
                $"{cardDataSO.Name}"
            );

            UpdateGameStatItems();

            return;
        }
    }


    // =========================================================
    // CLEAR RUN ITEMS
    // =========================================================

    public void ClearRunItems()
    {
        foreach (ActiveItem activeItem in activeItems)
        {
            if (activeItem.itemSO == null)
                continue;

            if (activeItem.itemSO.itemType !=
                ItemType.Game)
            {
                continue;
            }

            // Revert all applied levels.
            for (int i = 0;
                 i <= activeItem.currentLevel;
                 i++)
            {
                ItemLevel level =
                    activeItem.itemSO.levels[i];

                Stat stat =
                    statManager.GetStat(
                        level.targetStat
                    );

                if (stat == null)
                    continue;

                if (level.isPercentage)
                {
                    stat.RevertModifier(
                        level.modifierAmount
                    );
                }
                else
                {
                    stat.AddFlat(
                        -level.modifierAmount
                    );
                }
            }

            if (activeItem.instance != null)
            {
                Destroy(
                    activeItem.instance
                );
            }
        }

        activeItems.RemoveAll(
            i =>
                i.itemSO != null &&
                i.itemSO.itemType ==
                ItemType.Game
        );

        currentItems.Clear();

        UpdateGameStatItems();
    }


    // =========================================================
    // STARTING ITEMS
    // =========================================================

    private void AddItemToStartingItems(
        ItemSO itemSO)
    {
        if (itemSO == null)
            return;

        if (startingItems.Contains(itemSO))
            return;

        startingItems.Add(
            itemSO
        );
    }
}