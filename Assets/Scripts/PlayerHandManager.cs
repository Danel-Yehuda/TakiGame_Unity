using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private float regularSpacing = 10f;  // Spacing for 6 or fewer cards
    [SerializeField] private float regularSize = 1f;      // Scale for 6 or fewer cards

    private void Awake()
    {
        AdjustHandLayout();
    }
    private void Start()
    {
        AdjustHandLayout();
    }

    public void AdjustHandLayout()
    {
        int cardCount = transform.childCount;
        Debug.Log(cardCount);

        if (cardCount >= 7)
        {
            layoutGroup.childControlWidth = true;
        }
        else
        {
            // Set regular spacing and card size
            layoutGroup.childControlWidth = false;
            layoutGroup.spacing = regularSpacing;
            SetAllChildSize(regularSize);
        }
    }

    private void SetAllChildSize(float size)
    {
        Vector3 targetSize = new Vector3(size, size, size);
        foreach (Transform child in transform)
        {
            child.localScale = targetSize;
        }
    }
}
