using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public Sprite cardBackSprite; 
    private Image cardImage;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void UpdateCardUI()
    {
        UpdateCardUI(cardData);
    }

    public void UpdateCardUI(Card actualCardData)
    {
        cardImage.sprite = actualCardData.cardSprite;  
    }

    public void HideCard()
    {
        cardImage.sprite = cardBackSprite;
    }
}
