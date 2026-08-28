using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public bool isDOT = false;
    public float dotDuration = 2f;
    public float _damage_interval=.3f;

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();
    //    if (enemy != null)
    //        enemy.GetComponent<HealthSystem>().Damage((int)damage);

    //    Destroy(gameObject);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);

        BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();
        if (enemy != null)
        {
            if (isDOT)
            {
                enemy.GetComponent<HealthSystem>().TakeDamageOverTime(dotDuration, _damage_interval, (int)damage);
            }
            else
                enemy.GetComponent<HealthSystem>().Damage((int)damage);
        }

    }
}
