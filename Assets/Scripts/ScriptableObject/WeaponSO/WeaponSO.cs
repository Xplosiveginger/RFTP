using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timings
{
    public float coolDownTime;
    public float activeTime;
}

[System.Serializable]
public class Levels
{
    public Timings time;
    public GameObject weaponLevelPrefab;
    public Sprite cardSprite;           // ✅ Card image for this level
    [TextArea] public string levelUpInfo; // ✅ New: Info text for this level
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;

    [Header("Per Level Settings")]
    public List<Levels> levels;
}
