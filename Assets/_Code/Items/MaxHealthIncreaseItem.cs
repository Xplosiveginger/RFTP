using UnityEngine;

public class MaxHealthIncreaseItem : MonoBehaviour
{
    [Header("Max Health Increase Settings")]
    [Tooltip("Percent increase to player's max health. Example: 25 = +25% max health.")]
    [Range(0, 500)]
    public float percentBonus = 25f;

    private static bool isActive = false;
    private static float totalPercentBonus = 0f;

    // Stores the player's original max health.
    private static float baseMaxHealth = -1f;

    private void OnEnable()
    {
        isActive = true;
        totalPercentBonus += percentBonus;

        ApplyToPlayerHealth();

        Debug.Log(
            $"[MaxHealthIncreaseItem] Activated: " +
            $"+{percentBonus}% " +
            $"(total {totalPercentBonus}%)"
        );
    }

    private void OnDisable()
    {
        totalPercentBonus -= percentBonus;

        if (totalPercentBonus <= 0f)
        {
            totalPercentBonus = 0f;
            isActive = false;
        }

        ApplyToPlayerHealth();

        Debug.Log(
            $"[MaxHealthIncreaseItem] Deactivated: " +
            $"total {totalPercentBonus}%"
        );
    }

    /// <summary>
    /// Finds the player's HealthSystem and updates its max health.
    /// </summary>
    private void ApplyToPlayerHealth()
    {
        HealthSystem player =
            FindObjectOfType<HealthSystem>(includeInactive: true);

        if (player == null || !player.isPlayer)
            return;

        // Store the original max health once.
        if (baseMaxHealth < 0f)
        {
            baseMaxHealth = player.MaxHealth;
        }

        float newMaxHealth =
            GetModifiedMaxHealth(baseMaxHealth);

        player.SetMaxHealth(
            newMaxHealth,
            resetCurrentHealth: false
        );
    }

    /// <summary>
    /// Calculates boosted max health based on the
    /// original base max health and total percentage bonus.
    /// </summary>
    public static float GetModifiedMaxHealth(float baseMaxHealth)
    {
        if (!isActive || totalPercentBonus <= 0f)
            return baseMaxHealth;

        float multiplier =
            1f + (totalPercentBonus / 100f);

        return Mathf.RoundToInt(baseMaxHealth * multiplier);
    }

    public static bool IsActive() => isActive;
}
