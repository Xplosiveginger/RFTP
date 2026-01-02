using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefactorCardUi : MonoBehaviour
{
    [SerializeField] private Image cardImageRenderer;
    [SerializeField] private CardManager cardManager;
    private CardDataSO cardData;
    // Initialize the card with data
    public void Initialize(CardDataSO data, CardManager manager)
    {
        cardData = data;
        cardManager = manager;
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
}
