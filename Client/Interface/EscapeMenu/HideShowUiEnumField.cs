using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideShowUiEnumField : HideShowUI //I KNOW! Im stupid. I didn't use Canvas Groups.
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI text;
    public Image arrow;
    public Image dropdownImage;
    public TMP_Dropdown dropdown;
    float targetAlpha;
    public float speed = 8f;
    public override void Hide()
    {
        targetAlpha = 0f;
        dropdown.interactable = false;
        label.raycastTarget = false;
    }
    public override void Show()
    {
        targetAlpha = 1f;
        dropdown.interactable = true;
        label.raycastTarget = true;
    }
    private void Update()
    {
        label.color = Color.Lerp(label.color, Utilities.SetColorAlpha(label.color, targetAlpha), Time.unscaledDeltaTime * speed);
        text.color = Color.Lerp(text.color, Utilities.SetColorAlpha(text.color, targetAlpha), Time.unscaledDeltaTime * speed);
        arrow.color = Color.Lerp(arrow.color, Utilities.SetColorAlpha(arrow.color, targetAlpha), Time.unscaledDeltaTime * speed);
        dropdownImage.color = Color.Lerp(dropdownImage.color, Utilities.SetColorAlpha(dropdownImage.color, targetAlpha), Time.unscaledDeltaTime * speed);
    }
}
