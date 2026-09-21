using System.Collections;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum PickupType
    {
        Money,
        Chocolate,
        LunchBox
    }

    [Header("Pickup Settings")]
    [SerializeField] private PickupType pickupType;

    [Tooltip("Amount added based on the pickup type.")]
    [SerializeField] private int amountToAdd = 10;

    [Header("Pickup Movement")]
    [SerializeField] private float pickupDelay = 1f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float pickupDistance = 0.1f;

    [Header("Pickup Effect")]
    [Tooltip("Optional effect played when the pickup is collected.")]
    [SerializeField] private GameObject pickupEffectPrefab;

    private Transform playerTransform;
    private bool isBeingPickedUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isBeingPickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        playerTransform = other.transform;

        StartCoroutine(PickupRoutine());
    }

    private IEnumerator PickupRoutine()
    {
        isBeingPickedUp = true;

        // Wait before moving toward the player.
        yield return new WaitForSeconds(pickupDelay);

        // Move toward the player until close enough.
        while (playerTransform != null)
        {
            float distance = Vector2.Distance(
                transform.position,
                playerTransform.position
            );

            if (distance <= pickupDistance)
                break;

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Player may have disappeared/died/etc.
        if (playerTransform == null)
        {
            Destroy(gameObject);
            yield break;
        }

        // Give the actual reward only after reaching the player.
        GivePickupReward();

        // Play pickup effect.
        PlayPickupEffect();

        // Destroy the pickup.
        Destroy(gameObject);
    }

    private void GivePickupReward()
    {
        switch (pickupType)
        {
            case PickupType.Money:
                GiveMoney();
                break;

            case PickupType.Chocolate:
                GiveHealth();
                break;

            case PickupType.LunchBox:
                GiveHealth();
                break;
        }
    }

    private void GiveMoney()
    {
        EconomyManager economyManager =
            PersistentObject.Instance.GetComponent<EconomyManager>();

        if (economyManager == null)
        {
            Debug.LogError("EconomyManager not found.");
            return;
        }

        economyManager.AddMoney(amountToAdd);

        Debug.Log($"PickupItem: Added {amountToAdd} money.");
    }

    private void GiveHealth()
    {
        HealthSystem playerHealth =
            playerTransform.GetComponent<HealthSystem>();

        if (playerHealth == null)
        {
            Debug.LogError("HealthSystem not found on player.");
            return;
        }

        if (playerHealth.IsDead)
            return;

        // Only heal if the player isn't already at max health.
        if (playerHealth.CurrentHealth < playerHealth.MaxHealth)
        {
            playerHealth.Heal(amountToAdd);

            Debug.Log(
                $"PickupItem: Added {amountToAdd} health ({pickupType})."
            );
        }
    }

    private void PlayPickupEffect()
    {
        if (pickupEffectPrefab == null)
            return;

        GameObject effectInstance = Instantiate(
            pickupEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        effectInstance.transform.SetParent(playerTransform);

        Destroy(effectInstance, 1f);
    }
}