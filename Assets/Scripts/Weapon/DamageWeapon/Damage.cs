using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage = 1f;
    public bool destroyOnCollision;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage((int)damage);
            }
            if (destroyOnCollision)
                Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HealthSystem enemy = other.gameObject.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                enemy.Damage((int)damage);
            }
            if (destroyOnCollision)
                Destroy(gameObject);
        }
    }
}
