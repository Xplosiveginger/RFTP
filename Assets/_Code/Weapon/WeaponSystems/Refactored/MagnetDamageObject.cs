using System.Collections;
using UnityEngine;

public class MagnetDamageObject : MonoBehaviour
{
    [Header("Base Hitbox")]
    [SerializeField] private Vector2 baseAOESize = Vector2.one;

    [Header("Runtime Stats")]
    [SerializeField] private float damage;
    [SerializeField] private Vector2 aoeSize;
    [SerializeField] private float size;
    [SerializeField] private float damageInterval;

    [Header("References")]
    [SerializeField] private MagnetRefactored magnet;
    [SerializeField] private LayerMask damageLM;

    [Header("Debug")]
    public bool drawHitBox;

    private Coroutine damageRoutine;


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    private void Awake()
    {
        if (magnet == null)
        {
            magnet = GetComponentInParent<MagnetRefactored>();
        }
    }


    private void OnEnable()
    {
        if (magnet == null)
        {
            Debug.LogError(
                $"MagnetDamageObject '{name}' " +
                $"could not find MagnetRefactored."
            );

            return;
        }

        // Get the latest effective values immediately.
        UpdateStats();

        damageRoutine = StartCoroutine(DamageEnemies());
    }


    private void OnDisable()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
            damageRoutine = null;
        }
    }


    // =========================================================
    // STAT UPDATE
    // =========================================================

    private void UpdateStats()
    {
        if (magnet == null)
            return;

        /*
         * IMPORTANT:
         *
         * MagnetRefactored already contains the FINAL effective
         * weapon values.
         *
         * Example:
         *
         * Magnet own damage      = 12
         * Player damage modifier = 1.10
         *
         * Magnet.damage          = 13.2
         *
         * We simply read 13.2 here.
         */

        damage = magnet.GetStatValue(EStatType.Damage);

        size = magnet.GetStatValue(EStatType.AOESize);


        /*
         * Always calculate from the original hitbox.
         *
         * This prevents:
         *
         * 2 × 1.1 × 1.1 × 1.1
         *
         * when stats change multiple times.
         */

        aoeSize = baseAOESize * size;
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    private IEnumerator DamageEnemies()
    {
        while (true)
        {
            /*
             * Read the latest effective weapon values before
             * each damage cycle.
             *
             * This also makes this object resilient if the
             * Magnet's stats change while it is active.
             */

            UpdateStats();


            Collider2D[] enemies =
                Physics2D.OverlapBoxAll(
                    transform.position,
                    aoeSize,
                    0f,
                    damageLM
                );


            foreach (Collider2D collider in enemies)
            {
                BaseEnemyRefactor enemy =
                    collider.GetComponent<BaseEnemyRefactor>();


                if (enemy == null)
                    continue;


                HealthSystem health =
                    enemy.GetComponent<HealthSystem>();


                if (health == null)
                    continue;


                health.Damage(
                    Mathf.RoundToInt(damage)
                );
            }


            yield return new WaitForSeconds(
                damageInterval
            );
        }
    }


    // =========================================================
    // GIZMOS
    // =========================================================

    private void OnDrawGizmos()
    {
        if (!drawHitBox)
            return;


        Gizmos.DrawWireCube(
            transform.position,
            aoeSize
        );
    }
}