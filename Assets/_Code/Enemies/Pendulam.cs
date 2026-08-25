using UnityEngine;
using UnityEngine.AI;

public class PendulumEnemy2D : MonoBehaviour
{
    [Header("Movement")]
    public float stoppingDistance = 5f;
    public float moveSpeed = 3.5f;

    [Header("Face")]
    [SerializeField] private GameObject faceObject;
    [SerializeField] private Sprite normalFaceSprite;
    [SerializeField] private Sprite waveFaceSprite;

    [Header("Wave Effect")]
    public GameObject frequencyWaveParticles;
    public float attackCooldown = 3f;

    private NavMeshAgent agent;
    private Transform playerTarget;
    private SpriteRenderer faceSpriteRenderer;

    private float attackTimer;
    private bool isDead = false;

    // Counts attacks
    private int attackCount = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stoppingDistance;
        }

        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;

        attackTimer = attackCooldown;

        // Get SpriteRenderer from the child face object
        if (faceObject != null)
        {
            faceSpriteRenderer = faceObject.GetComponent<SpriteRenderer>();
        }

        // Start with normal face
        SetNormalFace();
    }

    private void Update()
    {
        if (isDead || playerTarget == null || agent == null)
            return;

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            playerTarget.position
        );

        if (distanceToPlayer > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTarget.position);

            if (frequencyWaveParticles != null)
                frequencyWaveParticles.SetActive(true);
        }
        else
        {
            agent.isStopped = true;

            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                attackCount++;

                if (attackCount >= 5)
                {
                    // 5th attack = wave attack
                    SpawnWave();

                    // Reset attack count
                    attackCount = 0;
                }
                else
                {
                    // Normal attack
                    SetNormalFace();
                }

                attackTimer = attackCooldown;
            }
        }
    }

    private void SpawnWave()
    {
        // Change face to wave face
        SetWaveFace();

        if (frequencyWaveParticles != null)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);

            GameObject psObj = Instantiate(
                frequencyWaveParticles,
                transform.position,
                rotation
            );

            ParticleSystem ps = psObj.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                Destroy(psObj, ps.main.duration);
            }
            else
            {
                Destroy(psObj, 2f);
            }
        }

        // Return to normal face after spawning the wave
        SetNormalFace();

        // TODO: Add debuff logic here if needed.
    }

    private void SetNormalFace()
    {
        if (faceSpriteRenderer != null && normalFaceSprite != null)
        {
            faceSpriteRenderer.sprite = normalFaceSprite;
        }
    }

    private void SetWaveFace()
    {
        if (faceSpriteRenderer != null && waveFaceSprite != null)
        {
            faceSpriteRenderer.sprite = waveFaceSprite;
        }
    }

    public void Die()
    {
        isDead = true;

        if (agent != null)
            agent.isStopped = true;

        gameObject.SetActive(false);
    }
}