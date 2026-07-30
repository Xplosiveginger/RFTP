using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ItemUIStruct
{
    public Image itemImage;
    public Image[] levelObject;
}


[System.Serializable]
public struct StatsStruct
{
    public Image StatImage;
    public string name;
    public string value;
}


public class PauseData_UI : MonoBehaviour
{
    public GameStat_SO SO;


    public Color NormalColor, LevelColor;

    #region weapond UI
    public ItemUIStruct weapon1;
    
    public ItemUIStruct[] _ItemUI;
    #endregion

    #region stat
    public StatsStruct Damage;
    #endregion




    #region methods 
    private void OnEnable()
    {

        if (SO == null)
            return;

        UpdateWeaponUI();

    }

    #endregion

    #region UI View 

    [ContextMenu("update weapon UI")]
    public void UpdateWeaponUI()
    {
    }




    #endregion


}
