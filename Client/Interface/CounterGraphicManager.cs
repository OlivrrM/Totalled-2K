using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterGraphicManager : MonoBehaviour
{
    float timeAfterCounterChange;
    public TextMeshProUGUI counterText;
    Color defaultCounterTextCol;
    public Image counterIcon;
    Color defaultCounterIconCol;

    public Color denyColor;
    Color currentTextCol;
    Color currentIconCol;

    public RectObjectShake rectObjectShake;
    public Vector2 denyShakeAmount;
    public float denyShakeLifetime;

    public RectScaleBob rectScaleBob;
    public float bobAmount;
    public float bobLifetime;

    float previousValue;
    private void Start()
    {
        defaultCounterTextCol = counterText.color;
        defaultCounterIconCol = counterIcon.color;
    }

    public void SetCounter(float value)
    {
        if (value > previousValue) { rectScaleBob.Bob(bobAmount, bobLifetime); }
        counterText.text = value.ToString();
        timeAfterCounterChange = 0f;
        previousValue = value;
    }
    public void BobCounter(float amount, float lifetime)
    {
        rectScaleBob.Bob(amount, lifetime);
    }
    public void HideCounter()
    {
        counterText.text = "";
    }
    public void DenyCounter()
    {
        rectObjectShake.Shake(denyShakeAmount, denyShakeLifetime);
        currentTextCol = denyColor;
        currentIconCol = denyColor;
        timeAfterCounterChange = 1.5f;
        UpdateAlpha();
    }
    private void Update()
    {
        currentTextCol = Color.Lerp(currentTextCol, defaultCounterTextCol, Time.deltaTime / denyShakeLifetime);
        currentIconCol = Color.Lerp(currentIconCol, defaultCounterIconCol, Time.deltaTime / denyShakeLifetime);

        timeAfterCounterChange += Time.deltaTime;
        UpdateAlpha();
    }
    void UpdateAlpha()
    {
        if (timeAfterCounterChange > 3f)
        {
            counterIcon.color = Color.Lerp(counterIcon.color, new Color(currentIconCol.r, currentIconCol.g, currentIconCol.b, defaultCounterIconCol.a / 2f), Time.deltaTime * 2f);
            counterText.color = Color.Lerp(counterText.color, new Color(currentTextCol.r, currentTextCol.g, currentTextCol.b, defaultCounterTextCol.a * 0.75f), Time.deltaTime * 2f);
        }
        else
        {
            counterIcon.color = Color.Lerp(counterIcon.color, new Color(currentIconCol.r, currentIconCol.g, currentIconCol.b, defaultCounterIconCol.a), Time.deltaTime * 8f);
            counterText.color = Color.Lerp(counterText.color, new Color(currentTextCol.r, currentTextCol.g, currentTextCol.b, defaultCounterTextCol.a), Time.deltaTime * 8f);
        }
    }
}
