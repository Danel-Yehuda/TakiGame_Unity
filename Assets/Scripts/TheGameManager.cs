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
    public TMP_Text turnIndicator;  // Drag and drop your UI Text component here in the Inspector

    private void Start()
    {
        currentTurn = TurnState.PLAYER_TURN;
        UpdateTurnIndicator();
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
            StartPlayerTurn();
        }

        UpdateTurnIndicator();  // Refresh the turn indicator when turns change
    }

    private void StartPlayerTurn()
    {
        // Logic for setting up the player's turn, if any
        // Example: perhaps reset some timer, enable some player controls, etc.
    }

    private void StartComputerTurn()
    {
        // Logic for setting up the computer's turn
        // Example: Start some AI routines to decide what card to play
    }

    private void UpdateTurnIndicator()
    {
        turnIndicator.text = (currentTurn == TurnState.PLAYER_TURN) ? "Your Turn" : "Computer's Turn";
    }
}
