using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandManager : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private float regularSpacing = 10f;
    [SerializeField] private float regularSize = 1f;


    private void Awake()
    {
        AdjustHandLayout();
    }

    private void Start()
    {
        AdjustHandLayout();
    }

    public HorizontalLayoutGroup GetHandLayoutGroup()
    {
        return layoutGroup;
    }

    public void AdjustHandLayout()
    {
        int cardCount = transform.childCount;
        //Debug.Log(cardCount);

        if (cardCount >= 7)
        {
            layoutGroup.childControlWidth = true;
        }
        else
        {
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
