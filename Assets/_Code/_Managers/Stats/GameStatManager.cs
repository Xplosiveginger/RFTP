using System;
using UnityEngine;

public class GameStatManager : MonoBehaviour
{

    [SerializeField] private GameStat_SO gameStats;

    public  event Action OnStatUpdated;

    #region Offensive

    public void UpdateDamage(float value)
    {
        // place calculations here if needed
        gameStats.damage = value;
        InvokeStatUpdated();
    }

    public void UpdateAOE(float value)
    {
        gameStats.areaOfEffect = value;
        InvokeStatUpdated();
    }

    public void UpdateProjectileSpeed(float value)
    {
        gameStats.projectileSpeed = value;
        InvokeStatUpdated();
    }

    public void UpdateNumberOfProjectiles(float value)
    {
        gameStats.numberOfProjectiles = value;
        InvokeStatUpdated();
    }

    public void UpdateDuration(float value)
    {
        gameStats.duration = value;
        InvokeStatUpdated();
    }

    #endregion


    #region Defensive

    public void UpdateTotalHealth(float value)
    {
        gameStats.totalHealth = value;
        InvokeStatUpdated();
    }

    public void UpdateHealthRegen(float value)
    {
        gameStats.healthRegen = value;
        InvokeStatUpdated();
    }

    #endregion


    #region Utility

    public void UpdateCooldown(float value)
    {
        gameStats.cooldown = value;
        InvokeStatUpdated();
    }

    public void UpdateMoveSpeed(float value)
    {
        gameStats.moveSpeed = value;
        InvokeStatUpdated();
    }

    #endregion


    #region Reset

    public void ResetStatValues()
    {
        gameStats.ResetValues();
        InvokeStatUpdated();
    }

    #endregion


    private void InvokeStatUpdated()
    {
        OnStatUpdated?.Invoke();
    }
}