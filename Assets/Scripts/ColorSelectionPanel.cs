using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectionPanel : MonoBehaviour
{
    public TheGameManager gameManager;
    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public Button yellowButton;

    private void Start()
    {
        redButton.onClick.AddListener(() => SelectColor(CardColor.Red));
        greenButton.onClick.AddListener(() => SelectColor(CardColor.Green));
        blueButton.onClick.AddListener(() => SelectColor(CardColor.Blue));
        yellowButton.onClick.AddListener(() => SelectColor(CardColor.Yellow));

        gameObject.SetActive(false); // Hide the panel by default
    }

    public void ShowPanel()
    {
        Debug.Log("SuperChangeColor card played!");
        gameObject.SetActive(true);
    }

    private void SelectColor(CardColor color)
    {
        gameManager.ChangeDiscardPileColor(color);
        gameObject.SetActive(false); // Hide the panel after selection
        gameManager.EndTurn();
    }

}

