using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public List<BaseEnemyRefactor> enemyList;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();

            if (enemy != null && !enemyList.Contains(enemy))
            {
                enemyList.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            BaseEnemyRefactor enemy = collision.gameObject.GetComponent<BaseEnemyRefactor>();

            if (enemy != null)
            {
                enemyList.Remove(enemy);
            }
        }
    }

    public Vector3 GetPositionOfRandomEnemy()
    {
        if (enemyList.Count == 0)
            return Vector3.zero;

        return enemyList[Random.Range(0, enemyList.Count)].transform.position;
    }

    public Vector3 GetPositionOfNearestEnemy()
    {
        if (enemyList.Count == 0)
            return Vector3.zero;

        Vector3 playerPosition = transform.position;

        float closestSqrDistance = float.MaxValue;
        Vector3 nearestEnemyPosition = Vector3.zero;

        for (int i = 0; i < enemyList.Count; i++)
        {
            BaseEnemyRefactor enemy = enemyList[i];

            if (enemy == null)
                continue;

            Vector3 enemyPosition = enemy.transform.position;
            float sqrDistance = (enemyPosition - playerPosition).sqrMagnitude;

            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                nearestEnemyPosition = enemyPosition;
            }
        }

        return nearestEnemyPosition;
    }
}