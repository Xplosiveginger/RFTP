using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : WeaponBase
{
    [Header("Solar System Settings")]
    public List<GameObject> planetPrefabs;

    public float minOrbitRadius = 2f;
    public float gapBetweenPlanets = 1.5f;


    [Header("Speed Settings")]
    public float maxOrbitSpeed = 100f;
    public float speedSubtractor = 10f;


    [Header("Damage Settings")]
    public int damageIncWithLvl = 5;


    private Transform player;

    public List<PlanetData> planets =
        new List<PlanetData>();

    public List<GameObject> activePlanets =
        new List<GameObject>();


    [System.Serializable]
    public class PlanetData
    {
        public GameObject planet;
        public float angle;
        public float speed;
        public float radius;
    }


    // =========================================================
    // UNITY LIFECYCLE
    // =========================================================

    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start()
    {
        base.Start();

        player = transform.parent;

        if (planetPrefabs == null ||
            planetPrefabs.Count == 0)
        {
            Debug.LogWarning(
                "No planet prefabs assigned to SolarSystem!"
            );

            return;
        }

        SpawnPlanets();

        UpdatePlanetVisibility();
    }


    // =========================================================
    // SPAWN WEAPON
    // =========================================================

    public override void SpawnWeapon(
        Transform parent)
    {
        base.SpawnWeapon(parent);
    }


    // =========================================================
    // SPAWN PLANETS
    // =========================================================

    private void SpawnPlanets()
    {
        ClearPlanets();

        int planetCount =
            planetPrefabs.Count;


        for (int i = 0;
             i < planetCount;
             i++)
        {
            if (planetPrefabs[i] == null)
                continue;


            float angle =
                (360f / planetCount) * i;


            float radius =
                minOrbitRadius +
                gapBetweenPlanets * i;


            // First planet gets max speed.
            // Every following planet is slower.
            float calculatedSpeed =
                (i == 0)
                    ? maxOrbitSpeed
                    : maxOrbitSpeed -
                      (speedSubtractor * i);


            Vector3 position =
                player.position +
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                ) *
                Vector3.right *
                radius;


            GameObject planet =
                Instantiate(
                    planetPrefabs[i],
                    position,
                    Quaternion.identity,
                    transform
                );


            PlanetData data =
                new PlanetData
                {
                    planet = planet,
                    angle = angle,
                    speed = calculatedSpeed,
                    radius = radius
                };


            planets.Add(data);
        }
    }


    private void ClearPlanets()
    {
        foreach (PlanetData data in planets)
        {
            if (data != null &&
                data.planet != null)
            {
                Destroy(data.planet);
            }
        }

        planets.Clear();
        activePlanets.Clear();
    }


    // =========================================================
    // WEAPON UPDATE
    // =========================================================

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();

        UpdatePlanetRotation();
    }


    // =========================================================
    // GRAPHICS
    // =========================================================

    public override void ToggleGFXVisibility(
        bool visible)
    {
        if (activePlanets == null)
            return;

        foreach (GameObject planet in activePlanets)
        {
            if (planet != null)
            {
                planet.SetActive(visible);
            }
        }
    }


    // =========================================================
    // STAT UPDATES
    // =========================================================

    protected override void UpdateStatsHandled()
    {
        // IMPORTANT:
        //
        // Read ALL common weapon stats from this weapon's
        // own StatManager.
        //
        // This includes:
        // - Damage
        // - Projectile count
        // - AOE size
        // - Cooldown
        // - Duration
        // - Projectile speed
        // - Fire rate
        //
        base.UpdateStatsHandled();


        // Projectile count may have changed because of:
        //
        // - Item
        // - Weapon level
        //
        // So update which planets are active.
        UpdatePlanetVisibility();


        // Keep currently active planets visible when
        // the weapon is active.
        if (isActive)
        {
            ToggleGFXVisibility(true);
        }
    }


    // =========================================================
    // DAMAGE
    // =========================================================

    public override void UpdateWeaponDamage()
    {
        base.UpdateWeaponDamage();
    }


    // =========================================================
    // LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        base.LevelUpWeapon();

        UpgradeSolarSystem();
    }


    private void UpgradeSolarSystem()
    {
        switch (level)
        {
            // -------------------------------------------------
            // LEVEL 1
            // -------------------------------------------------

            case 1:
                break;


            // -------------------------------------------------
            // LEVEL 2
            // -------------------------------------------------

            case 2:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // LEVEL 3
            // -------------------------------------------------

            case 3:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // LEVEL 4
            // -------------------------------------------------

            case 4:

                AddPlanetLevelStats();

                statManager.ModifyStatValue(
                    EStatType.ActiveDuration,
                    2.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 5
            // -------------------------------------------------

            case 5:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // LEVEL 6
            // -------------------------------------------------

            case 6:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // LEVEL 7
            // -------------------------------------------------

            case 7:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // LEVEL 8
            // -------------------------------------------------

            case 8:

                AddPlanetLevelStats();

                statManager.ModifyStatValue(
                    EStatType.ActiveDuration,
                    2.5f
                );

                break;


            // -------------------------------------------------
            // LEVEL 9
            // -------------------------------------------------

            case 9:

                AddPlanetLevelStats();

                break;


            // -------------------------------------------------
            // MAX LEVEL
            // -------------------------------------------------

            default:

                Debug.Log(
                    $"Max Level Reached for " +
                    $"{weaponData.weaponName}"
                );

                break;
        }


        UpdatePlanetVisibility();
    }


    private void AddPlanetLevelStats()
    {
        statManager.ModifyStatValue(
            EStatType.ProjectileCount,
            1f
        );

        /*statManager.ModifyStatValue(
            EStatType.AOESize,
            1f
        );*/
    }


    // =========================================================
    // PLANET VISIBILITY
    // =========================================================

    private void UpdatePlanetVisibility()
    {
        if (planets == null ||
            planets.Count == 0)
        {
            return;
        }


        /*
         * ProjectileCount controls the number of planets.
         *
         * Example:
         *
         * ProjectileCount = 1
         * → Planet 1
         *
         * ProjectileCount = 2
         * → Planet 1 + 2
         *
         * ProjectileCount = 3
         * → Planet 1 + 2 + 3
         */


        int numberOfActivePlanets =
            Mathf.Clamp(
                Mathf.RoundToInt(projectileCount),
                0,
                planets.Count
            );


        activePlanets.Clear();


        for (int i = 0;
             i < planets.Count;
             i++)
        {
            if (planets[i] == null ||
                planets[i].planet == null)
            {
                continue;
            }


            bool shouldBeActive =
                i < numberOfActivePlanets;


            planets[i].planet.SetActive(
                shouldBeActive
            );


            if (shouldBeActive)
            {
                activePlanets.Add(
                    planets[i].planet
                );
            }
        }
    }


    // =========================================================
    // PLANET ROTATION
    // =========================================================

    private void UpdatePlanetRotation()
    {
        if (player == null ||
            inCooldown)
        {
            return;
        }


        for (int i = 0;
             i < activePlanets.Count;
             i++)
        {
            if (i >= planets.Count)
                break;


            PlanetData planetData =
                planets[i];


            if (planetData == null ||
                planetData.planet == null)
            {
                continue;
            }


            planetData.angle +=
                planetData.speed *
                Time.deltaTime;


            if (planetData.angle > 360f)
            {
                planetData.angle -= 360f;
            }


            Vector3 offset =
                Quaternion.Euler(
                    0f,
                    0f,
                    planetData.angle
                ) *
                Vector3.right *
                planetData.radius;


            planetData.planet.transform.position =
                player.position + offset;
        }
    }
}