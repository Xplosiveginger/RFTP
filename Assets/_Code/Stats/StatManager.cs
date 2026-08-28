using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    [Header("Health")]
    public bool isDamagable;
    public HealthSystem health;

    [Header("Stats")]
    public List<StatDataSO> statDataList;
    public List<Stat> statList = new List<Stat>();

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<Stat> OnValueChanged;
    public event Action OnStatChanged;

    public event Action OnMoveSpeedChanged;
    public event Action OnHealthChanged;
    public event Action OnCooldownChanged;

    public event Action OnAttackCooldownChanged;
    public event Action OnActiveDurationChanged;
    public event Action OnProjectileCountChanged;
    public event Action OnProjectileSpeedChanged;
    public event Action OnDamageChanged;
    public event Action OnAOESizeChanged;


    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        InitializeStats();
    }

    private void OnEnable()
    {
        if (isDamagable && health != null)
        {
            health.OnHealthChanged += UpdateHealthCurrentValue;
        }

        SubscribeToStatEvents();
    }

    private void OnDisable()
    {
        if (isDamagable && health != null)
        {
            health.OnHealthChanged -= UpdateHealthCurrentValue;
        }

        UnsubscribeFromStatEvents();
    }


    /// <summary>
    /// Creates the runtime Stat objects from the StatDataSO list.
    /// </summary>
    public void InitializeStats()
    {
        UnsubscribeFromStatEvents();

        statList.Clear();

        if (statDataList == null || statDataList.Count == 0)
        {
            Debug.LogWarning(
                $"No StatDataSO entries found on {gameObject.name}."
            );

            return;
        }

        // ==========================================
        // CREATE BASE STATS
        // ==========================================

        foreach (StatDataSO statData in statDataList)
        {
            if (statData == null)
                continue;

            Stat stat = statData.Init();

            if (stat != null)
            {
                statList.Add(stat);
            }
        }

        // ==========================================
        // APPLY PERMANENT SHOP MODIFIERS
        // ==========================================

        ApplySavedShopModifiers();

        // ==========================================
        // SUBSCRIBE
        // ==========================================

        SubscribeToStatEvents();
    }


    private void ApplySavedShopModifiers()
    {
        if (GameSaveSystem.instance == null)
        {
            Debug.Log("No GameSaveSystem found. Using base stats.");
            return;
        }

        foreach (GameSaveSystem.SavedShopItem savedItem
                 in GameSaveSystem.instance.GetSavedShopItems())
        {
            Stat stat = GetStat(savedItem.affectedStat);

            if (stat == null)
            {
                Debug.LogWarning(
                    $"Saved shop item affects {savedItem.affectedStat}, " +
                    $"but that stat does not exist on {gameObject.name}."
                );

                continue;
            }

            Debug.Log(
                $"Applying shop modifier: " +
                $"{savedItem.affectedStat} +" +
                $"{savedItem.modifier}" +
                (savedItem.isPercentage ? "%" : "")
            );

            stat.ApplyStartModifier(
                savedItem.modifier,
                savedItem.isPercentage
            );
        }
    }


    private void SubscribeToStatEvents()
    {
        if (statList == null)
            return;

        foreach (Stat stat in statList)
        {
            if (stat != null)
            {
                stat.OnCurrentValueChanged += OnCurrentValueChangedHandled;
            }
        }
    }


    private void UnsubscribeFromStatEvents()
    {
        if (statList == null)
            return;

        foreach (Stat stat in statList)
        {
            if (stat != null)
            {
                stat.OnCurrentValueChanged -= OnCurrentValueChangedHandled;
            }
        }
    }


    // =========================================================
    // GET STATS
    // =========================================================

    /// <summary>
    /// Returns the Stat matching the requested stat type.
    /// </summary>
    public Stat GetStat(EStatType statName)
    {
        if (statList == null)
            return null;

        return statList.Find(stat => stat.statName == statName);
    }


    /// <summary>
    /// Returns the Stat if it exists.
    /// Logs a warning if it doesn't.
    /// </summary>
    public Stat TryGetStat(EStatType statName)
    {
        Stat stat = GetStat(statName);

        if (stat == null)
        {
            Debug.LogWarning(
                $"{statName} stat not present in {gameObject.name}."
            );
        }

        return stat;
    }


    /// <summary>
    /// Returns all runtime stats.
    /// </summary>
    public List<Stat> GetAllStats()
    {
        return statList;
    }


    // =========================================================
    // MODIFY STATS
    // =========================================================

    /// <summary>
    /// Adds a flat value to a stat.
    ///
    /// Example:
    /// Damage +5
    /// ProjectileCount +1
    /// </summary>
    public void ModifyStatValue(EStatType statName, float value)
    {
        Stat stat = GetStat(statName);

        if (stat == null)
        {
            Debug.LogWarning(
                $"Cannot modify {statName}. Stat does not exist."
            );

            return;
        }

        stat.AddFlat(value);

        OnValueChanged?.Invoke(stat);
        OnStatChanged?.Invoke();
    }


    /// <summary>
    /// Applies a percentage modifier to a stat.
    ///
    /// Example:
    /// Damage +20%
    /// Health +30%
    /// MoveSpeed +10%
    /// </summary>
    public void ModifyStat(EStatType statName, float modifier)
    {
        Stat stat = GetStat(statName);

        if (stat == null)
        {
            Debug.LogWarning(
                $"Cannot modify {statName}. Stat does not exist."
            );

            return;
        }



        Debug.Log(
            $"Modifying {statName}: " +
            $"Before = {stat.currentValue}, " +
            $"Modifier = {modifier}%"
        );

        // Special handling for cooldown because a positive
        // modifier represents a reduction in cooldown.

        if(statName==EStatType.Health)
        {
            stat.ApplyHealthModifier(modifier);
        }
        else if (statName == EStatType.AttackCooldown)
        {
            stat.ApplyCooldownModifier(modifier);
        }
        else
        {
            stat.ApplyModifier(modifier);
        }

        Debug.Log(
            $"Modifying {statName}: " +
            $"After = {stat.currentValue}"
        );
    }


    // =========================================================
    // TEMPORARY MODIFIERS
    // =========================================================

    public void ApplyTemporaryStatModifier(
        EStatType statName,
        float modifier,
        float time)
    {
        StartCoroutine(
            ApplyTemporaryStatModifierCoroutine(
                statName,
                modifier,
                time
            )
        );
    }


    private IEnumerator ApplyTemporaryStatModifierCoroutine(
        EStatType statName,
        float modifier,
        float time)
    {
        Stat stat = GetStat(statName);

        if (stat == null)
            yield break;

        if (statName == EStatType.AttackCooldown)
        {
            stat.ApplyCooldownModifier(modifier);
        }
        else
        {
            stat.ApplyModifier(modifier);
        }

        yield return new WaitForSeconds(time);

        stat.RevertModifier(modifier);
    }


    // =========================================================
    // HEALTH
    // =========================================================

    private void UpdateHealthCurrentValue(float value)
    {
        Stat healthStat = GetStat(EStatType.Health);

        if (healthStat == null)
            return;

        healthStat.currentValue = value;

        OnValueChanged?.Invoke(healthStat);
        OnStatChanged?.Invoke();
    }


    /// <summary>
    /// Resets the player's current health to its max value.
    /// </summary>
    public void ResetHealthStatOnDeath()
    {
        Stat stat = GetStat(EStatType.Health);

        if (stat == null)
            return;

        stat.currentValue = stat.maxValue;

        OnValueChanged?.Invoke(stat);
        OnStatChanged?.Invoke();
    }


    // =========================================================
    // STAT CHANGE EVENTS
    // =========================================================

    private void OnCurrentValueChangedHandled(Stat stat)
    {
        if (stat == null)
            return;

        InvokeOnStatChangedEvents(stat.statName);

        OnValueChanged?.Invoke(stat);
        OnStatChanged?.Invoke();
    }


    private void InvokeOnStatChangedEvents(EStatType statName)
    {
        switch (statName)
        {
            case EStatType.MoveSpeed:
                OnMoveSpeedChanged?.Invoke();
                break;

            case EStatType.Health:
                OnHealthChanged?.Invoke();
                break;

            case EStatType.AttackCooldown:
                OnAttackCooldownChanged?.Invoke();
                break;

            case EStatType.ActiveDuration:
                OnActiveDurationChanged?.Invoke();
                break;

            case EStatType.ProjectileCount:
                OnProjectileCountChanged?.Invoke();
                break;

            case EStatType.ProjectileSpeed:
                OnProjectileSpeedChanged?.Invoke();
                break;

            case EStatType.Damage:
                OnDamageChanged?.Invoke();
                break;

            case EStatType.AOESize:
                OnAOESizeChanged?.Invoke();
                break;

            default:
                Debug.LogWarning(
                    $"Stat changed event for {statName} is not defined."
                );
                break;
        }
    }
}