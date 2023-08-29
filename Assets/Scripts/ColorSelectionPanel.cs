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

    private enum PanelMode
    {
        SuperChangeColor,
        SuperTaki
    }

    private PanelMode currentMode;

    private void Start()
    {
        redButton.onClick.AddListener(() => HandleColorSelection(CardColor.Red));
        greenButton.onClick.AddListener(() => HandleColorSelection(CardColor.Green));
        blueButton.onClick.AddListener(() => HandleColorSelection(CardColor.Blue));
        yellowButton.onClick.AddListener(() => HandleColorSelection(CardColor.Yellow));

        gameObject.SetActive(false); // Hide the panel by default
    }

    public void ShowPanelForSuperChangeColor()
    {
        currentMode = PanelMode.SuperChangeColor;
        ShowPanel();
    }

    public void ShowPanelForSuperTaki()
    {
        currentMode = PanelMode.SuperTaki;
        ShowPanel();
    }

    public void ShowPanel()
    {
        Debug.Log(currentMode == PanelMode.SuperChangeColor ? "SuperChangeColor card played!" : "SuperTaki card played!");
        gameObject.SetActive(true);
    }

    private void HandleColorSelection(CardColor color)
    {
        switch (currentMode)
        {
            case PanelMode.SuperChangeColor:
                gameManager.ChangeDiscardPileColor(color);
                gameManager.EndTurn();
                break;
            case PanelMode.SuperTaki:
                gameManager.ChangeDiscardPileToTaki(color);
                break;
        }
        gameObject.SetActive(false); // Hide the panel after selection
    }
}
