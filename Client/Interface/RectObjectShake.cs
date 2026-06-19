using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectObjectShake : MonoBehaviour
{
    RectTransform rect;

    float currentLifetime;
    Vector2 currentAmount;

    Vector2 defaultPos;
    private void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        defaultPos = rect.anchoredPosition;
    }
    public void Shake(Vector2 amount, float lifetime)
    {
        currentAmount = amount;
        currentLifetime = lifetime;
    }
    private void Update()
    {
        if (Time.deltaTime!=0f)
        {
            rect.anchoredPosition = defaultPos + new Vector2(currentAmount.x * (Random.RandomRange(-1f, 1f)), currentAmount.y * (Random.RandomRange(-1f, 1f)));
            currentAmount = Vector2.Lerp(currentAmount, Vector2.zero, Time.deltaTime / currentLifetime);
        }
    }
}
