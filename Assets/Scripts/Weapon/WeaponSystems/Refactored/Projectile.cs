using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [DisplayOnly] public float damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();
        if (enemy != null)
            enemy.GetComponent<HealthSystem>().Damage((int)damage);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();
        if (enemy != null)
            enemy.GetComponent<HealthSystem>().Damage((int)damage);

        Destroy(gameObject);
    }
}
