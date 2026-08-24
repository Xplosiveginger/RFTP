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

    private List<GameObject> puddles = new List<GameObject>();

    protected override void OnEnable()
    {

    }

    protected override void Awake()
    {
        base.Awake();

        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
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

            GameObject go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            go.GetComponent<PreSecDamage>().Initialize(damage, AOESize);
            puddles.Add(go);
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
        if(b == false)
        {
            foreach(GameObject puddle in puddles)
            {
                Destroy(puddle);
            }

            puddles.Clear();
        }
        else
        {
            SpawnProjectiles();
        }
    }

    public override void SpawnWeapon(Transform parent)
    {
        base.SpawnWeapon(parent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();
    }

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();
    }

    protected override void UpdateStatsHandled()
    {
        damage = statManager.GetStat(EStatType.Damage).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();
        UpgradeAcidRain();
    }

    private void UpgradeAcidRain()
    {
        switch (level)
        {
            case 1:
                break;
            case 2:
                statManager.ModifyStat(EStatType.AOESize, 10);
                break;
            case 3:
                statManager.ModifyStatValue(EStatType.Damage, 10);
                statManager.ModifyStatValue(EStatType.ActiveDuration, 0.5f);
                break;
            case 4:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStat(EStatType.AOESize, 10);
                break;
            case 5:
                statManager.ModifyStatValue(EStatType.Damage, 10);
                statManager.ModifyStatValue(EStatType.ActiveDuration, 0.5f);
                break;
            case 6:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStat(EStatType.AOESize, 10);
                break;
            case 7:
                statManager.ModifyStatValue(EStatType.Damage, 10);
                statManager.ModifyStatValue(EStatType.ActiveDuration, 0.5f);
                break;
            case 8:
                statManager.ModifyStat(EStatType.AOESize, 10);
                break;
            default:
                Debug.Log($"Max Level Reached for {weaponData.weaponName}");
                break;
        }
    }
}