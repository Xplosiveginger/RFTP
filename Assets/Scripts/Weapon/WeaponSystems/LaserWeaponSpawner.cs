using System.Collections.Generic;
using UnityEngine;

public class LaserWeaponSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Range(1, 3)] public int range = 1;      // 1 = Right, 2 = Right+Left, 3 = Right+Left+Up
    public GameObject laserPrefab;           // Prefab designed to fire right

    [Header("Offsets (distance from center)")]
    public float offsetRight = 0.5f;
    public float offsetLeft = 0.5f;
    public float offsetUp = 0.5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 60f;        // Degrees per second

    private readonly List<GameObject> activeLasers = new List<GameObject>();

    void OnEnable()
    {
        SpawnLasers();
    }

    void OnDisable()
    {
        ClearLasers();
    }

    void SpawnLasers()
    {
        ClearLasers();
        if (laserPrefab == null) return;

        // Always spawn Right
        CreateLaser(Vector2.right, offsetRight);

        if (range >= 2)
            CreateLaser(Vector2.left, offsetLeft);

        if (range >= 3)
            CreateLaser(Vector2.up, offsetUp);
    }

    void CreateLaser(Vector2 dir, float distanceOffset)
    {
        GameObject laser = Instantiate(laserPrefab, transform);

        // Position relative to spawner
        laser.transform.localPosition = dir * distanceOffset;

        // Ensure laser points along its local direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        laser.transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Add rotation component
        LaserRotator rotator = laser.AddComponent<LaserRotator>();
        rotator.center = transform;
        rotator.rotationSpeed = rotationSpeed;

        activeLasers.Add(laser);
    }

    void ClearLasers()
    {
        foreach (var laser in activeLasers)
            if (laser != null) Destroy(laser);

        activeLasers.Clear();
    }
}

/// <summary>
/// Rotates the object around a center point (the spawner) at a given speed.
/// </summary>
public class LaserRotator : MonoBehaviour
{
    public Transform center;
    public float rotationSpeed = 60f;

    void Update()
    {
        if (center == null) return;

        transform.RotateAround(center.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
