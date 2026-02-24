using System.Collections;
using UnityEngine;

public class TuningFork : MonoBehaviour
{
    [Header("Freeze time")]
    public float freezeTime = 1f;
    [SerializeField, ] private float size;
    [SerializeField] private TuningForkRefactored tuning;
    [SerializeField] private Collider2D hitbox;

    private void UpdateStats()
    {
        size = tuning.statManager.GetStat(EStatType.AOESize).currentValue;
    }

    private void OnEnable()
    {
        UpdateStats();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BaseEnemyRefactor>(); //add Freeze(freezeTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size, size, size));
    }
}