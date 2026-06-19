using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideShowUiSlider : HideShowUI
{
    public TextMeshProUGUI text;
    public Image background;
    public Image fill;
    public Image handle;
    public Slider slider;
    float targetAlpha;
    public float speed = 8f;
    public override void Hide()
    {
        targetAlpha = 0f;
        slider.interactable = false;
        handle.raycastTarget = false;
    }
    public override void Show()
    {
        targetAlpha = 1f;
        slider.interactable = true;
        handle.raycastTarget = true;
    }
    private void Update()
    {
        background.color = Color.Lerp(background.color, Utilities.SetColorAlpha(background.color, targetAlpha), Time.unscaledDeltaTime * speed);
        text.color = Color.Lerp(text.color, Utilities.SetColorAlpha(text.color, targetAlpha), Time.unscaledDeltaTime * speed);
        fill.color = Color.Lerp(fill.color, Utilities.SetColorAlpha(fill.color, targetAlpha), Time.unscaledDeltaTime * speed);
        handle.color = Color.Lerp(handle.color, Utilities.SetColorAlpha(handle.color, targetAlpha), Time.unscaledDeltaTime * speed);
    }
}
