using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    private Vector2 originalSize;  // To store the initial size of the card

    public Transform discardPileTransform;
    public float dragWidth = 100f;  // Width of card when dragging
    public float dragHeight = 150f;  // Height of card when dragging
    private Vector3 startPos;
    private Transform startParent;
    public bool isDroppedOnDiscardPile = false;
    public PlayerHandManager playerHandManager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Dragging {this.GetComponent<CardDisplay>().cardData.cardColor} card");
        originalPosition = transform.position;
        originalSize = (transform as RectTransform).sizeDelta;  // Capture the original size
        (transform as RectTransform).sizeDelta = new Vector2(dragWidth, dragHeight);  // Set the drag size
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDroppedOnDiscardPile)
        {
            CardDisplay thisCardDisplay = GetComponent<CardDisplay>();
            
            Transform lastChild = discardPileTransform.childCount > 0 ? discardPileTransform.GetChild(discardPileTransform.childCount - 1) : null;
            CardDisplay topCardDisplay = lastChild ? lastChild.GetComponent<CardDisplay>() : null;

            if (topCardDisplay == null)
            {
                Debug.LogError("Top card does not have a CardDisplay component!");
                ReturnToStartPosition();
                return;
            }

            if (thisCardDisplay.cardData.CanBePlayedOn(topCardDisplay.cardData))
            {
                DiscardThisCard();
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

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    private void DiscardThisCard()
    {
        transform.SetParent(discardPileTransform);
        transform.position = discardPileTransform.position;
        transform.SetAsLastSibling();

        // Update the player's hand layout
        playerHandManager.AdjustHandLayout();
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPos;
        transform.SetParent(startParent);
        (transform as RectTransform).sizeDelta = originalSize;  // Reset the size to the original value

        // Update the player's hand layout
        playerHandManager.AdjustHandLayout();
    }
}
