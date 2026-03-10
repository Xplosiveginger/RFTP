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

    public event Action OnCardsInitialized;
    public static event Action<CardDataSO> CardSelected;
    public static event Action CardClicked;

    private void OnEnable()
    {
        PlayerXp.OnPlayerLeveledUp += CardInitializer;
        //PlayerXPRefactored.OnLeveledUp += CardInitializer;

        //foreach (var card in cards)
        //{
        //    card.OnCardSelected += OnCardSelectedHandled;
        //}
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.P))
    //    {
    //        OnCardsInitialized?.Invoke();
    //    }
    //}

    // Will populate the cards and stop time in the game until player picks a card
    private void CardInitializer()
    {
        Time.timeScale = 0f; 
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

        for (int i = 0; i < cardCount; i++)
        {
            if (weightedCardPool.Count == 0) break;

            CardDataSO selectedCard = weightedCardPool[UnityEngine.Random.Range(0, weightedCardPool.Count)];

            cards[i].Initialize(selectedCard, this , weaponManager);

            weightedCardPool.RemoveAll(c => c == selectedCard);
        }
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
        PlayerXp.OnPlayerLeveledUp -= CardInitializer;
        //PlayerXPRefactored.OnLeveledUp -= CardInitializer;

        //foreach (var card in cards)
        //{
        //    card.OnCardSelected -= OnCardSelectedHandled;
        //}
    }
}
