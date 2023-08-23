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
    public TheGameManager gameManager;
    
    private void Start()
    {
        if (!playerHandManager)
        {
            playerHandManager = GetComponent<PlayerHandManager>();
        }
        DealInitialCards();
    }

    public GameObject AddCardToHand(Card cardData)
    {
        GameObject newCard = Instantiate(cardPrefab, handTransform);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardData = cardData;

        if (isPlayerHand)
        {
            cardDisplay.UpdateCardUI(cardDisplay.cardData);
            newCard.tag = "PlayerCard";
            
            CardDragHandler dragHandler = newCard.AddComponent<CardDragHandler>();
            dragHandler.discardPileTransform = discardPileTransform;
            dragHandler.playerHandManager = playerHandManager;
            
            dragHandler.gameManager = gameManager;  // Set the gameManager reference here
        }
        else
        {
            cardDisplay.HideCard();
        }

        if (playerHandManager)
        {
            playerHandManager.AdjustHandLayout();
        }

        return newCard;
    }




    public void DealInitialCards()
    {
        for (int i = 0; i < 7; i++)
        {
            Card cardToDeal = mainDeck.DrawCard();
            //Debug.Log(cardToDeal);
            if (cardToDeal != null)
                AddCardToHand(cardToDeal);
        }
    }

    public void RemoveCardFromHand(GameObject card)
    {
        Destroy(card);
        
        if (playerHandManager)
        {
            playerHandManager.AdjustHandLayout();
        }
    }

    public void ShowAllCards()
    {
        foreach (Transform card in handTransform)
        {
            card.GetComponent<CardDisplay>().UpdateCardUI(card.GetComponent<CardDisplay>().cardData);  // Adjusted this line
        }
    }
}
