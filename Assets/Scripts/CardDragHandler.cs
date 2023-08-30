using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    private Vector2 originalSize;
    public Transform discardPileTransform;
    public float dragWidth = 100f;
    public float dragHeight = 150f;
    private Vector3 startPos;
    private Transform startParent;
    public bool isDroppedOnDiscardPile = false;
    public PlayerHandManager playerHandManager;
    private HorizontalLayoutGroup handLayoutGroup;
    public TheGameManager gameManager;
    public Deck mainDeck;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameManager.currentTurn != TheGameManager.TurnState.PLAYER_TURN)
        {
            return;
        }
        if (gameManager.isGameOver) return;
        handLayoutGroup = playerHandManager.GetHandLayoutGroup();
        handLayoutGroup.enabled = false;

        //Debug.Log($"Dragging {this.GetComponent<CardDisplay>().cardData.cardColor} card");
        originalPosition = transform.position;
        originalSize = (transform as RectTransform).sizeDelta;
        (transform as RectTransform).sizeDelta = new Vector2(dragWidth, dragHeight);
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gameManager.currentTurn != TheGameManager.TurnState.PLAYER_TURN)
        {
            return;
        }
        if (gameManager.isGameOver) return;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        handLayoutGroup.enabled = true;

        if (gameManager.consecutivePlusTwoCount > 0 && !gameManager.PlayerHasPlusTwoCard())
        {
            // If there are +2 cards played and the player doesn't have a +2 card, they can only draw cards.
            mainDeck.PlayerDrawCard();
            return;
        }

        if (isDroppedOnDiscardPile)
        {
            CardDisplay thisCardDisplay = GetComponent<CardDisplay>();
            Transform lastChild = discardPileTransform.childCount > 0 ? discardPileTransform.GetChild(discardPileTransform.childCount - 1) : null;
            CardDisplay topCardDisplay = lastChild ? lastChild.GetComponent<CardDisplay>() : null;

            if (topCardDisplay == null)
            {
                //Debug.LogError("Top card does not have a CardDisplay component!");
                ReturnToStartPosition();
                return;
            }

            if (thisCardDisplay.cardData.CanBePlayedOn(topCardDisplay.cardData))
            {
                DiscardThisCard();
                
                gameManager.HandleSpecialAbility(thisCardDisplay.cardData, TheGameManager.TurnState.PLAYER_TURN);

                if (thisCardDisplay.cardData.cardType != CardType.SuperChangeColor && thisCardDisplay.cardData.cardType != CardType.SuperTaki)
                {
                    gameManager.EndTurn();  // End the player's turn after playing a card
                }
            }
            else
            {
                ReturnToStartPosition();
            }
        }
        else
        {
            ReturnToStartPosition();
        }

        gameManager.CheckForWinner();
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    private void DiscardThisCard()
    {
        transform.SetParent(discardPileTransform);
        transform.position = discardPileTransform.position;
        transform.SetAsLastSibling();
        playerHandManager.AdjustHandLayout();
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPos;
        transform.SetParent(startParent);
        (transform as RectTransform).sizeDelta = originalSize;
        playerHandManager.AdjustHandLayout();
    }
}
