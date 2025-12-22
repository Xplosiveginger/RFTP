using System.Collections.Generic;
using UnityEngine;

public class LaserWeaponRefactored : WeaponBase
{
    [Header("Spawner Settings")]
    [Range(1, 3)] public int range = 1;      // 1 = Right, 2 = Right+Left, 3 = Right+Left+Up
    public GameObject laserPrefab;           // Prefab designed to fire right

    [Header("Offsets (distance from center)")]
    public float offsetRight = 0.5f;
    public float offsetLeft = 0.5f;
    public float offsetUp = 0.5f;

    [Header("Oscillation Settings")]
    public float moveRange = 0.5f;   // How far they move
    public float moveSpeed = 2f;     // Speed of oscillation

    public List<GameObject> activeLasers = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();

        damage = statManager.GetStat(EStatType.Damage).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
    }

    protected override void Start()
    {
        SpawnLasers();
        base.Start();
    }

    public override void SpawnWeapon(Transform parent)
    {
        base.SpawnWeapon(parent);
    }

    void SpawnLasers()
    {
        ClearLasers();
        if (laserPrefab == null) return;
        Debug.Log("Spawning");

        // Always spawn Right
        CreateLaser(Vector2.right, 0f, offsetRight);

        if (range >= 2)
            CreateLaser(Vector2.left, 180f, offsetLeft);

        if (range >= 3)
            CreateLaser(Vector2.up, 90f, offsetUp);
    }

    void CreateLaser(Vector2 dir, float angle, float distanceOffset)
    {
        GameObject laser = Instantiate(laserPrefab, transform);

        // Position relative to spawner
        laser.transform.localPosition = dir * distanceOffset;

        // Rotate correctly
        laser.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Add oscillator
        LaserOscillator osc = laser.AddComponent<LaserOscillator>();
        osc.range = moveRange;
        osc.speed = moveSpeed;
        osc.startPos = laser.transform.localPosition;

        // Random phase for non-simultaneous motion
        osc.phaseOffset = Random.Range(0f, Mathf.PI * 2f);

        activeLasers.Add(laser);
    }

    void ClearLasers()
    {
        foreach (var laser in activeLasers)
            if (laser != null) Destroy(laser);

        activeLasers.Clear();
    }

    public override void ToggleGFXVisibility(bool b)
    {
        foreach(var laser in activeLasers)
        {
            laser.SetActive(b);
        }
    }

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();

        UpdateDamageForEachLaser();
    }

    private void UpdateDamageForEachLaser()
    {
        foreach(var laser in activeLasers)
        {
            laser.transform.GetChild(4).GetComponent<Damage>().damage = damage;
        }
    }
}