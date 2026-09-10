using UnityEngine;

public class LI_Ion_battery_projectile : Projectile
{
    [Header("Projectile Settings")]
    public ParticleSystem blastFx;
    public float blastRadius;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        bool hasatleastOneenemy = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);
        
        foreach (Collider2D collider in colliders)
        {
            BaseEnemyRefactor enemy = collider.GetComponent<BaseEnemyRefactor>();
            if (enemy != null)
            {
                hasatleastOneenemy = true;
                enemy.GetComponent<HealthSystem>().Damage((int)damage);
            }
        }
        if (hasatleastOneenemy)
        {
            Instantiate(blastFx, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
