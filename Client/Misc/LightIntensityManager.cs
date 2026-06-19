using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityManager : MonoBehaviour
{
    public Light light;
    float defaultRange;
    float defaultIntensity;

    float targetAmount;
    float currentSmoothness;
    private void Start()
    {
        targetAmount = 1f;
        currentSmoothness = 1f;
    }
    public void SetIntensity(float amount,float smoothness)
    {
        targetAmount = amount;
        currentSmoothness = smoothness;
    }
    private void Update()
    {
        light.range = Mathf.Lerp(light.range, defaultRange * targetAmount, Time.deltaTime * currentSmoothness);
        light.intensity = Mathf.Lerp(light.intensity, defaultIntensity * targetAmount, Time.deltaTime * currentSmoothness);
    }
}
