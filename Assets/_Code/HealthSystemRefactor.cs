using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystemRefactor : MonoBehaviour
{
    [SerializeField] StatManager statManager;
    public float baseValue;

    public bool isDead;


    public static event Action<float> OnDamageRecieved;
    public static event Action OnDeath;
    public static event Action OnHealthReset;

    public void Initialize()
    {
        baseValue = statManager.GetStat(EStatType.Health).baseValue;
    }

    private float GetHealth()
    {
        return statManager.GetStat(EStatType.Health).currentValue;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        float currentValue = GetHealth();
        currentValue -= damageAmount;
        currentValue = Mathf.Max(0, currentValue);
        statManager.GetStat(EStatType.Health).currentValue = currentValue;

        if (currentValue <= 0)
        {
            // Trigger Death Animation
        }
        else
        {
            // Play Hurt SFX And Animations
            OnDamageRecieved?.Invoke(currentValue);
        }
    }

    public void ResetHealth()
    {
        statManager.GetStat(EStatType.Health).currentValue = statManager.GetStat(EStatType.Health).baseValue;
        OnHealthReset?.Invoke();
    }
}
