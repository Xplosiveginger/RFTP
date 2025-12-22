using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public List<BaseEnemyRefactor> enemyList;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
            enemyList.Add(collision.gameObject.GetComponent<BaseEnemyRefactor>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(enemyList.Contains(collision.gameObject.GetComponent<BaseEnemyRefactor>()))
                enemyList.Remove(collision.gameObject.GetComponent<BaseEnemyRefactor>());
        }
    }

    public Vector3 GetPositionOfRandomEnemy()
    {
        if (enemyList.Count == 0) return Vector3.zero;
        return enemyList[Random.Range(0, enemyList.Count)].gameObject.transform.position;
    }
}
