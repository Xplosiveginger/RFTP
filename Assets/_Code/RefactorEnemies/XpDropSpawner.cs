using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class XpDropSpawner : MonoBehaviour
{
    [Title("XP Drop")]

    [SerializeField, Min(1)]
    private int xpAmount = 1;

    [SerializeField, Min(0f)]
    private float dropRadius = 1f;

    private HealthSystem healthSystem;
    private GameObject xpDropPrefab;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError($"{name}: HealthSystem not found.");
            enabled = false;
            return;
        }

        xpDropPrefab = Resources.Load<GameObject>("XpDrop");

        if (xpDropPrefab == null)
        {
            Debug.LogError("Resources/XpDrop.prefab could not be found.");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        healthSystem.onDeath.AddListener(DropXp);
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            healthSystem.onDeath.RemoveListener(DropXp);
    }

    private void DropXp()
    {
        Vector2 offset = Random.insideUnitCircle * dropRadius;
        Vector3 spawnPosition = transform.position + new Vector3(offset.x, 0f, offset.y);

        GameObject xpObject = Instantiate(xpDropPrefab, spawnPosition, Quaternion.identity);

        XpDrop xpDrop = xpObject.GetComponent<XpDrop>();

        if (xpDrop != null)
        {
            xpDrop.SetXpAmount(xpAmount);
        }
        else
        {
            Debug.LogWarning("XpDrop component missing on XpDrop prefab.");
        }
    }

    [Button]
    private void TestDropXp()
    {
        DropXp();
    }
}