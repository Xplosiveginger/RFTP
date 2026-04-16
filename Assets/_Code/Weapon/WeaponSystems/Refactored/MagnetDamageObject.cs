using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetDamageObject : MonoBehaviour
{
    [SerializeField  ] private float damage;
    [SerializeField  ] private float size;
    [SerializeField] private float damageInterval;
    [SerializeField] private MagnetRefactored magnet;
    [SerializeField] private LayerMask damageLM;

    public bool drawHitBox;
    private Coroutine damageRoutine;

    private void UpdateStats()
    {
        damage = magnet.GetStatValue(EStatType.Damage);
        size = magnet.GetStatValue(EStatType.AOESize);
    }

    private void OnEnable()
    {
        UpdateStats();

        damageRoutine = StartCoroutine(DamageEnemies());
    }

    private void OnDisable()
    {
        if(damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }

    IEnumerator DamageEnemies()
    {
        while (true)
        {
            Collider2D[] enemies = Physics2D.OverlapBoxAll(transform.position, new Vector2(size, size), 0.0f, damageLM);

            foreach (Collider2D collider in enemies)
            {
                BaseEnemyRefactor enemy = collider.GetComponent<BaseEnemyRefactor>();

                if(enemy != null)
                {
                    enemy.GetComponent<HealthSystem>().Damage((int)damage);
                }
                else
                    continue;
            }

            yield return new WaitForSeconds(damageInterval); 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
    }
}