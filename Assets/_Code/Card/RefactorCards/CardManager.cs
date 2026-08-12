using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct CardCategoryData
{
    public ECardCategory cardCategory;
    public List<CardDataSO> cardDataList;
}

public class CardManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip levelUpSound;
    public AudioClip cardSelectionSound;
    
    [Header("Status Panel")]
    [SerializeField] private GameObject statusPanel;
    [Header("Level Up Image")]
    [SerializeField] private GameObject levelUpImage;

    [Header("Level Up Animation")]
    [SerializeField] private Animator levelUpImageAnimator;
    [SerializeField] private string levelUpAnimation = "LevelUp";

    [Header("Status Panel Animation")]
    [SerializeField] private Animator statusPanelAnimator;
    [SerializeField] private float statusAnimationDuration = 0.5f;

    private Coroutine statusPanelRoutine;
    [Header("Status Panel Stats")]
    [SerializeField] private TMPro.TextMeshProUGUI damageText;
    [SerializeField] private TMPro.TextMeshProUGUI totalHealthText;
    [SerializeField] private TMPro.TextMeshProUGUI healthRegenText;
    [SerializeField] private TMPro.TextMeshProUGUI cooldownText;
    [SerializeField] private TMPro.TextMeshProUGUI aoeText;
    [SerializeField] private TMPro.TextMeshProUGUI speedOfWeaponText;
    [SerializeField] private TMPro.TextMeshProUGUI durationText;
    [SerializeField] private TMPro.TextMeshProUGUI numOfProjectilesText;
    [SerializeField] private TMPro.TextMeshProUGUI moveSpeedText;
    
    [Header("Card Categories")]
    [SerializeField] protected List<CardCategoryData> cardCategories;
    
    [Header("Card UI References")]
    [SerializeField] protected List<RefactorCardUi> cards;
    
    [Header("Game Data")]
    [SerializeField] private GameStat_SO gameStatSO;
    
    [Header("Weapon Manager")]
    protected ReworkedWeaponManager weaponManager;
    
    // Track which cards have been shown in the current selection
    private HashSet<CardDataSO> currentSelectionCards = new HashSet<CardDataSO>();
    private HealthSystem PlayerHealth;
    private StatManager StatManager;
    public event Action OnCardsInitialized;
    public static event Action<CardDataSO> CardSelected;
    public static event Action CardClicked;

    private void Awake()
    {
        if (statusPanelAnimator != null)
        {
            statusPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        PlayerHealth = SM.Instance.Player.GetComponent<HealthSystem>();
        StatManager = SM.Instance.Player.GetComponent<StatManager>();
    }
    private void OnEnable()
    {
        XpManager.OnPlayerLeveledUp += CardInitializer;
        
        // Optional: Subscribe to weapon name updates for debugging
        if (gameStatSO != null)
        {
            gameStatSO.OnEquippedWeaponNamesUpdated += OnEquippedWeaponNamesUpdatedHandler;
        }
    }
    private void PlayLevelUpAnimation()
    {
        if (levelUpImageAnimator == null)
            return;

        if (string.IsNullOrEmpty(levelUpAnimation))
            return;

        // Make sure the Animator is reset before replaying
        levelUpImageAnimator.Rebind();
        levelUpImageAnimator.Update(0f);

        levelUpImageAnimator.Play(
            levelUpAnimation,
            0,
            0f
        );
    }
    
    private void OnEquippedWeaponNamesUpdatedHandler(List<EWeaponName> weaponNames)
    {
        Debug.Log($"CardManager notified of weapon name update: [{string.Join(", ", weaponNames)}]");
    }
    private void UpdateStatusPanelStats()
    {
        if (gameStatSO == null)
            return;

        // Replace these with the actual stat references
        // exposed by your GameStat_SO.
        if (StatManager != null)
        {
            if((StatManager.GetStat(EStatType.Damage)!=null))
                damageText.text = (StatManager.GetStat(EStatType.Damage).currentValue).ToString();
            
            
            if(StatManager.GetStat(EStatType.HealthRegen)!=null)
                healthRegenText.text = (StatManager.GetStat(EStatType.HealthRegen).currentValue).ToString();
        
            if((StatManager.GetStat(EStatType.MoveSpeed)!=null))
                moveSpeedText.text = ((StatManager.GetStat(EStatType.MoveSpeed).currentValue)).ToString();
            
            if((StatManager.GetStat(EStatType.AOESize)!=null))
                aoeText.text = (StatManager.GetStat(EStatType.AOESize).currentValue).ToString();
            
            if(StatManager.GetStat(EStatType.ProjectileSpeed)!=null)
                speedOfWeaponText.text = (StatManager.GetStat(EStatType.ProjectileSpeed).currentValue).ToString();
            
            if((StatManager.GetStat(EStatType.ProjectileCount)!=null))
                numOfProjectilesText.text = (StatManager.GetStat(EStatType.ProjectileCount)).currentValue.ToString();
            
            if((StatManager.GetStat(EStatType.AttackCooldown)!=null))
                cooldownText.text = (StatManager.GetStat(EStatType.AttackCooldown)).currentValue.ToString();  
            
            if((StatManager.GetStat(EStatType.ActiveDuration)!=null))
                durationText.text = (StatManager.GetStat(EStatType.ActiveDuration)).currentValue.ToString();
            
            
        }
    }
    
    private void CardInitializer()
    {
        // Stop any previous status animation
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }
        audioSource.PlayOneShot(levelUpSound);
        Time.timeScale = 0f;

        currentSelectionCards.Clear();

        IDCardManager.instance.ShowPauseUI();

        // Show status panel
        ShowStatusPanel();

        // Show level-up image
        if (levelUpImage != null)
        {
            levelUpImage.SetActive(true);

            // Play the one-shot animation from the beginning
            PlayLevelUpAnimation();
        }

        PopulateCards();

        foreach (var card in cards)
        {
            card.gameObject.SetActive(true);
        }
    }
    private void PopulateCards()
    {
        // Step 1: Accumulate all qualified candidates from all categories with relevancy filtering
        List<CardDataSO> qualifiedCandidates = GetAllQualifiedCandidates();
        
        // Step 2: Randomize the qualified candidates list
        Shuffle(qualifiedCandidates);
        
        // Step 3: Determine how many cards to show
        int cardCount = Mathf.Min(cards.Count, qualifiedCandidates.Count);
        
        // Clear current selection tracking
        currentSelectionCards.Clear();
        
        Debug.Log($"Populating {cardCount} cards from {qualifiedCandidates.Count} qualified candidates");
        
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

    // Get all qualified candidates from all categories with relevancy filtering
    private List<CardDataSO> GetAllQualifiedCandidates()
    {
        List<CardDataSO> allCandidates = new List<CardDataSO>();
        
        foreach (var category in cardCategories)
        {
            if (category.cardDataList != null)
            {
                foreach (var card in category.cardDataList)
                {
                    if (IsCardRelevant(card))
                    {
                        allCandidates.Add(card);
                    }
                }
            }
        }
        
        Debug.Log($"Total qualified candidates after relevancy filter: {allCandidates.Count}");
        return allCandidates;
    }

    /// <summary>
    /// Determines if a card is relevant based on currently equipped weapons from GameStatSO
    /// </summary>
    private bool IsCardRelevant(CardDataSO card)
    {
        // Get equipped weapon names directly from GameStatSO
        List<EWeaponName> equippedWeapons = gameStatSO.GetEquippedWeaponNames();
        
        switch (card.cardType)
        {
            case ECardType.AffectsPlayer:
                // Player related cards are always included
                return true;
                
            case ECardType.AffectsEnemy:
                // Enemy affecting cards are always included
                return true;
                
            case ECardType.AddsWeapon:
                // Check if the weapon to add is already equipped
                if (card.weaponToAdd != null)
                {
                    bool isAlreadyEquipped = gameStatSO.IsWeaponEquipped(card.weaponToAdd.weaponName);
                    if (isAlreadyEquipped)
                    {
                        Debug.Log($"Card {card.name} filtered out - weapon {card.weaponToAdd.weaponName} already equipped");
                        return false;
                    }
                    return true;
                }
                // If no weapon to add specified, include it
                return true;
                
            case ECardType.AffectsWeaponLevel:
                // Only include cards for currently equipped weapons
                bool isEquipped = gameStatSO.IsWeaponEquipped(card.weaponName);
                if (!isEquipped)
                {
                    Debug.Log($"Card {card.name} filtered out - weapon {card.weaponName} not equipped");
                }
                return isEquipped;
                
            case ECardType.AffectsSpecificWeaponStat:
                // Only include cards for currently equipped weapons
                bool isWeaponEquipped = gameStatSO.IsWeaponEquipped(card.weaponName);
                if (!isWeaponEquipped)
                {
                    Debug.Log($"Card {card.name} filtered out - weapon {card.weaponName} not equipped");
                }
                return isWeaponEquipped;
                
            case ECardType.AffectsAllWeaponsStat:
                // Cards that affect all weapons are relevant if any weapon is equipped
                bool hasAnyWeapon = equippedWeapons.Count > 0;
                if (!hasAnyWeapon)
                {
                    Debug.Log($"Card {card.name} filtered out - no weapons equipped");
                }
                return hasAnyWeapon;
                
            default:
                // Default to include unknown card types
                Debug.LogWarning($"Unknown card type {card.cardType} for card {card.name}, including by default");
                return true;
        }
    }

    // Get cards from a specific category
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
        RemoveCardFromCategories(selectedData);

        IDCardManager.instance.ShowGameplayUI();

        if (levelUpImage != null)
            levelUpImage.SetActive(false);

        foreach (var card in cards)
        {
            card.gameObject.SetActive(false);
        }

        CardSelected?.Invoke(selectedData);
        audioSource.PlayOneShot(cardSelectionSound);
        CardClicked?.Invoke();

        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
        }

        statusPanelRoutine = StartCoroutine(
            HideStatusAndResumeRoutine()
        );
    }
    private IEnumerator HideStatusAndResumeRoutine()
    {
        if (statusPanel != null && statusPanelAnimator != null)
        {
            statusPanelAnimator.Play("SlideOut", 0, 0f);

            yield return new WaitForSecondsRealtime(
                statusAnimationDuration
            );

            statusPanel.SetActive(false);
        }
        else if (statusPanel != null)
        {
            statusPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        statusPanelRoutine = null;
    }
    // Remove a card from all categories
    private void RemoveCardFromCategories(CardDataSO cardToRemove)
    {
        for (int i = 0; i < cardCategories.Count; i++)
        {
            if (cardCategories[i].cardDataList != null && cardCategories[i].cardDataList.Contains(cardToRemove))
            {
                cardCategories[i].cardDataList.Remove(cardToRemove);
                Debug.Log($"Removed card {cardToRemove.name} from category {cardCategories[i].cardCategory}");
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
    private void ShowStatusPanel()
    {
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }

        if (statusPanel == null)
            return;

        UpdateStatusPanelStats();

        statusPanel.SetActive(true);

        if (statusPanelAnimator != null)
        {
            statusPanelAnimator.Play("SlideIn", 0, 0f);
        }
    }
    private void OnDisable()
    {
        if (statusPanelRoutine != null)
        {
            StopCoroutine(statusPanelRoutine);
            statusPanelRoutine = null;
        }

        XpManager.OnPlayerLeveledUp -= CardInitializer;

        if (gameStatSO != null)
        {
            gameStatSO.OnEquippedWeaponNamesUpdated -=
                OnEquippedWeaponNamesUpdatedHandler;
        }
    }
}