using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HideShowInputField : HideShowUI
{
    public TextMeshProUGUI placeholder;
    public TextMeshProUGUI text;
    public TextMeshProUGUI titleText;
    public TMP_InputField inputField;
    float targetAlpha;
    public float speed = 8f;
    public override void Hide()
    {
        targetAlpha = 0f;
        inputField.interactable = false;
        text.raycastTarget = false;
        titleText.raycastTarget = false;
        placeholder.raycastTarget = false;
    }
    public override void Show()
    {
        targetAlpha = 1f;
        inputField.interactable = true;
        text.raycastTarget = true;
        titleText.raycastTarget = true;
        placeholder.raycastTarget = true;
    }
    private void Update()
    {
        text.color = Color.Lerp(text.color, Utilities.SetColorAlpha(text.color, targetAlpha), Time.unscaledDeltaTime * speed);
        titleText.color = Color.Lerp(titleText.color, Utilities.SetColorAlpha(titleText.color, targetAlpha), Time.unscaledDeltaTime * speed);
        placeholder.color = Color.Lerp(placeholder.color, Utilities.SetColorAlpha(placeholder.color, targetAlpha), Time.unscaledDeltaTime * speed);
        inputField.caretColor = Color.Lerp(inputField.caretColor, Utilities.SetColorAlpha(inputField.caretColor, targetAlpha), Time.unscaledDeltaTime * speed);
    }
}
