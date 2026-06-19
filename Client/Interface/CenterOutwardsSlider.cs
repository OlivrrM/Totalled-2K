using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterOutwardsSlider : MonoBehaviour
{
    public Slider leftSlider;
    public Image leftSliderGraphic;
    public Slider rightSlider;
    public Image rightSliderGraphic;
    public void SetValue(float value)
    {
        leftSlider.value = value;
        rightSlider.value = value;
    }
    public void SetMaxValue(float value)
    {
        leftSlider.maxValue = value;
        rightSlider.maxValue = value;
    }
    public void SetColor(Color color)
    {
        leftSliderGraphic.color = color;
        rightSliderGraphic.color = color;
    }
}
