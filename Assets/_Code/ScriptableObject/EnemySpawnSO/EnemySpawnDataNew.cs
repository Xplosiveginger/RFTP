using System;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class EnemySpawnDataNew
{
    [PreviewField(50)]
    [HideLabel]
    [HorizontalGroup("Enemy", 50)]
    public GameObject enemyPrefab;

    [VerticalGroup("Enemy/Info")]
    [LabelText("Weight"), LabelWidth(50)]
    [Tooltip("Relative weight for spawning this enemy type. Higher weight = more likely to spawn.")]
    [Range(0f, 1f)]
    [OnValueChanged("OnWeightChanged")]
    public float weight = 0.5f;

    [VerticalGroup("Enemy/Info")]
    [LabelText("Initial Pool"), LabelWidth(70)]
    [Tooltip("Initial pool size. Pool will grow dynamically if needed.")]
    [MinValue(1)]
    public int initialPoolSize = 10;

    [VerticalGroup("Enemy/Info")]
    [LabelText("Max Pool"), LabelWidth(70)]
    [Tooltip("Maximum pool size. Set to 0 for unlimited growth.")]
    [MinValue(0)]
    public int maxPoolSize = 50;

    private void OnWeightChanged()
    {
        // This will trigger validation in the parent SpawnPhaseSO
    }

    public string EnemyName => enemyPrefab != null ? enemyPrefab.name : "None";
}