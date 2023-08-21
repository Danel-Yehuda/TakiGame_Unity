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

    private void Awake()
    {
        if (!gameManager)
        {
            gameManager = FindObjectOfType<TheGameManager>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        handLayoutGroup = playerHandManager.GetHandLayoutGroup();
        handLayoutGroup.enabled = false;

        Debug.Log($"Dragging {this.GetComponent<CardDisplay>().cardData.cardColor} card");
        originalPosition = transform.position;
        originalSize = (transform as RectTransform).sizeDelta;
        (transform as RectTransform).sizeDelta = new Vector2(dragWidth, dragHeight);
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
        handLayoutGroup.enabled = true;

        if (isDroppedOnDiscardPile)
        {
            CardDisplay thisCardDisplay = GetComponent<CardDisplay>();
            
            Transform lastChild = discardPileTransform.childCount > 0 ? discardPileTransform.GetChild(discardPileTransform.childCount - 1) : null;
            CardDisplay topCardDisplay = lastChild ? lastChild.GetComponent<CardDisplay>() : null;

            if (topCardDisplay == null || thisCardDisplay.cardData.CanBePlayedOn(topCardDisplay.cardData))
            {
                DiscardThisCard();
                gameManager.PlayCard(thisCardDisplay.cardData); // Inform the game manager that a card has been played.
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
