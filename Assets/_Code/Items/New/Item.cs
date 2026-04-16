using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string displayName;
    public string description;
    public Sprite cardSprite;

    public EStatType affectedStat;
    [Tooltip("Value to modify the stat by. + ve is adding and - ve is subtracting.")]
    public float modifierAmount;
    public bool isPercent = false;

    // Add ability to affect multiple stats
}
