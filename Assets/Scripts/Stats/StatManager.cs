using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public HealthSystem health;

    public List<Stat> statList;

    public event Action<Stat> OnValueChanged;
    public event Action OnMoveSpeedChanged;
    public event Action OnHealthChanged;
    public event Action OnCooldownChanged;

    public event Action OnAttackCooldownChanged;
    public event Action OnActiveDurationChanged;
    public event Action OnProjectileCountChanged;
    public event Action OnProjectileSpeedChanged;
    public event Action OnDamageChanged;
    public event Action OnAOESizeChanged;

    private void Awake()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        if(statList.Count == 0)
            return;

        foreach (Stat stat in statList)
        {
            stat.Init();

            stat.OnCurrentValueChanged += OnCurrentValueChangedHandled;
        }
    }

    /// <summary>
    /// Returns the Stat class for which the statName matches.
    /// </summary>
    /// <param name="statName">The name of the stat you want to get.</param>
    public Stat GetStat(EStatType statName)
    {
        return statList.Find(stat => stat.statName == statName);
    }

    public List<Stat> GetAllStats()
    {
        return statList;
    }

    public void ModifyStat(EStatType statName, float modifier)
    {
        foreach (Stat stat in statList)
        {
            if(stat.statName == statName)
            {
                stat.ApplyModifier(modifier);
                break;
            }
            continue;
        }
    }

    public void ModifyHealthStat(float modifier)
    {
        foreach (Stat stat in statList)
        {
            if (stat.statName == EStatType.Health)
            {
                stat.ApplyHealthModifier(modifier);
            }
        }
    }

    private void OnCurrentValueChangedHandled(Stat stat)
    {
        InvokeOnStatChangedEvents(stat.statName);
        OnValueChanged?.Invoke(stat);
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
