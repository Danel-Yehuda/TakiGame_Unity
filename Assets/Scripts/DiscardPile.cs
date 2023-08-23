using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardPile : MonoBehaviour, IDropHandler
{
    public GameObject cardPrefab; 
    public Transform topCardTransform;
    public Transform initialTopCard;  // Reference to the initial top card

    public void OnDrop(PointerEventData eventData)
    {
        CardDragHandler cardDrag = eventData.pointerDrag.GetComponent<CardDragHandler>();
        if (cardDrag != null)
        {
            CardDisplay draggedCardDisplay = cardDrag.GetComponent<CardDisplay>();
            
            Transform lastChild = transform.childCount > 0 ? transform.GetChild(transform.childCount - 1) : null;
            CardDisplay topCardDisplay = lastChild ? lastChild.GetComponent<CardDisplay>() : null;

            if (draggedCardDisplay == null || topCardDisplay == null) 
            {
                Debug.LogError("Card component not found!");
                return;
            }

            if (draggedCardDisplay.cardData.CanBePlayedOn(topCardDisplay.cardData))
            {
                Debug.Log("Card can be played on top!");
                cardDrag.isDroppedOnDiscardPile = true;
                // Don't set the parent here. We'll handle it in the CardDragHandler.
            }
            else
            {
                Debug.Log("Card cannot be played on top!");
            }
        }
    }

    public void AddCardToPile(Card cardData)
    {
        Debug.Log("Adding card to pile");
        ClearPile();
        Debug.Log("Number of children after clear: " + topCardTransform.childCount);

        GameObject newCard = Instantiate(cardPrefab, topCardTransform.position, Quaternion.identity, topCardTransform);
        CardDisplay cardDisplay = newCard.GetComponent<CardDisplay>();
        cardDisplay.cardData = cardData;
        cardDisplay.UpdateCardUI();

        if (initialTopCard == null)
        {
            initialTopCard = newCard.transform;
        }
    }

    public void ClearPile()
    {
        foreach (Transform child in topCardTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public Card GetTopCard()
    {
        if (transform.childCount > 0)
        {
            return transform.GetChild(transform.childCount - 1).GetComponent<CardDisplay>().cardData;
        }
        return null;
    }

}
