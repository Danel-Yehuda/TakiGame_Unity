using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TheGameManager : MonoBehaviour
{
    public enum TurnState
    {
        PLAYER_TURN,
        COMPUTER_TURN
    }

    public TurnState currentTurn;
    public TMP_Text turnIndicator;  // UI Text component
    public HandManager playerHandManager;
    public HandManager computerHandManager;

    private int extraTurns = 0;

    private void Start()
    {
        currentTurn = TurnState.PLAYER_TURN;
        UpdateTurnIndicator();
    }

    public void PlayCard(Card card) 
    {
        if(card.cardType == CardType.Taki)
        {
            HandleTakiCard(card.cardColor);
        }
        else if(card.cardType == CardType.Stop || card.cardType == CardType.ChangeDirection || card.cardType == CardType.Plus)
        {
            extraTurns++;
        }

        EndTurn();
    }

    private void HandleTakiCard(CardColor color)
    {
        HandManager activeHandManager = (currentTurn == TurnState.PLAYER_TURN) ? playerHandManager : computerHandManager;
        List<Card> cardsToPlay = new List<Card>();
            
        // Find all cards of the same color
        foreach(Transform child in activeHandManager.handTransform) 
        {
            Card cardInHand = child.GetComponent<Card>();
            if(cardInHand && cardInHand.cardColor == color)
            {
                cardsToPlay.Add(cardInHand);
            }
        }

        // Play the cards (or add logic to play them)
        foreach(Card cardToPlay in cardsToPlay)
        {
            // Add your logic to play the card here. 
            // For now, it just removes the card from the hand
            GameObject cardObj = cardToPlay.gameObject; 
            activeHandManager.RemoveCardFromHand(cardObj);
        }

        extraTurns += cardsToPlay.Count - 1;  // Taki itself is one turn, so subtract 1
    }


    public void EndTurn()
    {
        if (extraTurns > 0 && currentTurn == TurnState.PLAYER_TURN)
        {
            extraTurns--;
            StartPlayerTurn();
        }
        else if (currentTurn == TurnState.PLAYER_TURN)
        {
            currentTurn = TurnState.COMPUTER_TURN;
            StartComputerTurn();
        }
        else
        {
            currentTurn = TurnState.PLAYER_TURN;
            StartPlayerTurn();
        }

        UpdateTurnIndicator();
    }

    private void StartPlayerTurn()
    {
        // Logic for setting up the player's turn
    }

    private void StartComputerTurn()
    {
        // Logic for the computer's turn
        // Integrate your AI decision-making here
    }

    private void UpdateTurnIndicator()
    {
        turnIndicator.text = (currentTurn == TurnState.PLAYER_TURN) ? "Your Turn" : "Computer's Turn";
    }
}
