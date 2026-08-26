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
        LevelTxt.text = "Lvl."+ "\n" + "1";
        if (data.Description.Length > 0)
        {
            DescriptionTxt.text = data.Description[0];
        }
        else
        {
            DescriptionTxt.text = "No Description!";

        }
    }
}
