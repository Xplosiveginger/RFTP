using System.Collections;
using System.Collections.Generic;
using System.Linq;  // SINGLE using System.Linq
using UnityEngine;

/// <summary>
/// Singleton StatManager - Central hub for all player stats
/// </summary>
public class StatManager : MonoBehaviour
{
    public static StatManager Instance { get; private set; }

    [Header("Stats Configuration")]
    [SerializeField] public List<Stat> statList = new List<Stat>();

    private Dictionary<EStatType, List<(float modifier, float duration)>> tempModifiers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStats()
    {
        foreach (Stat stat in statList)
        {
            if (stat != null)
            {
                stat.Init();
                Debug.Log($"Initialized stat: {stat.statName} = {stat.currentValue}");
            }
        }
    }

    public Stat GetStat(EStatType statType)
    {
        Stat stat = statList.FirstOrDefault(s => s != null && s.statName == statType);
        if (stat == null)
        {
            Debug.LogWarning($"StatManager: Stat {statType} not found!");
        }
        return stat;
    }

    public void ApplyTemporaryModifier(EStatType statType, float modifier, float duration)
    {
        StartCoroutine(TemporaryModifierCoroutine(statType, modifier, duration));
    }

    private IEnumerator TemporaryModifierCoroutine(EStatType statType, float modifier, float duration)
    {
        Stat stat = GetStat(statType);
        if (stat == null) yield break;

        if (!tempModifiers.ContainsKey(statType))
            tempModifiers[statType] = new List<(float, float)>();

        tempModifiers[statType].Add((modifier, duration));
        Debug.Log($"Applied temp modifier: {statType} +{modifier}% for {duration}s");

        stat.ApplyModifier(modifier);
        yield return new WaitForSeconds(duration);

        stat.RevertModifier(modifier);
        Debug.Log($"Reverted temp modifier: {statType} -{modifier}%");

        tempModifiers[statType].RemoveAll(m => m.duration <= 0);
        if (tempModifiers[statType].Count == 0)
            tempModifiers.Remove(statType);
    }
}
