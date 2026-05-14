using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DynamicEnemyPooler
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly int maxPoolSize;
    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private readonly List<GameObject> activeObjects = new List<GameObject>();

    public int ActiveCount => activeObjects.Count;
    public int PooledCount => pool.Count;
    public int TotalCount => ActiveCount + PooledCount;
    public bool CanExpand => maxPoolSize == 0 || TotalCount < maxPoolSize;

    public DynamicEnemyPooler(GameObject prefab, int initialSize, int maxPoolSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.maxPoolSize = maxPoolSize;

        // Pre-instantiate initial pool
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewInstance();
        }
    }

    private GameObject CreateNewInstance()
    {
        GameObject obj = Object.Instantiate(prefab, parent);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject Get()
    {
        GameObject obj = null;

        // Try to get from pool first
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        // If pool is empty and we can expand, create new instance
        else if (CanExpand)
        {
            obj = CreateNewInstance();
            if (maxPoolSize > 0 && TotalCount >= maxPoolSize)
            {
                Debug.LogWarning($"[DynamicEnemyPooler] Pool for '{prefab.name}' reached max size ({maxPoolSize})!");
            }
        }
        // If pool is empty and can't expand, recycle the oldest active enemy
        else
        {
            Debug.LogWarning($"[DynamicEnemyPooler] Pool for '{prefab.name}' exhausted! Recycling oldest active enemy.");
            obj = RecycleOldestActive();
        }

        if (obj != null)
        {
            activeObjects.Add(obj);
        }

        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
    }

    private GameObject RecycleOldestActive()
    {
        if (activeObjects.Count == 0) return null;

        GameObject oldest = activeObjects[0];

        // Try to return it properly if it has enemy component
        BaseEnemyRefactor enemy = oldest.GetComponent<BaseEnemyRefactor>();
        if (enemy != null)
        {
            // Force return to pool
            EnemyManager.Instance?.DespawnEnemy(enemy);
            return oldest;
        }

        // If no enemy component, just deactivate and return
        oldest.SetActive(false);
        activeObjects.Remove(oldest);
        return oldest;
    }

    public void ClearPool()
    {
        // Return all active objects to pool first
        foreach (var obj in activeObjects.ToArray())
        {
            ReturnToPool(obj);
        }

        // Destroy all objects in the pool
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)
                Object.Destroy(obj);
        }

        activeObjects.Clear();
    }

    public void DestroyAll()
    {
        ClearPool();
    }

    /// <summary>
    /// Returns info about the pool state
    /// </summary>
    public string GetPoolInfo()
    {
        return $"Active: {ActiveCount}, Pooled: {PooledCount}, Total: {TotalCount}" +
               (maxPoolSize > 0 ? $", Max: {maxPoolSize}" : ", Unlimited");
    }
}