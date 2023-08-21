using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject cardPrefab;  // The Card Display prefab
    public Transform handTransform;  // Parent transform where cards will be added
    public Deck mainDeck;  // Reference to the main deck
    public bool isPlayerHand = true;  // Determine if this is a player's hand
    public Transform discardPileTransform;
    public PlayerHandManager playerHandManager;
    public TheGameManager gameManager;  // Add a reference to TheGameManager

    public List<Card> currentHand = new List<Card>(); // Store the current hand cards data

    private void Start()
    {
        // Try to find the GameManager if it wasn't set in the inspector
        if (!gameManager)
        {
            gameManager = FindObjectOfType<TheGameManager>();
        }
        DealInitialCards();
    }

    public void AddCardToHand(Card cardData)
    {
        GameObject newCard = Instantiate(cardPrefab, handTransform);
        
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardData = cardData;
        
        currentHand.Add(cardData); // Add the card data to the current hand list

        if (isPlayerHand)
        {
            cardDisplay.UpdateCardUI(cardDisplay.cardData);
            newCard.tag = "PlayerCard";
            CardDragHandler dragHandler = newCard.AddComponent<CardDragHandler>();
            dragHandler.discardPileTransform = discardPileTransform;
            dragHandler.playerHandManager = playerHandManager;
            dragHandler.gameManager = gameManager;  // Set the reference to the game manager
        }
        else
        {
            cardDisplay.HideCard();
        }
    }

    public void DealInitialCards()
    {
        for (int i = 0; i < 7; i++)
        {
            Card cardToDeal = mainDeck.DrawCard();
            Debug.Log(cardToDeal);
            if (cardToDeal != null)
                AddCardToHand(cardToDeal);
        }
    }

    public void RemoveCardFromHand(GameObject card)
    {
        CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
        if (currentHand.Contains(cardDisplay.cardData))
        {
            currentHand.Remove(cardDisplay.cardData); // Remove the card data from the current hand list
        }

        Destroy(card);
    }

    public void ShowAllCards()
    {
        foreach (Transform card in handTransform)
        {
            card.GetComponent<CardDisplay>().UpdateCardUI(card.GetComponent<CardDisplay>().cardData);  // Adjusted this line
        }
    }
}
