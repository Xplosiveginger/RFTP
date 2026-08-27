using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreSecDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 1f; // Damage applied every frame
    public float damageInterval = 1f;

    public float deathAfterSec = 3f;
    public Vector2 aoeSize;
    public float size;
    [SerializeField] private LayerMask damageLM;
    private float lastSize;

    private readonly List<HealthSystem> enemiesInRange = new List<HealthSystem>();
    private Dictionary<HealthSystem, float> damageBuffer = new Dictionary<HealthSystem, float>();

    public void Initialize(float damage, float size)
    {
        this.damage = damage;
        this.size = size;

        Vector2 scale = transform.localScale;

        scale = scale * size;

        transform.localScale = scale;

        StartCoroutine(DamageEnemies());
    }

    IEnumerator DamageEnemies()
    {
        while (true)
        {
            foreach (HealthSystem hs in enemiesInRange)
            {
                hs.Damage((int)damage);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                damageBuffer[enemy] = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
                damageBuffer.Remove(enemy);
            }
        }
    }
}
