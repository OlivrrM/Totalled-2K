using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFovManager : ValueMultiplierManager
{
    public Camera cam;
    [HideInInspector] public float defaultFov;

    [HideInInspector] public bool forcedOffsetsOnly;
    private void Start()
    {
        defaultFov = Cache.vcam.m_Lens.FieldOfView;
        Cache.handheldRenderFovManager = this;
    }
    private void Update()
    {
        cam.fieldOfView = defaultFov;
        foreach (KeyValuePair<string, object> value in values){
            FovMultiplier fovMultiplier = (FovMultiplier)value.Value;
            if (forcedOffsetsOnly) { if (!fovMultiplier.forcedValue) { continue; } }
            cam.fieldOfView *= fovMultiplier.value;
        }
    }
}
