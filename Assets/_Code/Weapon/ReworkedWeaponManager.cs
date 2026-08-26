using System;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedWeaponManager : MonoBehaviour
{
    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public Transform WeaponSpawnParentTransform;
    public EnemyDetection enemyDetector;
    public StatManager ownerStats;

    [Header("Game Stat SO")]
    [SerializeField] private GameStat_SO gameStatSO;

    [Header("Item Manager")]
    [SerializeField] private ItemManager itemManager;


    // =========================================================
    // DEFAULT WEAPONS
    // =========================================================

    [Header("Default Weapons")]
    [SerializeField] private WeaponDataSO[] defaultWeapons;


    // =========================================================
    // RUNTIME DATA
    // =========================================================

    [Header("Runtime Data")]
    public List<WeaponBase> activeWeapons;

    public event Action<EWeaponName> OnWeaponLeveledUp;


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    private void Awake()
    {
        if (activeWeapons == null)
        {
            activeWeapons =
                new List<WeaponBase>();
        }
        else
        {
            activeWeapons.Clear();
        }

        if (ownerStats == null)
        {
            ownerStats =
                GetComponent<StatManager>();
        }

        if (itemManager == null)
        {
            itemManager =
                GetComponent<ItemManager>();
        }

        if (ownerStats == null)
        {
            Debug.LogError(
                "ReworkedWeaponManager: " +
                "Player StatManager not found."
            );
        }

        if (itemManager == null)
        {
            Debug.LogWarning(
                "ReworkedWeaponManager: " +
                "ItemManager not found. " +
                "Existing weapon cards will not be applied " +
                "to newly spawned weapons."
            );
        }
    }


    private void Start()
    {
        if (gameStatSO == null)
        {
            Debug.LogError(
                "ReworkedWeaponManager: " +
                "GameStat_SO is missing."
            );

            return;
        }

        if (ownerStats == null)
        {
            ownerStats =
                GetComponent<StatManager>();
        }

        if (itemManager == null)
        {
            itemManager =
                GetComponent<ItemManager>();
        }

        // -----------------------------------------------------
        // Start with a clean weapon loadout.
        // -----------------------------------------------------

        gameStatSO.ResetWeaponData();


        // -----------------------------------------------------
        // Spawn default weapons.
        // -----------------------------------------------------

        InitializeDefaultWeapons();
    }


    private void Update()
    {
        if (activeWeapons == null)
            return;

        foreach (WeaponBase weapon in activeWeapons)
        {
            if (weapon != null)
            {
                weapon.UpdateWeapon();
            }
        }
    }


    private void OnDestroy()
    {
        if (activeWeapons != null)
        {
            activeWeapons.Clear();
        }

        Debug.Log(
            "WeaponManager destroyed - " +
            "weapon data cleared."
        );
    }


    // =========================================================
    // INITIALIZE DEFAULT WEAPONS
    // =========================================================

    private void InitializeDefaultWeapons()
    {
        if (defaultWeapons == null ||
            defaultWeapons.Length == 0)
        {
            Debug.LogWarning(
                "No default weapons assigned to WeaponManager."
            );

            return;
        }

        foreach (WeaponDataSO defaultWeapon in defaultWeapons)
        {
            if (defaultWeapon == null)
                continue;

            int availableSlot =
                gameStatSO.GetFirstAvailableWeaponSlot();

            if (availableSlot == -1)
            {
                Debug.LogWarning(
                    "No available weapon slots for " +
                    defaultWeapon.weaponName
                );

                continue;
            }

            SpawnAndRegisterWeapon(
                defaultWeapon,
                availableSlot
            );

            Debug.Log(
                $"Spawned default weapon: " +
                $"{defaultWeapon.weaponName} " +
                $"in slot {availableSlot}"
            );
        }
    }


    // =========================================================
    // SPAWN / REGISTER WEAPON
    // =========================================================

    private void SpawnAndRegisterWeapon(
        WeaponDataSO weaponDataSO,
        int slotIndex)
    {
        if (weaponDataSO == null)
            return;


        // -----------------------------------------------------
        // SPAWN
        // -----------------------------------------------------

        WeaponBase weapon =
            SpawnWeapon(
                weaponDataSO
            );

        if (weapon == null)
        {
            Debug.LogError(
                $"Failed to spawn weapon: " +
                $"{weaponDataSO.weaponName}"
            );

            return;
        }


        // -----------------------------------------------------
        // VERIFY WEAPON STAT MANAGER
        // -----------------------------------------------------

        if (weapon.statManager == null)
        {
            Debug.LogError(
                $"Weapon {weaponDataSO.weaponName} " +
                "has no StatManager."
            );

            return;
        }


        // -----------------------------------------------------
        // STORE WEAPON IN GAME STAT SO
        // -----------------------------------------------------

        gameStatSO.SetWeaponData(
            slotIndex,
            weaponDataSO,
            weapon.statManager
        );


        // -----------------------------------------------------
        // CONNECT PLAYER STATS
        // -----------------------------------------------------
        //
        // The weapon does NOT copy player currentValue.
        //
        // The weapon keeps its own stats and uses the
        // player's multiplier/flat modifier when calculating
        // its effective runtime values.
        //

        if (ownerStats != null)
        {
            weapon.UpdateStatsFromPlayer(
                ownerStats
            );
        }


        // -----------------------------------------------------
        // APPLY EXISTING WEAPON CARDS
        // -----------------------------------------------------
        //
        // IMPORTANT:
        //
        // This handles the situation where:
        //
        // 1. Player gets Laser +20% Damage card
        // 2. Laser is NOT equipped yet
        // 3. Player later obtains Laser
        //
        // The new Laser must receive that +20%.
        //
        // It also handles all-weapons cards that were obtained
        // before this weapon existed.
        //

        if (itemManager != null)
        {
            itemManager.ApplyCurrentWeaponModifiersToWeapon(
                weapon
            );
        }


        // -----------------------------------------------------
        // RECALCULATE FINAL EFFECTIVE VALUES
        // -----------------------------------------------------
        //
        // At this point:
        //
        // Weapon own stats
        //       +
        // Existing weapon cards
        //       +
        // Player global multipliers
        //
        // are all ready.
        //

        weapon.UpdateWeaponDamage();
    }


    // =========================================================
    // SPAWN WEAPON
    // =========================================================

    private WeaponBase SpawnWeapon(
        WeaponDataSO weaponDataSO)
    {
        if (weaponDataSO == null)
        {
            Debug.LogError(
                "Cannot spawn null weapon data."
            );

            return null;
        }

        if (WeaponSpawnParentTransform == null)
        {
            Debug.LogError(
                "WeaponSpawnParentTransform is missing."
            );

            return null;
        }

        WeaponBase weapon =
            weaponDataSO.SpawnWeapon(
                WeaponSpawnParentTransform
            );

        if (weapon == null)
        {
            Debug.LogError(
                $"WeaponDataSO failed to spawn " +
                $"{weaponDataSO.weaponName}."
            );

            return null;
        }

        weapon.enemyDetector =
            enemyDetector;

        AddActiveWeapon(
            weapon
        );

        return weapon;
    }


    // =========================================================
    // ACTIVE WEAPONS
    // =========================================================

    public void AddActiveWeapon(
        WeaponBase weapon)
    {
        if (weapon == null)
            return;

        if (activeWeapons.Contains(weapon))
            return;

        activeWeapons.Add(
            weapon
        );
    }


    public WeaponBase GetWeapon(
        EWeaponName weaponName)
    {
        if (activeWeapons == null)
            return null;

        return activeWeapons.Find(
            weapon =>
                weapon != null &&
                weapon.weaponData != null &&
                weapon.weaponData.weaponName ==
                weaponName
        );
    }


    // =========================================================
    // WEAPON STAT MODIFIERS
    // =========================================================

    /// <summary>
    /// Applies a percentage modifier to a stat
    /// on every currently equipped weapon.
    ///
    /// Example:
    ///
    /// Damage +20%
    ///
    /// Laser:
    /// 12 -> 14.4
    ///
    /// Bacteria:
    /// 30 -> 36
    ///
    /// IMPORTANT:
    /// This modifies each weapon's OWN StatManager.
    /// It does not modify the player's StatManager.
    /// </summary>
    public void UpdateStatForAllWeapons(
        EStatType statName,
        float modifier)
    {
        if (activeWeapons == null)
            return;

        foreach (WeaponBase weapon in activeWeapons)
        {
            if (weapon == null ||
                weapon.statManager == null)
            {
                continue;
            }

            weapon.statManager.ModifyStat(
                statName,
                modifier
            );
        }
    }


    /// <summary>
    /// Applies a flat modifier to a stat on
    /// every currently equipped weapon.
    ///
    /// Example:
    ///
    /// ProjectileCount +1
    /// </summary>
    public void AddFlatStatForAllWeapons(
        EStatType statName,
        float value)
    {
        if (activeWeapons == null)
            return;

        foreach (WeaponBase weapon in activeWeapons)
        {
            if (weapon == null ||
                weapon.statManager == null)
            {
                continue;
            }

            weapon.statManager.ModifyStatValue(
                statName,
                value
            );
        }
    }


    /// <summary>
    /// Applies a percentage modifier to one
    /// specific weapon.
    /// </summary>
    public void UpdateWeaponStat(
        EWeaponName weaponName,
        EStatType statName,
        float modifier)
    {
        WeaponBase weapon =
            GetWeapon(
                weaponName
            );

        if (weapon == null)
        {
            Debug.LogWarning(
                $"Cannot modify {weaponName}. " +
                "Weapon is not equipped."
            );

            return;
        }

        if (weapon.statManager == null)
        {
            Debug.LogWarning(
                $"{weaponName} has no StatManager."
            );

            return;
        }

        weapon.statManager.ModifyStat(
            statName,
            modifier
        );
    }


    /// <summary>
    /// Applies a flat modifier to one
    /// specific weapon.
    /// </summary>
    public void AddFlatWeaponStat(
        EWeaponName weaponName,
        EStatType statName,
        float value)
    {
        WeaponBase weapon =
            GetWeapon(
                weaponName
            );

        if (weapon == null)
        {
            Debug.LogWarning(
                $"Cannot modify {weaponName}. " +
                "Weapon is not equipped."
            );

            return;
        }

        if (weapon.statManager == null)
        {
            Debug.LogWarning(
                $"{weaponName} has no StatManager."
            );

            return;
        }

        weapon.statManager.ModifyStatValue(
            statName,
            value
        );
    }


    // =========================================================
    // ADD NEW WEAPON
    // =========================================================

    public void AddNewWeapon(
        WeaponDataSO weaponToAdd)
    {
        if (weaponToAdd == null)
            return;

        if (gameStatSO == null)
        {
            Debug.LogError(
                "GameStat_SO is missing."
            );

            return;
        }

        int availableSlot =
            gameStatSO.GetFirstAvailableWeaponSlot();

        if (availableSlot == -1)
        {
            Debug.LogWarning(
                "No available weapon slots! " +
                $"Cannot add weapon: " +
                $"{weaponToAdd.weaponName}"
            );

            return;
        }

        SpawnAndRegisterWeapon(
            weaponToAdd,
            availableSlot
        );

        Debug.Log(
            $"Added new weapon: " +
            $"{weaponToAdd.weaponName} " +
            $"in slot {availableSlot}"
        );
    }


    // =========================================================
    // LEVEL UP WEAPON
    // =========================================================

    public void LevelUpWeapon(
        EWeaponName weaponName)
    {
        WeaponBase weapon =
            GetWeapon(
                weaponName
            );

        if (weapon == null)
        {
            Debug.LogWarning(
                $"Cannot level up {weaponName}. " +
                "Weapon is not equipped."
            );

            return;
        }


        // -----------------------------------------------------
        // Weapon subclass modifies its OWN stats here.
        // -----------------------------------------------------

        weapon.LevelUpWeapon();


        OnWeaponLeveledUp?.Invoke(
            weaponName
        );


        // -----------------------------------------------------
        // Store updated weapon data.
        // -----------------------------------------------------

        UpdateWeaponInGameStat(
            weaponName
        );


        // -----------------------------------------------------
        // Recalculate effective runtime stats.
        // -----------------------------------------------------

        weapon.UpdateWeaponDamage();
    }


    private void UpdateWeaponInGameStat(
        EWeaponName weaponName)
    {
        if (gameStatSO == null)
            return;

        for (int i = 1; i <= 4; i++)
        {
            var weaponData =
                gameStatSO.GetWeaponData(i);

            if (weaponData.weaponDataSO == null)
                continue;

            if (weaponData.weaponDataSO.weaponName !=
                weaponName)
            {
                continue;
            }

            WeaponBase weapon =
                GetWeapon(
                    weaponName
                );

            if (weapon != null &&
                weapon.statManager != null)
            {
                gameStatSO.SetWeaponData(
                    i,
                    weaponData.weaponDataSO,
                    weapon.statManager
                );
            }

            break;
        }
    }


    // =========================================================
    // GET WEAPON STAT MANAGER
    // =========================================================

    public StatManager GetWeaponStatManager(
        EWeaponName weaponName)
    {
        if (gameStatSO == null)
            return null;

        var weaponData =
            gameStatSO.GetWeaponByType(
                weaponName
            );

        return weaponData.statManager;
    }


    // =========================================================
    // EQUIPPED WEAPON EFFECTS
    // =========================================================

    public void ReduceCoolodwnOfFirstWeapon(
        float reductionValue,
        float debuffDuration)
    {
        if (activeWeapons == null ||
            activeWeapons.Count <= 0)
        {
            return;
        }

        WeaponBase weapon =
            activeWeapons[0];

        if (weapon == null)
            return;

        CooldownDebuff appliedDebuff =
            weapon.gameObject.AddComponent<
                CooldownDebuff
            >();

        appliedDebuff.Init(
            reductionValue,
            debuffDuration
        );
    }
}