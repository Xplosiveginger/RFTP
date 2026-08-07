using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerItemAttractor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IAttractable>(out var attractable))
        {
            attractable.AttractTo(transform);
        }
    }
}