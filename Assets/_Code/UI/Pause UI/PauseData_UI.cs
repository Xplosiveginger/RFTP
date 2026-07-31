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
        weapon1.itemImage.sprite = SO.weapon1.image;

        for (int i = 0; i < weapon1.levelObject.Length; i++)
        {
            if (i < SO.weapon1.level)
            {
                weapon1.levelObject[i].color = LevelColor;
                continue;
            }
            weapon1.levelObject[i].color = NormalColor;
        }

    }




    #endregion


}
