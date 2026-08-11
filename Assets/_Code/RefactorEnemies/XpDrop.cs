
using System;
using UnityEngine;
public interface IAttractable
{
    void AttractTo(Transform target);
}


[RequireComponent(typeof(Collider2D))]
public class XpDrop : MonoBehaviour, IAttractable
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 18f;
    [SerializeField] private float accelerationTime = 0.35f;
    [SerializeField] private float collectDistance = 0.1f;

    public AudioClip expCollectSound;
    private Transform target;
    private Collider2D col;

    private bool isTravelling;
    private float currentSpeed;
    private float acceleration;
    private int xpAmount;

    private XpManager XpManager;
    private void Awake()
    {
        col = GetComponent<Collider2D>();

        acceleration = maxSpeed / Mathf.Max(0.01f, accelerationTime);
    }

    private void Start()
    {
        XpManager = SM.Instance.XPManager;
    }

    private void Update()
    {
        if (!isTravelling || target == null)
            return;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            maxSpeed,
            acceleration * Time.deltaTime);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            currentSpeed * Time.deltaTime);

        if ((transform.position - target.position).sqrMagnitude <= collectDistance * collectDistance)
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
        currentSpeed = 0f;

        if (col != null)
            col.enabled = false;
    }

    public void SetXpAmount(int amount)
    {
        xpAmount = amount;
    }

    private void XpCollected()
    {
        
        XpManager?.AddXP(xpAmount);
        GlobalAudioPlayer.Instance.PlayAudio(expCollectSound);
        Destroy(gameObject);
    }
}