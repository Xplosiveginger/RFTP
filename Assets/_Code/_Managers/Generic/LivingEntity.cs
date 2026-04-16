using System;
using UnityEngine;
using UnityEngine.Events;


public abstract class LivingEntity : MonoBehaviour, IHealth
{
    protected float health;
    public float startingHealth;
    protected virtual void Start()
    {   
        health = startingHealth;
    }

    public void TakeDamage()
    {
        
    }


    protected virtual void Die() { }
    public void SelfDestruct()
    {
        Die();
    }

    
}
