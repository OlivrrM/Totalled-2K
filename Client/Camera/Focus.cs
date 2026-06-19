using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus : MonoBehaviour
{
    public float focusFov;
    float currentFov;
    public float focusSmoothness;
    private void Start()
    {
        Cache.fovManager.AddValue("Focus", new FovMultiplier { value = 1f,forcedValue = true });
        Cache.handheldRenderFovManager.AddValue("Focus", new FovMultiplier { value = 1f, forcedValue = true });
        currentFov = 1f;
    }
    private void Update()
    {
        currentFov = Mathf.Lerp(currentFov, InputManager.GetFocusKey() ? focusFov : 1f, Time.deltaTime * focusSmoothness);
        Cache.fovManager.UpdateValue("Focus", new FovMultiplier { value = currentFov, forcedValue = true });
        Cache.handheldRenderFovManager.UpdateValue("Focus", new FovMultiplier { value = currentFov, forcedValue = true });
    }
}
