using Unity.VisualScripting;
using UnityEngine;

public class XpDrop : MonoBehaviour
{
    public int xpAmount = 10;
    public float pickupRange = 1.5f;
    public float moveSpeed = 5f;

    private Transform player;
    XPManager xpSystem;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        XpManager xpSystem = SM.Instance.XPManager;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= pickupRange)
        {
            // Move toward player for smooth pickup feel
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (xpSystem != null)
            {
                xpSystem.AddXP(xpAmount);
            }

            Destroy(gameObject); // or Destroy(gameObject);
        }
    }
}
