using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class FirearmSpreadCrosshairIndicator : MonoBehaviour
{
    public RectTransform[] indicators;
    public Image[] indicatorImages;
    public Image[] enemyIndicatorImages;
    public Color enemyIndicatorColor;
    public Vector2[] indicatorDirections;

    public float indicatorDistanceMultiplier;
    public float indicatorSmoothness;
    Vector2[] targetIndicatorPosition;

    public float indicatorSizeOverDistance;
    public float indicatorSizeIncrease;
    public float maxSize;
    public float minSize;

    public float recoilAmountMultiplier;

    [HideInInspector] public float currentFirearmDipAmount; // The spread required for individual firearm until indicator will apply currentFirearmDipMultiplier
    [HideInInspector] public float currentFirearmDipMultiplier;

    public float maxSpreadBeforeLog = 1.0f; // Adjust this threshold as needed
    public float logBase = 2f; // Base of the logarithm to control falloff
    public float logScale = 5f;  // Scaling factor to control how strong the log effect is
    private void Start()
    {
        Cache.firearmSpreadCrosshairIndicator = this;
        targetIndicatorPosition = new Vector2[indicators.Length];
        //indicatorImages = new Image[indicators.Length];
        /*for (int i = 0; i < indicators.Length; i++){
            indicatorImages[i] = indicators[i].GetComponent<Image>();
        }*/
    }
    //    1 - 5
    //   4  -  6
    private void Update()
    {
        for (int i = 0; i < indicators.Length; i++) {

            logBase = Mathf.Clamp(logBase, 1.01f, 100f);  // Prevents NaN by keeping it in a safe range

            // Get the current spread value
            float spread = ((Firearm.globalCurrentInaccuracy*0.5f) + (Firearm.globalCurrentCamRecoilAmount.magnitude*recoilAmountMultiplier)) * (Cache.crouch.crouching?0.66f:1f);

            // Ensure spread is valid (prevents NaN issues)
            spread = Mathf.Max(spread, 0.0001f); // Prevent zero or negative values

            // Apply logarithmic scaling if spread exceeds the threshold
            if (spread > maxSpreadBeforeLog)
            {
                float excessSpread = spread - maxSpreadBeforeLog;
                spread = maxSpreadBeforeLog + (Mathf.Log(excessSpread + 1, logBase) * logScale);
            }

            // Calculate indicator position
            Vector2 amount = Cache.inventory.equippedItem is Firearm ? (indicatorDirections[i] * Mathf.Clamp(spread * indicatorDistanceMultiplier,minSize * (Cache.crouch.crouching?0.66f:1f),Mathf.Infinity)):Vector2.zero;
            //Vector2 amount = indicatorDirections[i] * Firearm.globalCurrentSpread * indicatorDistanceMultiplier;
            targetIndicatorPosition[i] = amount * Firearm.globalCurrentSpreadCrosshairSizeMultiplier;
            indicators[i].anchoredPosition = Vector2.Lerp(indicators[i].anchoredPosition,targetIndicatorPosition[i], indicatorSmoothness * Time.deltaTime);
            indicators[i].localScale = Utilities.V3All(Mathf.Clamp((Mathf.InverseLerp(0f, indicatorSizeOverDistance, indicators[i].anchoredPosition.magnitude) * indicatorSizeIncrease) + 1f,0f,maxSize));
            indicatorImages[i].color = Color.Lerp(indicatorImages[i].color, new Color(Color.white.r, Color.white.g, Color.white.b, Inspect.lookingAtEnemy ? 1f : 0.75f), Time.deltaTime * 10f);
            enemyIndicatorImages[i].color = Color.Lerp(enemyIndicatorImages[i].color, Inspect.lookingAtEnemy ? enemyIndicatorColor : Utilities.Invisible(enemyIndicatorColor), Time.deltaTime * 10f);
        }
    }
}
