using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefactorCardUi : MonoBehaviour
{
    [SerializeField] private Image cardImageRenderer;
    [SerializeField] private CardManager cardManager;
    private ReworkedWeaponManager weaponManager;
    private CardDataSO cardData;
    // Initialize the card with data
    public void Initialize(CardDataSO data, CardManager manager , ReworkedWeaponManager wManager)
    {
        cardData = data;
        cardManager = manager;
        weaponManager = wManager;
        UpdateCardVisuals();
    }
    // Update the card's visual representation
    private void UpdateCardVisuals()
    {
        if (cardData != null && cardData.levelImages.Count > 0)
        {
            cardImageRenderer.sprite = cardData.levelImages[0]; // Assuming level 0 for simplicity
        }
    }

    public void OnCardclicked()
    {
        if (cardData != null && cardManager != null)
        {
            if (cardData.affectsWeaponLevel)
            {
                if(cardData.levelImages.Count > 0)
                    weaponManager.LevelUpWeaponHandled(cardData.weaponName);
                //else if(cardData.levelImages.Count == 0)

            }
            else if (cardData.affectsWeaponStat)
            {
                weaponManager.UpdateWeaponStat(cardData.affectedWeaponStat, cardData.weaponStatModifier);
            }
            cardManager.OnCardSelected(cardData);
        }
    }
}
