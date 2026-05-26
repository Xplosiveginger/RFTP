using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SpawnPhase", menuName = "Enemy Spawning/Spawn Phase")]
public class SpawnPhaseSO : ScriptableObject
{
    [Title("Phase Configuration")]

    [HorizontalGroup("PhaseInfo")]
    [VerticalGroup("PhaseInfo/Left")]
    [LabelText("Start Time (s)")]
    [Tooltip("When this phase starts (in seconds).")]
    public float startTime;

    [VerticalGroup("PhaseInfo/Right")]
    [LabelText("Spawn Gap")]
    [Tooltip("Time interval between spawns in seconds.")]
    [MinValue(0.1f)]
    [SuffixLabel("seconds", Overlay = true)]
    public float spawnGap = 1f;

    [VerticalGroup("PhaseInfo/Right")]
    [LabelText("Spawn Count")]
    [Tooltip("Number of enemies to spawn per interval.")]
    [MinValue(1)]
    [SuffixLabel("enemies", Overlay = true)]
    public int spawnCount = 2;

    [Title("Enemy Types")]
    [InfoBox("Total weight should be greater than 0 for enemies to spawn. " +
             "Weights are relative - they don't need to sum to 1.0.",
             InfoMessageType.Info)]
    [ListDrawerSettings(
        ShowIndexLabels = true,
        ListElementLabelName = "EnemyName",
        Expanded = true,
        DraggableItems = true,
        HideAddButton = false,
        HideRemoveButton = false,
        NumberOfItemsPerPage = 5
    )]
    [ValidateInput("ValidateWeights", "Total weight must be greater than 0!")]
    public List<EnemySpawnDataNew> enemiesToSpawn = new List<EnemySpawnDataNew>();

    /// <summary>
    /// Returns the total weight of all enemies in this phase
    /// </summary>
    [ShowInInspector, ReadOnly, TitleGroup("Stats")]
    [LabelText("Total Weight")]
    public float TotalWeight
    {
        get
        {
            float total = 0f;
            foreach (var enemy in enemiesToSpawn)
                total += enemy.weight;
            return total;
        }
    }

    [ShowInInspector, ReadOnly, TitleGroup("Stats")]
    [LabelText("Enemy Types")]
    public int EnemyTypeCount => enemiesToSpawn.Count;

    [ShowInInspector, ReadOnly, TitleGroup("Stats")]
    [LabelText("Spawn Rate")]
    public string SpawnRateInfo => $"{spawnCount} enemies every {spawnGap}s";

    [ShowInInspector, ReadOnly, TitleGroup("Stats")]
    [LabelText("Max Potential Spawn Rate")]
    public string MaxSpawnRateInfo => $"{spawnCount / spawnGap * 60:F1} enemies/min";

    private bool ValidateWeights()
    {
        return TotalWeight > 0f || enemiesToSpawn.Count == 0;
    }

    private void OnValidate()
    {
        if (TotalWeight <= 0 && enemiesToSpawn.Count > 0)
        {
            Debug.LogWarning($"[{name}]: Total weight is 0! No enemies will spawn in this phase.");
        }
    }
}