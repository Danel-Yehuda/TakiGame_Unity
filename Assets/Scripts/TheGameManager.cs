using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
public class TheGameManager : MonoBehaviour
{
    public enum TurnState
    {
        PLAYER_TURN,
        COMPUTER_TURN
    }

    public TurnState currentTurn;
    public TMP_Text turnIndicator;
    public HandManager computerHandManager; 
    public HandManager playerHandManager; 
    public DiscardPile discardPile; 
    public Deck mainDeck;
    private int turnsToSkip = 0;
    public int consecutivePlusTwoCount = 0;
    public ColorSelectionPanel colorSelectionPanel;
    public GameObject cardUIPrefab;
    public GameObject cardPrefab;
    public TMP_Text winnerText;
    public Button restartButton;
    public bool isGameOver = false;
    

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
        CheckForWinner();
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
        if (isGameOver) return;
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

        //Debug.Log("Player's turn! Play a card or skip.");
    }

    private void StartPlayerTurn()
    {
        // Logic for setting up the player's turn
    }

    private void StartComputerTurn()
    {
        if (isGameOver) return;
        computerHandManager.playerHandManager.AdjustHandLayout();
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
                    CheckForWinner();
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
            computerHandManager.playerHandManager.AdjustHandLayout();
            CheckForWinner();
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
                    computerHandManager.playerHandManager.AdjustHandLayout();
                    CheckForWinner();
                    EndTurn();
                    return;
                }
            }
        }

        // If no playable card in hand, draw a card
        drawnCard = mainDeck.DrawCard();
        computerHandManager.AddCardToHand(drawnCard);
        CheckForWinner();
        if (turnsToSkip > 0)
        {
            turnsToSkip--;
            StartComputerTurn();
        }
        else
        {
            CheckForWinner();
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
        CheckForWinner();
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
            case CardType.SuperChangeColor:
                if (turnState == TurnState.PLAYER_TURN)
                {
                    colorSelectionPanel.ShowPanelForSuperChangeColor();
                }
                else
                {
                    CardColor randomColor = (CardColor)Random.Range(0, 4); // Randomly select a color for the computer
                    ChangeDiscardPileColor(randomColor);
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
            case CardType.SuperTaki:
                if (turnState == TurnState.PLAYER_TURN)
                {
                    colorSelectionPanel.ShowPanelForSuperTaki();  // Show the color selection panel
                }
                else
                {
                    // For the computer, choose a random color
                    CardColor[] colors = (CardColor[])System.Enum.GetValues(typeof(CardColor));
                    CardColor randomColor = colors[Random.Range(0, colors.Length)];
                    ChangeDiscardPileToTaki(randomColor);
                }
                break;
        }
    }

    public void ChangeDiscardPileColor(CardColor newColor)
    {
        // Create a new ChangeColor card
        Card changeColorCard = Instantiate(cardPrefab).GetComponent<Card>();
        changeColorCard.cardType = CardType.ChangeColor;
        changeColorCard.cardColor = newColor;
        changeColorCard.AssignCardImage();

        // Add the new card to the discard pile
        discardPile.AddCardToPile(changeColorCard);
    }


    public void ChangeDiscardPileToTaki(CardColor chosenColor)
    {
        Card takiCard = Instantiate(cardPrefab).GetComponent<Card>();
        takiCard.cardType = CardType.Taki;
        takiCard.cardColor = chosenColor;
        takiCard.AssignCardImage();

        discardPile.AddCardToPile(takiCard);
        turnsToSkip--;
        HandleSpecialAbility(takiCard, currentTurn);
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
        computerHandManager.playerHandManager.AdjustHandLayout();
    }

    public void CheckForWinner()
    {
        if (playerHandManager.handTransform.childCount == 0)
        {
            // Player wins
            DisplayWinner("Player Wins!");

            // Optionally, you can add more logic here, like stopping the game or showing a restart button.
            StopGame();
            restartButton.gameObject.SetActive(true);
        }
        else if (computerHandManager.handTransform.childCount == 0)
        {
            // Computer wins
            DisplayWinner("Computer Wins!");

            // Optionally, you can add more logic here, like stopping the game or showing a restart button.
            StopGame();
            restartButton.gameObject.SetActive(true);
        }
    }

    private void DisplayWinner(string message)
    {
        winnerText.text = message;
        winnerText.gameObject.SetActive(true);
        // Optionally, you can add more logic here, like stopping the game or showing a restart button.
    }

    private void StopGame()
    {
        isGameOver = true;
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


}
