using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardCategoryData
{
    public ECardCategory cardCategory;
    public List<CardDataSO> cardDataList;
}

public class CardManager : MonoBehaviour
{
    [SerializeField] protected List<CardCategoryData> cardCategories;
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
        // Step 1: Accumulate all qualified candidates from all categories
        List<CardDataSO> qualifiedCandidates = GetAllQualifiedCandidates();
        
        // Step 2: Randomize the qualified candidates list
        Shuffle(qualifiedCandidates);
        
        // Step 3: Determine how many cards to show
        int cardCount = Mathf.Min(cards.Count, qualifiedCandidates.Count);
        
        // Clear current selection tracking
        currentSelectionCards.Clear();
        
        // Step 4: Select cards from the randomized list
        for (int i = 0; i < cardCount; i++)
        {
            if (qualifiedCandidates.Count == 0) break;

            // Get a unique random card from the qualified candidates
            CardDataSO selectedCard = GetUniqueRandomCard(qualifiedCandidates);

            if (selectedCard != null)
            {
                // Track this card as selected for this round
                currentSelectionCards.Add(selectedCard);
                
                // Initialize the card UI
                cards[i].Initialize(selectedCard, this, weaponManager);

                // Remove the selected card from the candidates to prevent duplicates
                qualifiedCandidates.RemoveAll(c => c == selectedCard);
            }
        }
    }

    // Get all qualified candidates from all categories (no filtering for now)
    private List<CardDataSO> GetAllQualifiedCandidates()
    {
        List<CardDataSO> allCandidates = new List<CardDataSO>();
        
        foreach (var category in cardCategories)
        {
            if (category.cardDataList != null)
            {
                allCandidates.AddRange(category.cardDataList);
            }
        }
        
        return allCandidates;
    }

    // Get cards from a specific category (useful for future filtering)
    public List<CardDataSO> GetCardsByCategory(ECardCategory category)
    {
        foreach (var cardCategory in cardCategories)
        {
            if (cardCategory.cardCategory == category)
            {
                return cardCategory.cardDataList;
            }
        }
        
        return new List<CardDataSO>();
    }

    // Helper method to get a random card that hasn't been selected in this round
    private CardDataSO GetUniqueRandomCard(List<CardDataSO> cardPool)
    {
        // Create a list of available cards (cards not yet selected in this round)
        List<CardDataSO> availableCards = new List<CardDataSO>();
        
        // Get unique cards from the pool that haven't been selected yet
        foreach (var card in cardPool)
        {
            if (!currentSelectionCards.Contains(card) && !availableCards.Contains(card))
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
        // Remove the selected card from its category
        RemoveCardFromCategories(selectedData);

        Time.timeScale = 1f; // Resume game

        foreach (var card in cards)
        {
            card.gameObject.SetActive(false);
        }
        
        CardSelected?.Invoke(selectedData);
        CardClicked?.Invoke();
    }

    // Remove a card from all categories
    private void RemoveCardFromCategories(CardDataSO cardToRemove)
    {
        for (int i = 0; i < cardCategories.Count; i++)
        {
            if (cardCategories[i].cardDataList != null && cardCategories[i].cardDataList.Contains(cardToRemove))
            {
                cardCategories[i].cardDataList.Remove(cardToRemove);
                break; // Card found and removed, exit loop
            }
        }
    }

    // Method to add a card to a specific category (useful for future dynamic card addition)
    public void AddCardToCategory(ECardCategory category, CardDataSO cardToAdd)
    {
        for (int i = 0; i < cardCategories.Count; i++)
        {
            if (cardCategories[i].cardCategory == category)
            {
                cardCategories[i].cardDataList.Add(cardToAdd);
                return;
            }
        }
        
        // If category doesn't exist, create it
        CardCategoryData newCategory = new CardCategoryData
        {
            cardCategory = category,
            cardDataList = new List<CardDataSO> { cardToAdd }
        };
        
        cardCategories.Add(newCategory);
    }

    private void OnDisable()
    {
        XpManager.OnPlayerLeveledUp -= CardInitializer;
    }
}