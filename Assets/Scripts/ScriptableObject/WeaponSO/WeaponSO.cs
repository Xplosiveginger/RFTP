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
    public Sprite cardSprite; // ✅ Image per level
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName; // ✅ kept for reference

    [Header("Per Level Settings")]
    public List<Levels> levels;
}
