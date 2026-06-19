using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScaleBob : MonoBehaviour
{
    RectTransform rect;

    float currentAmount;
    float currentLifetime;

    Vector2 defaultScale;
    private void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        defaultScale = rect.localScale;
        currentAmount = 1f;
    }
    public void Bob(float amount, float lifetime)
    {
        currentAmount = amount;
        currentLifetime = lifetime;
    }
    private void Update()
    {
        if (Time.deltaTime!=0f)
        {
            rect.localScale = defaultScale * currentAmount;
            currentAmount = Mathf.Lerp(currentAmount, 1f, Time.deltaTime / currentLifetime);
        }
    }
}
