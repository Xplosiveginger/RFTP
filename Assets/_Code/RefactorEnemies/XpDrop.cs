using UnityEngine;

public interface IAttractable
{
    void AttractTo(Transform target);
}

[RequireComponent(typeof(Collider2D))]
public class XpDrop : MonoBehaviour, IAttractable
{
    [SerializeField] private float attractionSpeed = 12f;
    [SerializeField] private float collectDistance = 0.1f;

    private Transform target;
    private Collider2D col;
    private bool isTravelling;
    private int xpAmount;

    private XpManager XpManager;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        XpManager = SM.Instance.XPManager;
    }

    private void Update()
    {
        if (!isTravelling || target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            attractionSpeed * Time.deltaTime);

        if (Vector3.SqrMagnitude(transform.position - target.position) <= collectDistance * collectDistance)
        {
            XpCollected();
        }
    }

    public void AttractTo(Transform targetTransform)
    {
        if (isTravelling)
            return;

        target = targetTransform;
        isTravelling = true;

        if (col != null)
            col.enabled = false;
    }

    public void SetXpAmount(int amount)
    {
        xpAmount = amount;
    }

    private void XpCollected()
    {
        XpManager.AddXP(xpAmount);
        Debug.Log($"Collected {xpAmount} XP");
        Destroy(gameObject);
    }
}