using System.Collections.Generic;
using UnityEngine;

public class SolarSystemRefactor : WeaponBase
{
    [Header("Solar System Settings")]
    public List<GameObject> planetPrefabs;

    public float minOrbitRadius = 2f;
    public float gapBetweenPlanets = 1.5f;

    [Header("Speed Settings")]
    public float maxOrbitSpeed = 100f;
    public float speedSubtractor = 10f;

    private Transform player;

    private readonly List<PlanetData> planets =
        new List<PlanetData>();


    [System.Serializable]
    private class PlanetData
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

        /*
         * WeaponBase has already initialized the weapon's
         * StatManager.
         *
         * UpdateStatsHandled() calculates the effective local
         * weapon values using:
         *
         * Weapon's own stat
         *          ×
         * Player global multiplier
         *
         * So projectileSpeed here is already the final value.
         */
        UpdateStatsHandled();
    }


    protected override void Start()
    {
        player = transform.parent;

        if (player == null)
        {
            Debug.LogError(
                $"SolarSystem '{name}' could not find player parent."
            );

            return;
        }


        if (planetPrefabs == null ||
            planetPrefabs.Count == 0)
        {
            Debug.LogWarning(
                "No planet prefabs assigned to SolarSystem!"
            );

            return;
        }


        SpawnPlanets();
    }


    // =========================================================
    // SPAWN PLANETS
    // =========================================================

    private void SpawnPlanets()
    {
        // Prevent duplicate planets if this method is ever
        // called again.
        ClearPlanets();


        for (int i = 0; i < planetPrefabs.Count; i++)
        {
            if (planetPrefabs[i] == null)
                continue;


            float angle =
                (360f / planetPrefabs.Count) * i;


            float radius =
                minOrbitRadius +
                gapBetweenPlanets * i;


            float calculatedSpeed =
                CalculatePlanetSpeed(i);


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


            planets.Add(
                new PlanetData
                {
                    planet = planet,
                    angle = angle,
                    speed = calculatedSpeed,
                    radius = radius
                }
            );
        }
    }


    private void ClearPlanets()
    {
        foreach (PlanetData data in planets)
        {
            if (data.planet != null)
            {
                Destroy(data.planet);
            }
        }

        planets.Clear();
    }


    // =========================================================
    // PLANET SPEED
    // =========================================================

    private float CalculatePlanetSpeed(int index)
    {
        /*
         * The first planet uses the weapon's effective
         * ProjectileSpeed.
         *
         * Every following planet is slower by
         * speedSubtractor.
         *
         * Example:
         *
         * Effective ProjectileSpeed = 120
         *
         * Planet 0 = 120
         * Planet 1 = 110
         * Planet 2 = 100
         * Planet 3 = 90
         */
        return projectileSpeed -
               (speedSubtractor * index);
    }


    private void UpdatePlanetSpeeds()
    {
        for (int i = 0; i < planets.Count; i++)
        {
            planets[i].speed =
                CalculatePlanetSpeed(i);
        }
    }


    // =========================================================
    // WEAPON UPDATE
    // =========================================================

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();

        if (player == null)
            return;


        /*
         * Keep all planets orbiting around the player.
         */

        foreach (PlanetData data in planets)
        {
            if (data.planet == null)
                continue;


            data.angle +=
                data.speed *
                Time.deltaTime;


            if (data.angle > 360f)
                data.angle -= 360f;


            Vector3 offset =
                Quaternion.Euler(
                    0f,
                    0f,
                    data.angle
                ) *
                Vector3.right *
                data.radius;


            data.planet.transform.position =
                player.position +
                offset;
        }
    }


    // =========================================================
    // GRAPHICS
    // =========================================================

    public override void ToggleGFXVisibility(bool b)
    {
        foreach (PlanetData data in planets)
        {
            if (data.planet != null)
            {
                data.planet.SetActive(b);
            }
        }
    }


    // =========================================================
    // WEAPON LEVEL UP
    // =========================================================

    public override void LevelUpWeapon()
    {
        /*
         * No Solar System level-specific stat upgrades are
         * currently defined.
         *
         * If you later add upgrades, modify this weapon's
         * StatManager here.
         */
        base.LevelUpWeapon();
    }


    // =========================================================
    // STAT UPDATE
    // =========================================================

    protected override void UpdateStatsHandled()
    {
        /*
         * WeaponBase calculates the final effective values:
         *
         * Solar System's own ProjectileSpeed
         *              ×
         * Player ProjectileSpeed multiplier
         *
         * The resulting value is stored in projectileSpeed.
         */
        base.UpdateStatsHandled();


        /*
         * ProjectileSpeed affects orbit speed, so whenever the
         * player gets a ProjectileSpeed modifier, immediately
         * recalculate every planet's orbit speed.
         */
        UpdatePlanetSpeeds();
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected void OnDestroy()
    {
        ClearPlanets();
    }
}