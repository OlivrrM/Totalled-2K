using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TmpTextTransparency : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float smoothness;
    float targetTransparency;
    float defaultTransparency;
    private void Start()
    {
        defaultTransparency = text.color.a;
        targetTransparency = defaultTransparency;
    }
    public void SetTransparency(float transparency, bool instant = false)
    {
        targetTransparency = transparency;
        if (instant) { text.color = text.color = new Color(text.color.r, text.color.g, text.color.b, targetTransparency); }
    }
    public void SetToDefaultTransparency(bool instant = false)
    {
        targetTransparency = defaultTransparency;
        if (instant) { text.color = text.color = new Color(text.color.r, text.color.g, text.color.b, defaultTransparency); }
    }
    private void Update()
    {
        text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, targetTransparency), Time.deltaTime * smoothness);
    }
}
