using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTransparency : MonoBehaviour
{
    public Image image;
    public float smoothness;
    float targetTransparency;
    float defaultTransparency;
    private void Start()
    {
        defaultTransparency = image.color.a;
        targetTransparency = defaultTransparency;
    }
    public void SetTransparency(float transparency, bool instant = false)
    {
        targetTransparency = transparency;
        if (instant) { image.color = image.color = new Color(image.color.r, image.color.g, image.color.b, targetTransparency); }
    }
    public void SetToDefaultTransparency(bool instant = false)
    {
        targetTransparency = defaultTransparency;
        if (instant) { image.color = image.color = new Color(image.color.r, image.color.g, image.color.b, defaultTransparency); }
    }
    private void Update()
    {
        image.color = Color.Lerp(image.color, new Color(image.color.r, image.color.g, image.color.b, targetTransparency), Time.deltaTime * smoothness);
    }
}
