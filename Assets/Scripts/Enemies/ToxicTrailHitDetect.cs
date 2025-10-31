using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicTrailHitDetect : MonoBehaviour
{
    public int damagePerHit = 2;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Player Hit");

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            HealthSystem playerHealth = other.gameObject.GetComponent<HealthSystem>();
            if(playerHealth != null)
            {
                playerHealth.Damage(damagePerHit);
            }
        }
    }

    private void OnParticleTrigger()
    {
        
    }
}
