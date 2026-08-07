using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RefactorCardUi : MonoBehaviour, IPointerClickHandler
{
    [Header("Card UI")]
    [SerializeField] private Image cardImageRenderer;

    [Header("Card Animation")]
    [SerializeField] private Animator cardAnimator;
    [SerializeField] private string initializeAnimation = "CardIn";

    [Header("References")]
    [SerializeField] private CardManager cardManager;

    private ReworkedWeaponManager weaponManager;
    private CardDataSO cardData;

    // Initialize the card with data
    public void Initialize(
        CardDataSO data,
        CardManager manager,
        ReworkedWeaponManager wManager)
    {
        cardData = data;
        cardManager = manager;
        weaponManager = wManager;

        cardImageRenderer = GetComponent<Image>();

        UpdateCardVisuals();

        // Play card animation when initialized
        if (cardAnimator != null && !string.IsNullOrEmpty(initializeAnimation))
        {
            Debug.Log("---------------Played Anim-----------------");
            cardAnimator.Play(initializeAnimation, 0, 0f);
        }
    }

    // Update the card's visual representation
    private void UpdateCardVisuals()
    {
        if (cardData != null && cardData.levelImages.Count > 0)
        {
            cardImageRenderer.sprite = cardData.levelImages[0];
        }
    }

    public void OnCardclicked()
    {
        if (cardData != null && cardManager != null)
        {
            if (cardData.affectsWeaponLevel)
            {
                if (cardData.levelImages.Count > 0)
                {
                    // Weapon level-up logic
                }
            }
            else if (cardData.affectsWeaponStat)
            {
                // Weapon stat logic
            }

            cardManager.OnCardSelected(cardData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardManager != null && cardData != null)
        {
            cardManager.OnCardSelected(cardData);
        }
    }
}