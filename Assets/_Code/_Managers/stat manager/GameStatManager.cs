using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameStatManager : MonoBehaviour
{
    public static GameStatManager instance; 
    public GameStat_SO gameStats;

    public static event Action OnStatUpdated;

    #region ===== OFFENSIVE =====

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }

    public void UpdateDamage(float value)
    {
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


    #region ===== DEFENSIVE =====

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


    #region ===== UTILITY =====

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


    #region ===== WEAPONS =====

    public void SetWeapon(int index, Sprite image, int level)
    {
        /*gameStats.SetWeaponData(index, image, level);
        InvokeStatUpdated();*/
    }

    public GameStat_SO.WeaponData GetWeapon(int index)
    {
        return gameStats.GetWeaponData(index);
    }

    #endregion

    


    #region ===== RESET =====

    public void ResetStatValues()
    {
        gameStats.ResetAllValues();
        InvokeStatUpdated();
    }

    #endregion


    private void OnDestroy()
    {
        ResetStatValues();
    }

    private void InvokeStatUpdated()
    {
        OnStatUpdated?.Invoke();
    }
}