using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemRefactor : WeaponBase
{
    [Header("Solar System Settings")]
    public List<GameObject> planetPrefabs; // Assign different planet prefabs in Inspector
    public float minOrbitRadius = 2f;      // Radius of the first planet
    public float gapBetweenPlanets = 1.5f; // Constant gap between each orbit

    [Header("Speed Settings")]
    public float maxOrbitSpeed = 100f;     // Speed of first planet
    public float speedSubtractor = 10f;    // How much slower each next planet gets

    private Transform player;
    private readonly List<PlanetData> planets = new List<PlanetData>();

    [System.Serializable]
    private class PlanetData
    {
        public GameObject planet;
        public float angle;
        public float speed;
        public float radius;
    }

    protected override void Awake()
    {
        base.Awake();

        UpdateStatsHandled();
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
        }
    }

    public override void UpdateWeapon()
    {
        base.UpdateWeapon();

        foreach (var data in planets)
        {
            data.angle += data.speed * Time.deltaTime;
            if (data.angle > 360f) data.angle -= 360f;

            Vector3 offset = Quaternion.Euler(0, 0, data.angle) * Vector3.right * data.radius;
            data.planet.transform.position = player.position + offset;
        }
    }

    public override void ToggleGFXVisibility(bool b)
    {
        foreach (var planet in planets)
        {
            planet.planet.SetActive(b);
        }
    }

    protected override void LevelUpWeapon()
    {
        
    }
}
