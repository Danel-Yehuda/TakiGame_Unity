using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 originalPosition;
    public Transform discardPileTransform;
    private Vector3 startPos;
    private Transform startParent;
    public bool isDroppedOnDiscardPile = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"Dragging {this.GetComponent<CardDisplay>().cardData.cardColor} card");
        originalPosition = transform.position;
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
        transform.SetAsLastSibling();  // Makes sure the dragged card becomes the last child, hence the top card.
    }



    private void ReturnToStartPosition()
    {
        transform.position = startPos;
        transform.SetParent(startParent);
    }
}
