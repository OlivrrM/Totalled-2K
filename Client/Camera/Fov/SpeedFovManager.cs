using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedFovManager : MonoBehaviour
{
    public float fovMultiplier;
    public float fovSpeed;
    float fovTarget;
    float fov;

    public float maxFovMultiplier;
    private void Start()
    {
        Cache.fovManager.AddValue("SpeedFov", new FovMultiplier { value = 1f });
    }
    private void Update()
    {
        fovTarget = Mathf.Clamp(1f + ((Mathf.Clamp(FragMovementListener.hSpeedPercentage,0.8f,Mathf.Infinity)) * fovMultiplier),0f, maxFovMultiplier);
        fov = Mathf.Lerp(fov, fovTarget, Time.deltaTime * fovSpeed);
        Cache.fovManager.UpdateValue("SpeedFov", new FovMultiplier { value = fov });
    }
}
