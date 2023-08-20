using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    public TMP_Text handleText; 
    public Slider slider;

    private void Update()
    {
        if(handleText == null) 
        {
            Debug.LogError("handleText is not assigned!");
            return;
        }
        
        if(slider == null) 
        {
            Debug.LogError("slider is not assigned!");
            return;
        }
        
        handleText.text = Mathf.RoundToInt(slider.value).ToString();
    }

}