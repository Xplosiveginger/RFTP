using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : WeaponBase
{
    [Header("Solar System Settings")]
    public List<GameObject> planetPrefabs; // Assign different planet prefabs in Inspector
    public float minOrbitRadius = 2f;      // Radius of the first planet
    public float gapBetweenPlanets = 1.5f; // Constant gap between each orbit

    [Header("Speed Settings")]
    public float maxOrbitSpeed = 100f;     // Speed of first planet
    public float speedSubtractor = 10f;    // How much slower each next planet gets

    public int damageIncWithLvl = 5;

    private Transform player;
    public List<PlanetData> planets = new List<PlanetData>();

    public List<GameObject> activePlanets = new List<GameObject>();

    [System.Serializable]
    public class PlanetData
    {
        public GameObject planet;
        public float angle;
        public float speed;
        public float radius;
    }

    protected override void Start()
    {
        player = transform.parent; // Weapon is attached to Player

        if (planetPrefabs == null || planetPrefabs.Count == 0)
        {
            Debug.LogWarning("No planet prefabs assigned to SolarSystem!");
            return;
        }

        for (int i = 0; i < planetPrefabs.Count; i++)
        {
            float angle = (360f / planetPrefabs.Count) * i;
            float radius = minOrbitRadius + gapBetweenPlanets * i;

            // First planet keeps max speed; others get reduced speed
            float calculatedSpeed = (i == 0)
                ? projectileSpeed
                : projectileSpeed - (speedSubtractor * i);

            Vector3 position = player.position + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
            GameObject planet = Instantiate(planetPrefabs[i], position, Quaternion.identity, transform);

            planets.Add(new PlanetData
            {
                planet = planet,
                angle = angle,
                speed = calculatedSpeed,
                radius = radius
            });

            if(level >= i + 1)
                activePlanets.Add(planet);
            else
                planet.SetActive(false);
        }
    }

    public override void SpawnWeapon(Transform parent)
    {
        base.SpawnWeapon(parent);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Awake()
    {
        base.Awake();

        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;
    }

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();
        UpdatePlanetRotation();
    }

    public override void ToggleGFXVisibility(bool b)
    {
        foreach(GameObject planet in activePlanets)
        {
            planet.SetActive(b);
        }
    }

    protected override void UpdateStatsHandled()
    {
        AOESize = statManager.GetStat(EStatType.AOESize).currentValue;
        cooldown = statManager.GetStat(EStatType.AttackCooldown).currentValue;
        duration = statManager.GetStat(EStatType.ActiveDuration).currentValue;
        projectileSpeed = statManager.GetStat(EStatType.ProjectileSpeed).currentValue;
        projectileCount = statManager.GetStat(EStatType.ProjectileCount).currentValue;

        for (int i = 0; i < planets.Count; i++)
        {
            float calculatedSpeed = (i == 0)
                ? projectileSpeed
                : projectileSpeed - (speedSubtractor * i);

            planets[i].speed = calculatedSpeed;
        }
    }

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();
    }

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeSolarSystem();
    }

    private void UpdatePlanetRotation()
    {
        if (player == null || inCooldown) return;

        for (int i = 0; i < activePlanets.Count; i++)
        {
            planets[i].angle += planets[i].speed * Time.deltaTime;
            if (planets[i].angle > 360f) planets[i].angle -= 360f;

            Vector3 offset = Quaternion.Euler(0, 0, planets[i].angle) * Vector3.right * planets[i].radius;
            planets[i].planet.transform.position = player.position + offset;
        }
    }

    private void UpgradeSolarSystem()
    {
        switch (level)
        {
            case 1:
                break;
            case 2:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            case 3:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            case 4:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                statManager.ModifyStatValue(EStatType.ActiveDuration, 2.5f);
                break;
            case 5:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            case 6:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            case 7:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            case 8:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                statManager.ModifyStatValue(EStatType.ActiveDuration, 2.5f);
                break;
            case 9:
                statManager.ModifyStatValue(EStatType.ProjectileCount, 1);
                statManager.ModifyStatValue(EStatType.AOESize, 1);
                break;
            default:
                Debug.Log($"Max Level Reached for {weaponData.weaponName}");
                break;
        }

        for (int i = 0; i < planets.Count; i++)
        {
            if (level >= i + 1)
            {
                if (!activePlanets.Contains(planets[i].planet))
                    activePlanets.Add(planets[i].planet);
            }
        }
    }
}
