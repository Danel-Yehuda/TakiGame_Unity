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
    public int consecutivePlusTwoCount = 0;

    
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
            if (currentTurn == TurnState.COMPUTER_TURN)
            {
                StartComputerTurn();
                return;  // Exit the method early to prevent switching the turn to the player
            }
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
        if (consecutivePlusTwoCount > 0)
        {
            if (!PlayerHasPlusTwoCard())
            {
                Debug.Log("You don't have a +2 card. You must draw cards.");
                for (int i = 0; i < 2 * consecutivePlusTwoCount; i++)
                {
                    Card drawnCard = mainDeck.DrawCard();
                    playerHandManager.AddCardToHand(drawnCard);
                }
                consecutivePlusTwoCount = 0; // Reset the count
                EndTurn();
                return;
            }
        }

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
        Card topDiscard = discardPile.GetTopCard();
        Card drawnCard;

        if (consecutivePlusTwoCount > 0)
        {
            // Check if the computer has a +2 card to play
            foreach (Transform cardTransform in computerHandManager.handTransform)
            {
                Card card = cardTransform.GetComponent<CardDisplay>().cardData;
                if (card.cardType == CardType.Number && card.cardNumber == 2)
                {
                    PlayCard(cardTransform.gameObject);
                    HandleSpecialAbility(card, TheGameManager.TurnState.COMPUTER_TURN);
                    EndTurn();
                    return;
                }
            }

            // If the computer doesn't have a +2 card, draw the required number of cards and end the turn
            for (int i = 0; i < 2 * consecutivePlusTwoCount; i++)
            {
                drawnCard = mainDeck.DrawCard();
                computerHandManager.AddCardToHand(drawnCard);
            }
            consecutivePlusTwoCount = 0; // Reset the count
            EndTurn();
            return;
        }

        foreach (Transform cardTransform in computerHandManager.handTransform)
        {
            Card card = cardTransform.GetComponent<CardDisplay>().cardData;
            if (card.CanBePlayedOn(topDiscard))
            {
                PlayCard(cardTransform.gameObject);
                HandleSpecialAbility(card, TheGameManager.TurnState.COMPUTER_TURN);
                if (turnsToSkip > 0)
                {
                    turnsToSkip--;  // Decrement turnsToSkip here
                    StartComputerTurn();
                    return;
                }
                else
                {
                    EndTurn();
                    return;
                }
            }
        }

        // If no playable card in hand, draw a card
        drawnCard = mainDeck.DrawCard();
        computerHandManager.AddCardToHand(drawnCard);
        if (turnsToSkip > 0)
        {
            turnsToSkip--;  // Decrement turnsToSkip here
            StartComputerTurn();
        }
        else
        {
            EndTurn();
        }
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
            case CardType.Number:
                if (card.cardNumber == 2)
                {
                    consecutivePlusTwoCount++;
                }
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
                    turnsToSkip = sameColorCount;
                }
                break;
        }
    }

    public bool PlayerHasPlusTwoCard()
    {
        foreach (Transform cardTransform in playerHandManager.handTransform)
        {
            Card card = cardTransform.GetComponent<CardDisplay>().cardData;
            if (card.cardType == CardType.Number && card.cardNumber == 2)
            {
                return true;
            }
        }
        return false;
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
