using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;

[RequireComponent(typeof(StatManager))]
public class Enemy_Damage_Contact : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Amount of damage dealt per tick.")]
    [SerializeField,ReadOnly] private float  DamageAmount = 10;

    [Tooltip("Time between damage ticks in seconds.")]
    [SerializeField] private float damageInterval = 1f;

    private StatManager StatManager;
    private HealthSystem playerHealth;
    private float damageTimer;

    private void Start()
    {
        StatManager = GetComponent<StatManager>();
        if (StatManager == null)
        {
            Debug.LogError("StatManager is missing");
        }
        else
        {
            StatManager.OnStatChanged += () =>
            {
                UpdateStats();
            };
        }

        DamageAmount = StatManager.GetStat(EStatType.Damage).currentValue;

    }

    void UpdateStats()
    {
        DamageAmount = StatManager.GetStat(EStatType.Damage).currentValue;
    }

    public void ModifyDamageInterval(float amount)
    {
        if(amount>0)
            damageInterval=amount;
    }
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
                playerHealth.Damage((int)DamageAmount);
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