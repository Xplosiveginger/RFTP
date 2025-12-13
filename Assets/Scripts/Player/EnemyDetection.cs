using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public List<Enemy> enemyList;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
            enemyList.Add(collision.gameObject.GetComponent<Enemy>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(enemyList.Contains(collision.gameObject.GetComponent<Enemy>()))
                enemyList.Remove(collision.gameObject.GetComponent<Enemy>());
        }
    }
}
