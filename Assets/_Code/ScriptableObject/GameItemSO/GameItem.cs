using UnityEngine;

/// <summary>
/// World pickup for Game items.
/// Only responsibility is detecting pickup
/// and notifying ItemManager.
/// </summary>
[RequireComponent(typeof(Collider))]
public class GameItem : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;

    private ItemManager itemManager;
    private bool pickedUp;

    private void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
    }

    private void Reset()
    {
        // Ensure collider is trigger for pickup
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        pickedUp = true;

        if (itemManager != null)
            itemManager.AddItem(itemSO);
        else
            Debug.LogError("[GameItem] ItemManager not found!");

        Destroy(gameObject);
    }
}