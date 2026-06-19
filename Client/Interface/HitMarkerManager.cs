using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarkerManager : MonoBehaviour
{
    public RectTransform markersRect;
    public Image[] markers;
    public Color defaultCol;
    public Color headshotCol;

    public float hitSize;
    float currentSize;
    public float sizeDecaySpeed;

    Color currentCol;
    private void Start()
    {
        Cache.hitMarkerManager = this;
    }
    public void Hit(float damage, bool headshot = false)
    {
        currentSize += hitSize * (Mathf.InverseLerp(0f, 50f, damage));
        if (headshot) { currentCol = headshotCol; }
    }
    private void Update()
    {
        currentSize = Mathf.Lerp(currentSize, 0f, Time.deltaTime * sizeDecaySpeed);
        markersRect.localScale = Utilities.V3All(currentSize);
        currentCol = Color.Lerp(currentCol, defaultCol, Time.deltaTime * 2);
        for (int i = 0; i < markers.Length; i++){
            markers[i].rectTransform.localScale = new Vector3(1 - (Mathf.InverseLerp(0.5f, 2.5f, currentSize)), 1f,1f);
            markers[i].color = currentCol;
        }
    }
}
