using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RefactorCardUi : MonoBehaviour, IPointerClickHandler
{
    [SerializeField, ReadOnly] private CardManager cardManager;
    private ReworkedWeaponManager weaponManager;
    [SerializeField] private CardDataSO cardData;
    [SerializeField] private GameStat_SO gameStatSO;

    [Header("UIs")] 
    public Image icon;

    public TextMeshProUGUI NameTxt;

    public TextMeshProUGUI DescriptionTxt;

    public TextMeshProUGUI LevelTxt;
    
    // Initialize the card with data
    public void Initialize(CardDataSO data, CardManager manager , ReworkedWeaponManager wManager)
    {
        cardData = data;
        cardManager = manager;
        weaponManager = wManager;
        //UpdateCardVisuals();
        UpdateCardUI(data);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //cardManager.OnCardSelected(cardData);
    }
    public void OnCardSelected()
    {
        cardManager.OnCardSelected(cardData); 
    }

    public void UpdateCardUI(CardDataSO data)
    {
        icon.sprite = data.Icon;
        NameTxt.text = data.Name;
        int level = gameStatSO.getItemLevel(data);
        if (LevelTxt!=null)
        {
            LevelTxt.text =(level+1).ToString();
        }

        if (data.Description.Length > 0)  //this needs to be the value of the level in item
        {
            int max = Mathf.Min(data.Description.Length, level);
            DescriptionTxt.text = data.Description[max];
        }
        else
        {
            DescriptionTxt.text = "No Description!";

        }
    }
}
