using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HideShowUiTextButton : HideShowUI
{
    public Button button;
    public TextMeshProUGUI text;
    float targetAlpha;
    public float speed = 8f;
    public override void Hide()
    {
        targetAlpha = 0f;
        button.interactable = false;
    }
    public override void Show()
    {
        targetAlpha = 1f;
        button.interactable = true;
    }
    private void Update()
    {
        text.color = Color.Lerp(text.color, Utilities.SetColorAlpha(text.color, targetAlpha), Time.unscaledDeltaTime * speed);
    }
}