using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dustbin : MonoBehaviour
{
    [Serializable]
    public struct SpawnItem
    {
        public GameObject prefab;

        [Range(0f, 100f)]
        public float probability;
    }

    [Header("Damage Settings")]
    public int damageAmount = 10;

    [Header("Spawn Settings")]
    [Tooltip("Add items and their probability here. Total probability should equal 100.")]
    public List<SpawnItem> spawnItems = new List<SpawnItem>();

    public AudioClip furnitureSound;

    private GameStat_SO GameStat_SO;

    private void Start()
    {
        GameStat_SO = PersistentObject.Instance.GameStat_SO;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SpawnRandomItem();

        GameStat_SO.RegisterBreakablesDestroyed();
        GlobalAudioPlayer.Instance.PlayAudio(furnitureSound);

        Destroy(gameObject);

        // Make sure this function does not continue
        // after the dustbin has been triggered.
        return;
    }

    private void SpawnRandomItem()
    {
        if (spawnItems == null || spawnItems.Count == 0)
            return;

        float totalProbability = 0f;

        // Calculate total probability.
        foreach (SpawnItem item in spawnItems)
        {
            totalProbability += item.probability;
        }

        if (totalProbability <= 0f)
            return;

        // Roll between 0 and total probability.
        float randomValue = Random.Range(0f, totalProbability);

        float currentProbability = 0f;

        foreach (SpawnItem item in spawnItems)
        {
            currentProbability += item.probability;

            if (randomValue <= currentProbability)
            {
                // Null prefab means "nothing".
                if (item.prefab != null)
                {
                    Instantiate(
                        item.prefab,
                        transform.position,
                        Quaternion.identity
                    );
                }

                // IMPORTANT:
                // Return immediately so only ONE result can spawn.
                return;
            }
        }
    }
}