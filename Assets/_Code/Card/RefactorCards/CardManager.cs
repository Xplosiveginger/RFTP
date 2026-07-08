using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] protected List<CardDataSO> cardDatas;
    private List<CardDataSO> neededCards;
    private List<CardDataSO> weaponLevelCarDatas;
    [SerializeField] protected List<RefactorCardUi> cards;
    protected ReworkedWeaponManager weaponManager;
    
    // Track which cards have been shown in the current selection
    private HashSet<CardDataSO> currentSelectionCards = new HashSet<CardDataSO>();

    public event Action OnCardsInitialized;
    public static event Action<CardDataSO> CardSelected;
    public static event Action CardClicked;

    private void OnEnable()
    {
        XpManager.OnPlayerLeveledUp += CardInitializer;
    }
    
    private void CardInitializer()
    {
        Time.timeScale = 0f;
        
        // Clear the current selection tracking
        currentSelectionCards.Clear();
        
        PopulateCards();

        foreach (var card in cards)
        {
            card.gameObject.SetActive(true);
        }
    }

    private void PopulateCards()
    {
        int cardCount = Mathf.Min(cards.Count, cardDatas.Count);

        // Create a weighted list based on priority
        List<CardDataSO> weightedCardPool = new List<CardDataSO>();

        foreach (var card in cardDatas)
        {
            int weight = 6 - (int)card.cardPriority;
            for (int i = 0; i < weight; i++)
            {
                weightedCardPool.Add(card);
            }
        }

        // Shuffle the weighted list to randomize selection
        Shuffle(weightedCardPool);

        // Clear current selection tracking
        currentSelectionCards.Clear();
        
        for (int i = 0; i < cardCount; i++)
        {
            if (weightedCardPool.Count == 0) break;

            // Get a random card that hasn't been selected yet in this round
            CardDataSO selectedCard = GetUniqueRandomCard(weightedCardPool);

            if (selectedCard != null)
            {
                // Track this card as selected for this round
                currentSelectionCards.Add(selectedCard);
                
                cards[i].Initialize(selectedCard, this, weaponManager);

                // Remove ALL instances of this card from the weighted pool
                weightedCardPool.RemoveAll(c => c == selectedCard);
            }
        }
    }

    // Helper method to get a random card that hasn't been selected in this round
    private CardDataSO GetUniqueRandomCard(List<CardDataSO> cardPool)
    {
        // Create a list of available cards (cards not yet selected in this round)
        List<CardDataSO> availableCards = new List<CardDataSO>();
        HashSet<CardDataSO> uniqueCardsInPool = new HashSet<CardDataSO>();
        
        // Get unique cards from the pool
        foreach (var card in cardPool)
        {
            uniqueCardsInPool.Add(card);
        }
        
        // Filter out cards already selected in this round
        foreach (var card in uniqueCardsInPool)
        {
            if (!currentSelectionCards.Contains(card))
            {
                availableCards.Add(card);
            }
        }
        
        // If no unique cards available, just return null
        if (availableCards.Count == 0)
        {
            Debug.LogWarning("No more unique cards available!");
            return null;
        }
        
        // Return a random card from available cards
        return availableCards[UnityEngine.Random.Range(0, availableCards.Count)];
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int j = UnityEngine.Random.Range(i, n);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // Called by RefactorCardUi when a card is picked
    public void OnCardSelected(CardDataSO selectedData)
    {
        if (cardDatas.Contains(selectedData))
            cardDatas.Remove(selectedData);

        Time.timeScale = 1f; // Resume game

        foreach (var card in cards)
        {
            card.gameObject.SetActive(false);
        }
        
        CardSelected?.Invoke(selectedData);
        CardClicked?.Invoke();
    }

    private void OnDisable()
    {
        XpManager.OnPlayerLeveledUp -= CardInitializer;
    }
}