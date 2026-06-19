using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFxVolumeManager : MonoBehaviour
{
    bool enabled;
    float currentSmoothness;

    float posOffset;

    public bool enableOnStart;

    float currentAmount = 1f;

    [HideInInspector] public float currentVisibility;

    public bool unscaledDeltaTime;
    private void Start()
    {
        if (enableOnStart) { Enable(999f); }
        else { Disable(999f); }
    }
    public void Enable(float smoothness = 1f, float amount = 1f)
    {
        enabled = true;
        currentSmoothness = smoothness;
        amount = Mathf.Clamp(amount, 0f, 1f);
        currentAmount = amount;
    }
    public void Disable(float smoothness = 1f)
    {
        enabled = false;
        currentSmoothness = smoothness;
    }
    private void Update()
    {
        if (enabled) { posOffset = Mathf.Lerp(posOffset, 0f+(2.55f * (1-currentAmount)), (unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime) * currentSmoothness); }
        else { posOffset = Mathf.Lerp(posOffset, 2.55f, (unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime) * currentSmoothness); }
        if (posOffset > 2.525f)
        {
            transform.position = Cache.vcam.transform.position - (-Cache.vcam.transform.forward * 99f);
        }
        else
        {
            transform.position = Cache.vcam.transform.position - (-Cache.vcam.transform.forward * posOffset);
        }
        currentVisibility = Mathf.InverseLerp(0f, 2.55f, posOffset);
        currentVisibility = 1 - currentVisibility;
    }
}
