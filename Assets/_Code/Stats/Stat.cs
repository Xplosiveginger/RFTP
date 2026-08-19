using System;

[System.Serializable]
public class Stat
{
    public EStatType statName;

    public float baseValue = 0f;
    public float maxValue = 0f;

    // Value after permanent pre-game/shop modifiers
    public float startValue = 0f;

    public float startMultiplier = 1f;

    public float currentMultiplier = 1f;
    public float currentValue;

    public event Action<Stat> OnCurrentValueChanged;
    public event Action OnMaxValueChanged;

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

        // Start from the actual base value.
        // Shop modifiers will be applied on top of this.
        this.startValue = baseValue;

        this.startMultiplier = startMultiplier;

        this.currentMultiplier = this.startMultiplier;

        this.currentValue = this.startValue;
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
            float valueToAdd = baseValue * (modifier / 100f);

            startValue += valueToAdd;
            currentValue = startValue;

            // Health also increases its maximum
            if (statName == EStatType.Health)
            {
                maxValue += valueToAdd;
            }

            startMultiplier += modifier / 100f;
            currentMultiplier = startMultiplier;
        }
        else
        {
            startValue += modifier;
            currentValue = startValue;

            if (statName == EStatType.Health)
            {
                maxValue += modifier;
            }
        }

        OnCurrentValueChanged?.Invoke(this);

        if (statName == EStatType.Health)
        {
            OnMaxValueChanged?.Invoke();
        }
    }
    // =========================================================
    // RUNTIME PERCENTAGE MODIFIER
    // =========================================================

    public void ApplyModifier(float modifier)
    {
        float tempMultiplier = modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;

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

        float tempMultiplier = modifier / 100f;

        float valueToAdd = maxValue * tempMultiplier;

        currentValue += valueToAdd;

        OnCurrentValueChanged?.Invoke(this);

        maxValue += valueToAdd;

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

        float tempMultiplier = modifier / 100f;

        float valueToRemove = baseValue * tempMultiplier;

        currentValue -= valueToRemove;

        OnCurrentValueChanged?.Invoke(this);

        currentMultiplier -= tempMultiplier;
    }

    // =========================================================
    // REVERT RUNTIME MODIFIER
    // =========================================================

    public void RevertModifier(float modifier)
    {
        float tempMultiplier = -modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;

        currentValue += valueToAdd;
        currentMultiplier += tempMultiplier;

        OnCurrentValueChanged?.Invoke(this);
    }

    // =========================================================
    // FLAT MODIFIER
    // =========================================================

    public void AddFlat(float value)
    {
        currentValue += value;

        OnCurrentValueChanged?.Invoke(this);
    }
}