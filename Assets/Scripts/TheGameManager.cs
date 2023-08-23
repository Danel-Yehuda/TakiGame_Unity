using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TheGameManager : MonoBehaviour
{
    public enum TurnState
    {
        PLAYER_TURN,
        COMPUTER_TURN
    }

    public TurnState currentTurn;
    public TMP_Text turnIndicator;
    public HandManager computerHandManager; // Drag the computer's HandManager here in the Inspector
    public HandManager playerHandManager; //
    public DiscardPile discardPile; // Drag the DiscardPile here in the Inspector
    public Deck mainDeck; // Drag the main Deck here in the Inspector
    private int turnsToSkip = 0;
    private void Start()
    {
        currentTurn = (Random.value > 0.5f) ? TurnState.PLAYER_TURN : TurnState.COMPUTER_TURN;
        UpdateTurnIndicator();
        if (currentTurn == TurnState.COMPUTER_TURN)
        {
            StartComputerTurn();
        }
        else
        {
            PromptPlayerTurn();
        }
    }

    public void EndTurn()
    {
        if (turnsToSkip > 0)
        {
            turnsToSkip--;
        }
        else
        {
            if (currentTurn == TurnState.PLAYER_TURN)
            {
                currentTurn = TurnState.COMPUTER_TURN;
                StartComputerTurn();
            }
            else
            {
                currentTurn = TurnState.PLAYER_TURN;
                PromptPlayerTurn();
            }
        }
        UpdateTurnIndicator();
    }



    private void PromptPlayerTurn()
    {
        Debug.Log("Player's turn! Play a card or skip.");
    }

    private void StartPlayerTurn()
    {
        // Logic for setting up the player's turn
    }

    private void StartComputerTurn()
    {
        Invoke("ComputerPlayCard", 1f);
    }


    private void ComputerPlayCard()
    {
        Card topDiscard = discardPile.GetTopCard(); // Assuming you have a method in DiscardPile to get the top card

        foreach (Transform cardTransform in computerHandManager.handTransform)
        {
            Card card = cardTransform.GetComponent<CardDisplay>().cardData;
            if (card.CanBePlayedOn(topDiscard))
            {
                PlayCard(cardTransform.gameObject);
                
                // Handle the special ability for the computer's card
                HandleSpecialAbility(card, TheGameManager.TurnState.COMPUTER_TURN);
                
                // If the card doesn't have a special ability that affects turn order, end the computer's turn
                if (card.cardType != CardType.Stop && card.cardType != CardType.Plus && card.cardType != CardType.ChangeDirection)
                {
                    EndTurn();
                }
                
                return;
            }
        }

        // If no playable card in hand, draw a card and end the turn
        Card drawnCard = mainDeck.DrawCard();
        computerHandManager.AddCardToHand(drawnCard); // Add the drawn card to the computer's hand
        EndTurn();
    }





    private void PlayCard(GameObject cardGameObject)
    {
        Card cardData = cardGameObject.GetComponent<CardDisplay>().cardData;

        // Remove from computer's hand
        computerHandManager.RemoveCardFromHand(cardGameObject);

        // Add to discard pile and show the card
        discardPile.AddCardFromComputer(cardData);
    }


    private void UpdateTurnIndicator()
    {
        turnIndicator.text = (currentTurn == TurnState.PLAYER_TURN) ? "Your Turn" : "Computer's Turn";
    }

    public void HandleSpecialAbility(Card card, TurnState turnState)
    {
        switch (card.cardType)
        {
            case CardType.Stop:
                turnsToSkip = 1;
                break;
            case CardType.Plus:
                turnsToSkip = 1;
                break;
            case CardType.ChangeDirection:
                turnsToSkip = 1;
                break;
            case CardType.Taki:
                if (turnState == TurnState.COMPUTER_TURN)
                {
                    // Allow the computer to play all cards of the same color
                    PlayAllSameColorCards(card.cardColor);
                }
                else
                {
                    // Count the player's cards of the same color as the Taki card
                    int sameColorCount = CountPlayerCardsOfColor(card.cardColor);
                    Debug.Log(sameColorCount);
                    turnsToSkip = sameColorCount - 1; // -1 because the current turn is already the player's
                }
                break;
        }
    }

    private int CountPlayerCardsOfColor(CardColor color)
    {
        int count = 0;
        foreach (Transform cardTransform in playerHandManager.handTransform)
        {
            Card card = cardTransform.GetComponent<CardDisplay>().cardData;
            if (card.cardColor == color)
            {
                count++;
            }
        }
        return count;
    }



    private void PlayAllSameColorCards(CardColor color)
    {
        // Get all cards of the same color from the player's hand
        List<GameObject> sameColorCards = new List<GameObject>();
        foreach (Transform cardTransform in computerHandManager.handTransform)
        {
            Card card = cardTransform.GetComponent<CardDisplay>().cardData;
            if (card.cardColor == color)
            {
                sameColorCards.Add(cardTransform.gameObject);
            }
        }

        // Play all the same color cards
        foreach (GameObject cardGameObject in sameColorCards)
        {
            PlayCard(cardGameObject);
        }
    }

}
