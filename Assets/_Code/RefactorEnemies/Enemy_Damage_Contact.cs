using UnityEngine;

public class Enemy_Damage_Contact : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Amount of damage dealt per tick.")]
    [SerializeField] private int damageAmount = 10;

    [Tooltip("Time between damage ticks in seconds.")]
    [SerializeField] private float damageInterval = 1f;

    private HealthSystem playerHealth;
    private float damageTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<HealthSystem>();
            damageTimer = damageInterval;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerHealth != null && !playerHealth.IsDead)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                playerHealth.Damage(damageAmount);
                damageTimer = 0f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
            damageTimer = 0f;
        }
    }
}