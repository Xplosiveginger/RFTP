using UnityEngine;
using DG.Tweening;

public class InteractiveFallingItem : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;

    [Header("Chance Settings")]
    [Tooltip("Initial fall chance (0 to 1, e.g. 0.4 for 40%)")]
    public float fallChance = 0.4f;
    [Tooltip("Chance multiplier added on each failed fall attempt (e.g. 0.05 for 5%)")]
    public float chanceMultiplier = 0.05f;

    [Header("Detection Settings")]
    public float detectionRadius = 1.5f;

    [Header("Fall & Animation")]
    public float verticalOffset = -0.5f;
    public float fallDuration = 0.7f;
    [Tooltip("Base arc height for reference distance")]
    public float baseArcHeight = 2f;
    [Tooltip("Multiplier for arc height based on horizontal distance")]
    public float arcHeightMultiplier = 0.5f;
    [Tooltip("Minimum arc height regardless of distance")]
    public float minArcHeight = 1f;
    [Tooltip("Maximum arc height regardless of distance")]
    public float maxArcHeight = 4f;
    public float popScale = 1.2f;
    public float popDuration = 0.15f;

    [Header("Rotation Settings")]
    public Vector3 targetRotationEuler = Vector3.zero;

    [Header("Damage Settings")]
    public int damage = 10;
    public float damageRadius = 0.4f;

    [Header("Destroy")]
    public bool destroyOnFall = true;

    private bool hasFallen = false;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool playerInsidePrevFrame = false;

    // Store target fall position and arc height for gizmo display
    private Vector3 targetFallPosition;
    private float currentArcHeight;
    private bool hasTargetPosition = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    void OnDrawGizmos()
    {
        // Draw detection radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw path to fall target if we have one
        if (hasTargetPosition)
        {
            Vector3 start = transform.position;
            Vector3 end = targetFallPosition;

            Gizmos.color = Color.red;
            int segs = 20;
            Vector3 prev = start;

            for (int i = 1; i <= segs; ++i)
            {
                float t = i / (float)segs;
                Vector3 pos = Parabola(start, end, currentArcHeight, t);
                Gizmos.DrawLine(prev, pos);
                prev = pos;
            }

            // Draw damage radius at impact point
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(end, damageRadius);

            // Draw arc height indicator
            Vector3 peakPos = (start + end) / 2 + Vector3.up * currentArcHeight;
            Gizmos.color = Color.green;
            Gizmos.DrawLine((start + end) / 2, peakPos);
            Gizmos.DrawWireSphere(peakPos, 0.15f);
        }
        else
        {
            // Preview: draw potential fall straight down
            Vector3 previewEnd = transform.position + new Vector3(0, verticalOffset, 0);
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, previewEnd);
            Gizmos.DrawWireSphere(previewEnd, damageRadius);
        }
    }

    void Update()
    {
        if (hasFallen) return;

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

        // Update target position and arc height for gizmo while player is in range
        if (playerInsideNow && playerTransform != null)
        {
            targetFallPosition = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + verticalOffset,
                playerTransform.position.z
            );

            // Calculate dynamic arc height based on horizontal distance
            currentArcHeight = CalculateArcHeight(transform.position, targetFallPosition);
            hasTargetPosition = true;
        }
        else
        {
            hasTargetPosition = false;
        }

        // Detect player entering detection radius (rising edge)
        if (playerInsideNow && !playerInsidePrevFrame && playerTransform != null)
        {
            if (Random.value < fallChance)
            {
                StartPopAndFall(playerTransform.position);
            }
            else
            {
                fallChance = Mathf.Clamp01(fallChance + chanceMultiplier);
            }
        }

        playerInsidePrevFrame = playerInsideNow;
    }

    float CalculateArcHeight(Vector3 start, Vector3 end)
    {
        // Calculate horizontal distance (ignoring Y difference)
        Vector2 startFlat = new Vector2(start.x, start.z);
        Vector2 endFlat = new Vector2(end.x, end.z);
        float horizontalDistance = Vector2.Distance(startFlat, endFlat);

        // Calculate vertical drop
        float verticalDrop = Mathf.Abs(start.y - end.y);

        // Arc height scales with horizontal distance and considers vertical drop
        // Longer horizontal distances = higher arc
        // Greater vertical drops = slightly reduced arc (since gravity assists)
        float calculatedHeight = baseArcHeight + (horizontalDistance * arcHeightMultiplier) - (verticalDrop * 0.2f);

        // Clamp to min/max values for reasonable physics
        return Mathf.Clamp(calculatedHeight, minArcHeight, maxArcHeight);
    }

    void StartPopAndFall(Vector3 playerPosition)
    {
        hasFallen = true;

        // Lock in the fall target position
        targetFallPosition = new Vector3(
            playerPosition.x,
            playerPosition.y + verticalOffset,
            playerPosition.z
        );

        // Calculate final arc height for this fall
        currentArcHeight = CalculateArcHeight(transform.position, targetFallPosition);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(popScale * originalScale, popDuration).SetEase(Ease.OutBack));
        seq.Append(transform.DOScale(originalScale, popDuration).SetEase(Ease.InQuart));
        seq.AppendCallback(DoArcFall);
        seq.SetUpdate(true);
    }

    void DoArcFall()
    {
        Vector3 start = transform.position;
        Vector3 end = targetFallPosition;
        float tVal = 0f;

        Tweener moveTween = DOTween.To(() => tVal, x =>
        {
            tVal = x;
            transform.position = Parabola(start, end, currentArcHeight, tVal);
        }, 1f, fallDuration).SetEase(Ease.InQuad);

        Tweener rotateTween = transform.DOLocalRotate(targetRotationEuler, fallDuration).SetEase(Ease.Linear);

        Sequence fallSeq = DOTween.Sequence();
        fallSeq.Join(moveTween);
        fallSeq.Join(rotateTween);

        fallSeq.OnComplete(() =>
        {
            TryHitPlayer();

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, end, Quaternion.identity);
            }

            if (destroyOnFall)
                Destroy(gameObject);
        });
    }

    void TryHitPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(targetFallPosition, damageRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag("Player"))
            {
                var healthSys = col.GetComponent<HealthSystem>();
                if (healthSys != null)
                    healthSys.Damage(damage);
            }
        }
    }

    Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 peak = (start + end) / 2 + Vector3.up * height;
        Vector3 a = Vector3.Lerp(start, peak, t);
        Vector3 b = Vector3.Lerp(peak, end, t);
        return Vector3.Lerp(a, b, t);
    }
}
