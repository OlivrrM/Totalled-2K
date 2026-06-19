using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFovManager : MonoBehaviour
{
    public float fovMultiplier;
    public float fovSpeed;
    float fovTarget;
    float fov;

    public float maxFallSpeed;
    public float fallSpeedMultiplier;
    private void Start()
    {
        Cache.fovManager.AddValue("FallFov", new FovMultiplier { value = 1f });
    }
    private void Update()
    {
        fovTarget = 1f + (-(Mathf.Clamp(Cache.moveData.Velocity.y* fallSpeedMultiplier, maxFallSpeed, 0f)) * fovMultiplier);
        fov = Mathf.Lerp(fov, fovTarget, Time.deltaTime * fovSpeed);
        Cache.fovManager.UpdateValue("FallFov", new FovMultiplier { value = fov });
    }
}
