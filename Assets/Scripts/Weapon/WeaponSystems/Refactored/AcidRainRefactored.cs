using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRainRefactored : WeaponBase
{
    [Header("Spawn Area")]
    public float boxLength = 5f;
    public float boxBreadth = 3f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    //public int projectileCount = 10;  // how many to spawn once

    [Header("Spawn Timing")]
    public bool spawnOnEnable = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (spawnOnEnable)
        {
            SpawnProjectiles();
        }
    }

    protected override void Awake()
    {
        base.Awake();

        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
    }

    protected override void Start()
    {
        Debug.Log(damage);


        base.Start();
    }

    public void SpawnProjectiles()
    {
        if (projectilePrefab == null) return;

        for (int i = 0; i < projectileCount; i++)
        {
            // Pick a random point inside the box
            float randX = Random.Range(-boxLength / 2f, boxLength / 2f);
            float randY = Random.Range(-boxBreadth / 2f, boxBreadth / 2f);

            Vector3 spawnPos = transform.position + new Vector3(randX, randY, 0f);

            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        }
    }

    // Draw Gizmos for editor visualization
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(boxLength, boxBreadth, 0f));
    }

    public override void ToggleGFXVisibility(bool b)
    {
        //foreach (var laser in activeLasers)
        //{
        //    laser.SetActive(b);
        //}
    }
}
