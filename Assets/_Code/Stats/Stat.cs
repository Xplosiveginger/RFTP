using System;

[System.Serializable]
public class Stat
{
    public EStatType statName;

    // =========================================================
    // BASE VALUES
    // =========================================================

    public float baseValue = 0f;
    public float maxValue = 0f;

    // Value after permanent / shop modifiers.
    // This is the starting point for runtime modifications.
    public float startValue = 0f;

    // Permanent/shop percentage multiplier.
    public float startMultiplier = 1f;

    // =========================================================
    // RUNTIME MODIFIERS
    // =========================================================

    // Runtime percentage multiplier.
    //
    // Example:
    // +20% = 0.20
    // +30% = 0.30
    //
    // Multiple modifiers stack additively:
    // +20% +30% = 1.50 total multiplier.
    public float currentMultiplier = 1f;

    // Runtime flat modifications.
    //
    // Example:
    // Weapon level +5 Damage
    // Projectile Count +1
    public float flatModifier = 0f;

    // Final calculated value.
    public float currentValue;

    public event Action<Stat> OnCurrentValueChanged;
    public event Action OnMaxValueChanged;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public Stat(
        EStatType statName,
        float baseValue,
        float maxValue,
        float startValue,
        float startMultiplier)
    {
        this.statName = statName;

        this.baseValue = baseValue;
        this.maxValue = maxValue;

        // Stat starts from its actual base value.
        this.startValue = baseValue;

        this.startMultiplier = startMultiplier;
        this.currentMultiplier = startMultiplier;

        this.flatModifier = 0f;

        RecalculateValue(false);
    }


    // =========================================================
    // RECALCULATION
    // =========================================================

    /// <summary>
    /// Rebuilds the final stat value from its underlying values.
    ///
    /// Formula:
    ///
    /// (startValue + flatModifier)
    /// × currentMultiplier
    /// ÷ startMultiplier
    ///
    /// The startMultiplier is removed so permanent/shop
    /// percentage modifiers are not accidentally applied twice.
    /// </summary>
    private void RecalculateValue(bool invokeEvent = true)
    {
        float permanentMultiplier =
            Math.Max(startMultiplier, 0.0001f);

        float runtimeMultiplier =
            currentMultiplier / permanentMultiplier;

        float modifiedValue =
            startValue + flatModifier;

        currentValue =
            modifiedValue * runtimeMultiplier;

        if (invokeEvent)
        {
            OnCurrentValueChanged?.Invoke(this);
        }
    }


    // =========================================================
    // PRE-GAME / SHOP MODIFIER
    // =========================================================

    public void ApplyStartModifier(
        float modifier,
        bool isPercentage)
    {
        if (isPercentage)
        {
            float multiplier = modifier / 100f;

            float valueToAdd =
                baseValue * multiplier;

            startValue += valueToAdd;

            startMultiplier += multiplier;

            currentMultiplier = startMultiplier;

            // Health also increases its maximum.
            if (statName == EStatType.Health)
            {
                maxValue += valueToAdd;
            }
        }
        else
        {
            startValue += modifier;

            if (statName == EStatType.Health)
            {
                maxValue += modifier;
            }
        }

        RecalculateValue();

        if (statName == EStatType.Health)
        {
            OnMaxValueChanged?.Invoke();
        }
    }


    // =========================================================
    // RUNTIME PERCENTAGE MODIFIER
    // =========================================================

    // =========================================================
// RUNTIME PERCENTAGE MODIFIER
// =========================================================

    public void ApplyModifier(float modifier)
    {
        float tempMultiplier = modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;

        // -----------------------------------------------------
        // HEALTH
        // -----------------------------------------------------
        // A health modifier increases BOTH:
        // - Current Health
        // - Maximum Health
        //
        // Example:
        // Base Health = 100
        // +20% Health
        //
        // Current Health += 20
        // Max Health     += 20
        // -----------------------------------------------------

        if (statName == EStatType.Health)
        {
            currentValue += valueToAdd;
            maxValue += valueToAdd;

            currentMultiplier += tempMultiplier;

            OnCurrentValueChanged?.Invoke(this);
            OnMaxValueChanged?.Invoke();

            return;
        }

        // -----------------------------------------------------
        // NORMAL STATS
        // -----------------------------------------------------

        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;

        OnCurrentValueChanged?.Invoke(this);
    }
    // =========================================================
    // HEALTH MODIFIER
    // =========================================================

    public void ApplyHealthModifier(float modifier)
    {
        if (statName != EStatType.Health)
            return;

        float tempMultiplier =
            modifier / 100f;

        float valueToAdd =
            maxValue * tempMultiplier;

        currentValue += valueToAdd;
        maxValue += valueToAdd;

        OnCurrentValueChanged?.Invoke(this);
        OnMaxValueChanged?.Invoke();

        currentMultiplier += tempMultiplier;
    }


    // =========================================================
    // COOLDOWN MODIFIER
    // =========================================================

    public void ApplyCooldownModifier(float modifier)
    {
        if (statName != EStatType.AttackCooldown)
            return;

        float tempMultiplier =
            modifier / 100f;

        // Positive modifier means cooldown reduction.
        currentMultiplier -= tempMultiplier;

        RecalculateValue();
    }


    // =========================================================
    // REVERT RUNTIME MODIFIER
    // =========================================================

    public void RevertModifier(float modifier)
    {
        float tempMultiplier =
            modifier / 100f;

        // Reverse the exact percentage modification.
        currentMultiplier -= tempMultiplier;

        RecalculateValue();
    }


    // =========================================================
    // FLAT MODIFIER
    // =========================================================

    public void AddFlat(float value)
    {
        flatModifier += value;

        RecalculateValue();
    }


    // =========================================================
    // REMOVE FLAT MODIFIER
    // =========================================================

    public void RemoveFlat(float value)
    {
        flatModifier -= value;

        RecalculateValue();
    }


    // =========================================================
    // RESET RUNTIME MODIFIERS
    // =========================================================

    public void ResetRuntimeModifiers()
    {
        flatModifier = 0f;
        currentMultiplier = startMultiplier;

        RecalculateValue();
    }
}