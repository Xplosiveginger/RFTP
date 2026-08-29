using UnityEngine;

public class PendulumDamageWave : MonoBehaviour
{
    [SerializeField] private float damage = 2f;
    [SerializeField] public float lifetime = 5f;

    public float speed = 5f;

    [Space]

    public bool destroysOnCollisionWithObjects = false;

    private Vector2 direction;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Init(float _damage, Vector2 _direction)
    {
        damage= _damage;
        direction = _direction.normalized;
        
        if(_direction.x<0)
        {
            transform.localScale = new Vector3(-1*transform.localScale.x,transform.localScale.y,transform.localScale.z);

        }


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
            var health = other.gameObject.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.Damage((int)damage);
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
