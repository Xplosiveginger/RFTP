using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrowingWeapon : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public int projectilesPerThrow = 3;
    public float throwForce = 5f;

    [Header("Fire Rate")]
    public float fireInterval = 2f;

    private float nextFireTime;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            ThrowProjectiles();
            nextFireTime = Time.time + fireInterval;
        }
    }

    void ThrowProjectiles()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        for (int i = 0; i < projectilesPerThrow; i++)
        {
            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            if (enemy == null) continue;

            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            Vector2 dir = (enemy.transform.position - transform.position).normalized;

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(dir * throwForce, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
