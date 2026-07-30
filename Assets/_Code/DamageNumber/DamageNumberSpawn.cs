using UnityEngine;
using DamageNumbersPro;
using Sirenix.OdinInspector;
public class DamageNumberSpawn : MonoBehaviour
{
     private HealthSystem healthSystem;

    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1f, 0);

    public DamageNumber DamageNumber;

    private Transform objectTransform;

    private void Start()
    {
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();
        }

        if (healthSystem == null)
        {
            Debug.LogError($"{gameObject.name}: HealthSystem reference is missing!");
            return;
        }

        objectTransform = transform;

        // Subscribe to the damage event
        healthSystem.onDamageTaken.AddListener(OnDamageTaken);
    }
    
    private void OnDamageTaken(int damageAmount)
    {
        DamageNumber.Spawn(transform.position + spawnOffset,(int) damageAmount);
        
    }
    
    [Button("Spawn Damage Number")]
    private void SpawnTest()
    {
        OnDamageTaken(2);
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.onDamageTaken.RemoveListener(OnDamageTaken);
        }
    }
    
}