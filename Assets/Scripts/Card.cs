using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public enum CardType
{
    Number,
    Taki,
    Stop,
    Plus,
    ChangeDirection,
    ChangeColor,
    SuperChangeColor,
    SuperTaki
}

public class Card : MonoBehaviour
{
    public CardColor cardColor;
    public CardType cardType;
    public int cardNumber;
    public Sprite cardSprite;

    public void AssignCardImage()
    {
        string path = "Images/cards/";

        // Construct the path based on card color and card type.
        switch (cardType)
        {
            case CardType.Number:
                path += cardColor.ToString() + "_" + cardNumber.ToString();
                break;
            case CardType.Taki:
                path += cardColor.ToString() + "_Taki";
                break;
            case CardType.Stop:
                path += cardColor.ToString() + "_Stop";
                break;
            case CardType.Plus:
                path += cardColor.ToString() + "_Plus";
                break;
            case CardType.ChangeDirection:
                path += cardColor.ToString() + "_ChangeDirection";
                break;
            case CardType.ChangeColor:
                path += cardColor.ToString() + "_ChangeColor";
                break;
            case CardType.SuperChangeColor:
                path += "ChangeColor";
                break;
            case CardType.SuperTaki:
                path += "SuperTaki";
                break;
            default:
                //Debug.LogError("Unknown card type: " + cardType);
                return;
        }

        cardSprite = Resources.Load<Sprite>(path);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = cardSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on " + gameObject.name);
        }
    }

    public bool HasSpecialAbility()
    {
        return cardType == CardType.Stop || cardType == CardType.Plus || cardType == CardType.ChangeDirection || cardType == CardType.Taki;
    }


    public bool CanBePlayedOn(Card otherCard)
    {
        // Cards with the same color can always be played on each other.
        if (this.cardColor == otherCard.cardColor)
        {
            //Debug.Log($"Matching Color: {this.cardColor} == {otherCard.cardColor}");
            return true;
        }

        // Number cards can be played on other cards with the same number.
        if (this.cardType == CardType.Number && this.cardNumber == otherCard.cardNumber)
        {
            //Debug.Log($"Matching Number: {this.cardNumber} == {otherCard.cardNumber}");
            return true;
        }

        // Cards with the same type (except Number) can be played on each other.
        if (this.cardType == otherCard.cardType && this.cardType != CardType.Number)
        {
            //Debug.Log($"Matching Card Type: {this.cardType}");
            return true;
        }

        // ChangeColor cards can be played on any card.
        if (this.cardType == CardType.ChangeColor)
        {
            //Debug.Log("Playing a ChangeColor card.");
            return true;
        }

        if (this.cardType == CardType.SuperChangeColor)
        {
            return true;
        }

        if (this.cardType == CardType.SuperTaki)
        {
            return true;
        }

        //Debug.Log($"No match: Color - {this.cardColor} vs {otherCard.cardColor}, Number - {this.cardNumber} vs {otherCard.cardNumber}");
        return false;  // If none of the above conditions are met, the card cannot be played.
}


}//class
