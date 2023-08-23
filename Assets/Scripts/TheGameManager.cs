using UnityEngine;
using TMPro;

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
    public DiscardPile discardPile; // Drag the DiscardPile here in the Inspector
    public Deck mainDeck; // Drag the main Deck here in the Inspector

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
                EndTurn();  // End the turn after playing the card
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
}
