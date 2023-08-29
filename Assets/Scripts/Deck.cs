using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>(); // The main deck of cards
    public Card cardPrefab;  // Drag your Card prefab here in the Inspector.
    public DiscardPile discardPile;
    public HandManager playerHandManager;
    public TheGameManager gameManager;

    // Initialize the deck with all cards.
    private void Awake()
    {
        InitializeDeck();
        StartGame();
    }

    public void InitializeDeck()
    {
        foreach (CardColor color in System.Enum.GetValues(typeof(CardColor)))
        {
            // Create number cards
            for (int i = 1; i < 10; i++) // Assuming cards go from 1-10
            {
                Card newCard = Instantiate(cardPrefab);
                newCard.cardColor = color;
                newCard.cardType = CardType.Number;
                newCard.cardNumber = i;
                newCard.AssignCardImage();
                cards.Add(newCard);
            }

            // Create Taki cards
            Card takiCard = Instantiate(cardPrefab);
            takiCard.cardColor = color;
            takiCard.cardType = CardType.Taki;
            takiCard.AssignCardImage();
            cards.Add(takiCard);

            // Create Stop cards
            Card stopCard = Instantiate(cardPrefab);
            stopCard.cardColor = color;
            stopCard.cardType = CardType.Stop;
            stopCard.AssignCardImage();
            cards.Add(stopCard);

            // Create Plus cards
            Card plusCard = Instantiate(cardPrefab);
            plusCard.cardColor = color;
            plusCard.cardType = CardType.Plus;
            plusCard.AssignCardImage();
            cards.Add(plusCard);

            // Create ChangeDirection cards
            Card changeDirectionCard = Instantiate(cardPrefab);
            changeDirectionCard.cardColor = color;
            changeDirectionCard.cardType = CardType.ChangeDirection;
            changeDirectionCard.AssignCardImage();
            cards.Add(changeDirectionCard);

            // Add other card types similarly if required
        }

        // Create ChangeColor cards (These cards might not have a specific color, so modify as necessary)
        Card changeColorCard = Instantiate(cardPrefab);
        changeColorCard.cardType = CardType.ChangeColor;
        changeColorCard.AssignCardImage();
        cards.Add(changeColorCard);

        // Create SuperChangeColor card
        Card superChangeColorCard = Instantiate(cardPrefab);
        superChangeColorCard.cardType = CardType.SuperChangeColor;
        superChangeColorCard.AssignCardImage(); // Make sure you have an image for this card type
        cards.Add(superChangeColorCard);

        // Create SuperTaki card
        Card superTakiCard = Instantiate(cardPrefab);
        superTakiCard.cardType = CardType.SuperTaki;
        superTakiCard.AssignCardImage(); // Make sure you have an image for this card type
        cards.Add(superTakiCard);


        ShuffleDeck();
    }


    // Shuffle the deck of cards.
    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    // Draw a card from the top of the deck.
    public Card DrawCard()
    {
        if (cards.Count == 0) return null;

        Card drawnCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        return drawnCard;
    }

    public void StartGame()
    {
        // Draw a card from the deck and place it in the DiscardPile
        Card startingCard = DrawCard();
        if (startingCard != null)
        {
            discardPile.AddCardToPile(startingCard);
        }
    }

    public void PlayerDrawCard()
    {
        Card drawnCard = DrawCard();
        if(drawnCard != null)
        {
            playerHandManager.AddCardToHand(drawnCard);
            playerHandManager.playerHandManager.AdjustHandLayout();  // Adjust the player's hand layout after drawing a card.
        }
        gameManager.EndTurn();
    }


}
