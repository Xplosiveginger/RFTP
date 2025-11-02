using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicTrailHitDetect : MonoBehaviour
{
    public int damagePerHit = 2;
    public float damageDuration = 5f;
    public float damageInterval;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Player Hit");

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController2D player = other.gameObject.GetComponent<PlayerController2D>();
            if (player != null)
            {
                if (!player.inflicted)
                {
                    player.inflicted = true;
                    player.health.TakeDamageOverTime(damageDuration, damageInterval, damagePerHit);
                }
            }
        }
    }

    private void OnParticleTrigger()
    {
        
    }
}
