using System;
using UnityEngine;

public class Pendulum_Wave : MonoBehaviour
{
    [SerializeField] private float cooldownReduction = 2f;
    [SerializeField] public float lifetime = 5f;
    [SerializeField] public float debuffDuration = 5f;

    public float speed = 5f;

    [Space]

    public bool destroysOnCollisionWithObjects = false;

    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Init(float _cooldownReduction, Vector2 _direction, float _debuffDuration)
    {
        cooldownReduction = _cooldownReduction;
        direction = _direction.normalized;
        this.debuffDuration= _debuffDuration;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Logic to reduce the cooldown of the weapon
            ReworkedWeaponManager weaponManager =
                other.GetComponent<ReworkedWeaponManager>();

            if (weaponManager != null)
            {
                weaponManager.ReduceCoolodwnOfFirstWeapon(cooldownReduction,debuffDuration);
            }

            Destroy(gameObject);
        }
        else
        {
            if (destroysOnCollisionWithObjects)
                Destroy(gameObject);
        }
    }
}