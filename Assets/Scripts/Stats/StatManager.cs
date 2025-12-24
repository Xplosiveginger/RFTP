using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public bool isDamagable;
    public HealthSystem health;

    public List<StatDataSO> statDataList;
    public List<Stat> statList;

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

    private void OnEnable()
    {
        if(isDamagable) health.OnHealthChanged += UpdateHealthCurrentValue;

        if (statList.Count == 0) return;
        foreach (Stat stat in statList)
        {
            stat.OnCurrentValueChanged += OnCurrentValueChangedHandled;
        }
    }

    private void OnDisable()
    {
        if(isDamagable) health.OnHealthChanged -= UpdateHealthCurrentValue;

        foreach (Stat stat in statList)
        {
            stat.OnCurrentValueChanged -= OnCurrentValueChangedHandled;
        }
    }

    public void InitializeStats()
    {
        if(statDataList.Count == 0)
            return;

        statList.Clear();
        foreach (StatDataSO statData in statDataList)
        {
            Stat stat = statData.Init();
            statList.Add(stat);
        }
    }

    public Stat TryGetStat(EStatType statName)
    {
        Stat stat = GetStat(statName);
        if (stat == null)
        {
            Debug.Log($"{statName} stat not present in {transform.gameObject.name}.");
            return null; 
        }

        return stat;
    }

    /// <summary>
    /// Returns the Stat class for which the statName matches.
    /// </summary>
    /// <param name="statName">The name of the stat you want to get.</param>
    public Stat GetStat(EStatType statName) // Add a TryGetStat() which checks the obtained value.
    {
        return statList.Find(stat => stat.statName == statName);
    }

    public List<Stat> GetAllStats()
    {
        return statList;
    }

    /// <summary>
    /// Modifies the Current value of the provided stat by a certain amount.
    /// </summary>
    /// <param name="statName">The stat to modify.</param>
    /// <param name="value">The amount by which the current value should be changed.</param>
    /// <param name="subtract">Should the value be subtracted or added to the current value.</param>
    public void ModifyStatValue(EStatType statName, float value, bool subtract)
    {
        Stat stat = GetStat(statName);
        if (stat == null) return;

        stat.currentValue = (!subtract)? stat.currentValue + value : stat.currentValue - value;
        OnValueChanged?.Invoke(stat);
        OnStatChanged?.Invoke();
    }

    /// <summary>
    /// Modifies the Current value of the provided stat by a certain percentage.
    /// </summary>
    /// <param name="statName">The stat to modify.</param>
    /// <param name="modifier">The percentage by which the current value should be changed.</param>
    /// <param name="subtract">Should the value be subtracted or added to the current value.</param>
    public void ModifyStat(EStatType statName, float modifier, bool subtract)
    {
        foreach (Stat stat in statList)
        {
            if(stat.statName == statName)
            {
                stat.ApplyModifier(modifier, subtract);
                break;
            }
            continue;
        }
    }

    public void ModifyHealthStat(float modifier, bool subtract)
    {
        foreach (Stat stat in statList)
        {
            if (stat.statName == EStatType.Health)
            {
                stat.ApplyHealthModifier(modifier, subtract);
            }
        }
    }

    private void UpdateHealthCurrentValue(float value)
    {
        GetStat(EStatType.Health).currentValue = value;

    }

    private void OnCurrentValueChangedHandled(Stat stat)
    {
        InvokeOnStatChangedEvents(stat.statName);
        OnValueChanged?.Invoke(stat);
        OnStatChanged?.Invoke();
    }

    private void OnMaxValueChangedHandled()
    {
        Debug.Log("Health Max Value Changed");
        OnHealthChanged?.Invoke();
    }

    private void InvokeOnStatChangedEvents(EStatType statName)
    {
        switch (statName)
        {
            case EStatType.MoveSpeed: OnMoveSpeedChanged?.Invoke();
                break;
            case EStatType.Health: OnHealthChanged?.Invoke();
                break;
            case EStatType.AttackCooldown: OnAttackCooldownChanged?.Invoke();
                break;
            case EStatType.ActiveDuration: OnActiveDurationChanged?.Invoke();
                break;
            case EStatType.ProjectileCount: OnProjectileCountChanged?.Invoke();
                break;
            case EStatType.ProjectileSpeed: OnProjectileSpeedChanged?.Invoke();
                break;
            case EStatType.Damage: OnDamageChanged?.Invoke();
                break;
            case EStatType.AOESize: OnAOESizeChanged?.Invoke();
                break;
            default: Debug.LogError($"Stat changed event for {statName} stat not defined.\nPlease define the event.");
                break;
        }
    } 
}
