using UnityEngine;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected bool canDamagePlayerOnContact = false;
    public float damageOnContact = 10f;
    public float damage = 5f;
    public float damageInterval = 1f; // Time between damage ticks
    [SerializeField] protected bool canDamageSelfOnContact = false;

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f; // Degrees per second

    [Header("XP Drop")]
    public int expDrop = 10;
    public GameObject XpOrbPrefab;

    private bool canDamage = true;

    void Update()
    {
        // Rotate continuously around Z-axis
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canDamage)
        {
            IDamagable damagable = collision.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            DoDamageToPlayerOnCollision(other); 
            if (canDamageSelfOnContact)
            {
                TakeDamageToSelfOnCollision(other);
            }
        }
    }

    protected virtual void DoDamageToPlayerOnCollision(Collision2D player)
    {
        
    }

    protected virtual void TakeDamageToSelfOnCollision(Collision2D player)
    {

    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageInterval);
        canDamage = true;
    }
}
