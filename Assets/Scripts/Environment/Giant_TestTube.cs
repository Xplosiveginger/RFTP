using UnityEngine;
using DG.Tweening;
using System.Collections;

public class InteractiveTiltingItem : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;

    [Header("Chance Settings")]
    [Tooltip("Initial tilt chance (0 to 1, e.g. 0.2 for 20%)")]
    public float tiltChance = 0.2f;
    [Tooltip("Chance multiplier added on each failed tilt attempt (e.g. 0.05 for 5%)")]
    public float chanceMultiplier = 0.05f;

    [Header("Detection Settings")]
    public float detectionRadius = 1.5f;

    [Header("Tilt & Animation")]
    public Transform tiltTarget;
    public float tiltDuration = 0.7f;
    public Vector3 targetRotationEuler = new Vector3(45f, 45f, 0f);
    public bool useDynamicTiltDirection = true;

    [Header("Droplet Fall Animation")]
    [Tooltip("Sprite to use for the falling droplet effect")]
    public Sprite dropletSprite;
    [Tooltip("Height of the parabolic arc for the droplet animation")]
    public float dropletFallArcHeight = 2f;
    [Tooltip("Duration of the droplet fall animation")]
    public float dropletFallDuration = 0.5f;

    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Destroy")]
    public bool destroyOnTilt = true;

    [Header("Reset Settings")]
    [Tooltip("Time in seconds to wait before allowing tilt again")]
    public float resetInterval = 0.5f;

    [Header("Sprite Settings")]
    [Tooltip("Sprite to switch to when player enters detection radius")]
    public Sprite spriteOnPlayerEnter;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    private bool hasTilted = false;
    private Quaternion originalRotation;
    private bool playerInsidePrevFrame = false;
    private Vector3 dynamicTiltEuler;

    private void Awake()
    {
        originalRotation = transform.localRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (tiltTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, tiltTarget.position);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tiltTarget.position, 0.3f);
        }
    }

    void Update()
    {
        if (hasTilted) return;

        bool playerInsideNow = false;
        Transform playerTransform = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                playerInsideNow = true;
                playerTransform = col.transform;
                break;
            }
        }

        // Change sprite on player entering detection radius (rising edge)
        if (playerInsideNow && !playerInsidePrevFrame && spriteRenderer != null && spriteOnPlayerEnter != null)
        {
            spriteRenderer.sprite = spriteOnPlayerEnter;
        }
        // Revert sprite on player exit
        if (!playerInsideNow && playerInsidePrevFrame && spriteRenderer != null && originalSprite != null)
        {
            spriteRenderer.sprite = originalSprite;
        }

        // Trigger tilt and droplet on player entering detection radius
        if (playerInsideNow && !playerInsidePrevFrame)
        {
            if (Random.value < tiltChance)
            {
                if (useDynamicTiltDirection && playerTransform != null)
                {
                    Vector3 dir = (playerTransform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    dynamicTiltEuler = new Vector3(0f, 0f, angle);
                }
                StartTiltAndDropletFall();
            }
            else
            {
                tiltChance += chanceMultiplier;
                tiltChance = Mathf.Clamp01(tiltChance);
            }
        }

        playerInsidePrevFrame = playerInsideNow;
    }

    void StartTiltAndDropletFall()
    {
        hasTilted = true;
        Vector3 tiltEuler = useDynamicTiltDirection ? dynamicTiltEuler : targetRotationEuler;

        // Tilt animation (optional: can comment out if you only want droplet, not tilt)
        transform.DOLocalRotate(tiltEuler, tiltDuration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            SoundFXManager.instance.PlayTestTubeSFX(transform, 1);
            StartCoroutine(StartDropletFall());
        });
    }

    IEnumerator StartDropletFall()
    {
        if (dropletSprite == null || tiltTarget == null)
        {
            
            // Fallback: just spawn prefab
            if (prefabToSpawn != null)
                Instantiate(prefabToSpawn, tiltTarget.position, Quaternion.identity);
            yield break;
        }

        // Create temporary droplet GameObject
        GameObject droplet = new GameObject("DropletTempSprite");
        SpriteRenderer dropletRenderer = droplet.AddComponent<SpriteRenderer>();
        dropletRenderer.sprite = dropletSprite;
        dropletRenderer.sortingOrder = 1000; // render above other sprites if needed

        // Set start position
        droplet.transform.position = transform.position;

        Vector3 startPos = transform.position;
        Vector3 endPos = tiltTarget.position;

        float tVal = 0f;
        Sequence seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => tVal, x =>
        {
            tVal = x;
            droplet.transform.position = Parabola(startPos, endPos, dropletFallArcHeight, tVal);
        }, 1f, dropletFallDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            if (prefabToSpawn != null)
                Instantiate(prefabToSpawn, endPos, Quaternion.identity);

            Destroy(droplet);

            if (destroyOnTilt)
                Destroy(gameObject);
            else
                StartCoroutine(ResetTiltAfterInterval());
        });

        yield return seq.WaitForCompletion();
    }

    IEnumerator ResetTiltAfterInterval()
    {
        yield return new WaitForSeconds(resetInterval);
        hasTilted = false;
    }

    Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 peak = (start + end) / 2 + Vector3.up * height;
        Vector3 a = Vector3.Lerp(start, peak, t);
        Vector3 b = Vector3.Lerp(peak, end, t);
        return Vector3.Lerp(a, b, t);
    }
}
