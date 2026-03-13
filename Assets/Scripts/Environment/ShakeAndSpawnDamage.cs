using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ShakeAndSpawnDamage : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 2f;
    public string playerTag = "Player";

    [Header("Shake Settings")]
    public float shakeDuration = 3f;
    public float shakeStrength = 10f;
    public int shakeVibrato = 20;

    [Header("Damage Settings")]
    public GameObject damageParticlePrefab;
    public float damageDuration = 2f;
    public int damageAmount = 10;

    public float damageCheckInterval = 0.5f;
    public float damageParticleRadius = 1f;

    [Header("Spawn Offset")]
    [Tooltip("Offset position for the damage particle (relative to this object)")]
    public Vector3 damageParticleOffset = Vector3.zero;

    [Header("Chance Settings")]
    [Tooltip("Initial chance to start shake and spawn (0 to 1, e.g. 0.4 for 40%)")]
    public float initialChance = 0.4f;
    [Tooltip("Chance multiplier added on each failed attempt (e.g. 0.05 for 5%)")]
    public float chanceMultiplier = 0.05f;

    [Header("Slow Settings")]
    [Tooltip("Multiplier to slow player speed (e.g. 0.5 means 50% speed)")]
    public float slowMultiplier = 0.5f;
    [Tooltip("Duration of slow in seconds")]
    public float slowDuration = 1f;

    private float currentChance;
    private bool playerDetectedPrevFrame = false;
    private bool hasSpawned = false;
    private Sequence shakeSequence;

    void Awake()
    {
        currentChance = initialChance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw offset preview for spawn position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + damageParticleOffset, damageParticleRadius);
    }

    void Update()
    {
        bool playerDetectedNow = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag(playerTag))
            {
                playerDetectedNow = true;
                break;
            }
        }

        if (playerDetectedNow && !playerDetectedPrevFrame)
        {
            if (Random.value < currentChance)
            {
                StartShakeAndSpawn();
                currentChance = initialChance;
            }
            else
            {
                currentChance += chanceMultiplier;
                currentChance = Mathf.Clamp01(currentChance);
            }
        }

        playerDetectedPrevFrame = playerDetectedNow;
    }

    void StartShakeAndSpawn()
    {
        hasSpawned = true;

        shakeSequence = DOTween.Sequence();
        shakeSequence.Append(transform.DOPunchRotation(new Vector3(0, 0, shakeStrength), shakeDuration, shakeVibrato));
        shakeSequence.OnComplete(() =>
        {
            SoundFXManager.instance.PlaySmokeSFX(transform, 1);

            SpawnDamageParticle();
        });
    }

    void SpawnDamageParticle()
    {
        if (damageParticlePrefab != null)
        {
            Vector3 spawnPosition = transform.position + damageParticleOffset;
            GameObject particle = Instantiate(damageParticlePrefab, spawnPosition, Quaternion.identity);
            Destroy(particle, damageDuration);
            StartCoroutine(DealDamageOverTime(particle.transform));
        }
    }

    IEnumerator DealDamageOverTime(Transform particleTransform)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            if (particleTransform == null) yield break;

            Collider2D[] hits = Physics2D.OverlapCircleAll(particleTransform.position, damageParticleRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag(playerTag))
                {
                    var health = col.GetComponent<HealthSystem>();
                    if (health != null && !health.IsDead)
                    {
                        health.Damage(damageAmount);

                        var playerController = col.GetComponent<PlayerController2D>();
                        if (playerController != null)
                        {
                            playerController.ApplyTemporarySpeedModifier(slowMultiplier, slowDuration);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(damageCheckInterval);
            elapsed += damageCheckInterval;
        }
        hasSpawned = false;
    }
}
