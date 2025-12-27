using System;

[System.Serializable]
public class Stat
{
    public EStatType statName;
    public float baseValue = 0f;
    public float maxValue = 0f; // incase the max value also scales
    public float startValue = 0f; // pregame items
    public float startMultiplier = 1f;

    public float currentMultiplier = 1f;
    public float currentValue;

    public event Action<Stat> OnCurrentValueChanged;
    public event Action OnMaxValueChanged;

    public Stat(EStatType statName, float baseValue, float maxValue, float startValue, float startMultiplier) // Change this
    {
        this.statName = statName;
        this.baseValue = baseValue;
        this.maxValue = maxValue;
        this.startValue = startValue;
        this.startMultiplier = startMultiplier;
        this.currentMultiplier = this.startMultiplier;
        this.currentValue = this.baseValue;
    }

    public void ApplyModifier(float modifier, bool subtract)
    {
        float tempMultiplier = modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;
        currentValue = (!subtract)? currentValue + valueToAdd : currentValue - valueToAdd;
        currentMultiplier = (!subtract) ? currentMultiplier + tempMultiplier : currentMultiplier - tempMultiplier;

        OnCurrentValueChanged?.Invoke(this);
    }

    public void ApplyHealthModifier(float modifier, bool subtract)
    {
        if (statName != EStatType.Health) return;

        float tempMultiplier = modifier / 100f;

        float valueToAdd = maxValue * tempMultiplier;
        currentValue = (!subtract) ? currentValue + valueToAdd : currentValue - valueToAdd;
        OnCurrentValueChanged?.Invoke(this);
        maxValue = (!subtract) ? maxValue + valueToAdd : maxValue - valueToAdd;
        OnMaxValueChanged.Invoke();
        currentMultiplier = (!subtract) ? currentMultiplier + tempMultiplier : currentMultiplier - tempMultiplier;
    }

    public void ApplyCooldownModifier(float modifier)
    {
        if (statName != EStatType.AttackCooldown) return;

        float tempMultiplier = modifier / 100f;

        float valueToRemove = baseValue * tempMultiplier;
        currentValue -= valueToRemove;
        OnCurrentValueChanged?.Invoke(this);
        currentMultiplier -= tempMultiplier;
    }

    public void RevertModifier(float modifier)
    {
        float tempMultiplier = -modifier / 100f;

        float valueToAdd = baseValue * tempMultiplier;
        currentValue +=  valueToAdd;
        currentMultiplier += tempMultiplier;

        OnCurrentValueChanged?.Invoke(this);
    }

    public void AddFlat(float value)
    {
        currentValue += value;
        OnCurrentValueChanged?.Invoke(this);
    }
}