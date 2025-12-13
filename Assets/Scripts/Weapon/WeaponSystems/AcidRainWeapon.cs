using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRainWeapon : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectileCount = 10;  // how many to spawn once

    [Header("Spawn Timing")]
    public bool spawnOnEnable = true;

    void OnEnable()
    {
        if (spawnOnEnable)
        {
            SpawnProjectiles();
        }
    }

    public void SpawnProjectiles()
    {
        if (projectilePrefab == null) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            if (enemy == null) continue;

            Vector3 spawnPos = enemy.transform.position;
            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
