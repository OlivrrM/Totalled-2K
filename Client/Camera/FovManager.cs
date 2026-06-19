using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FovManager : ValueMultiplierManager
{
    public CinemachineVirtualCamera vcam;
    [HideInInspector] public float defaultFov;

    [HideInInspector] public bool forcedOffsetsOnly;
    private void Awake()
    {
        Cache.vcam = vcam;
    }
    private void Start()
    {
        defaultFov = Cache.vcam.m_Lens.FieldOfView;
        Cache.fovManager = this;
    }
    private void Update()
    {
        Cache.vcam.m_Lens.FieldOfView = defaultFov;
        foreach (KeyValuePair<string, object> value in values){
            FovMultiplier fovMultiplier = (FovMultiplier)value.Value;
            if (forcedOffsetsOnly) { if (!fovMultiplier.forcedValue) { continue; } }
            Cache.vcam.m_Lens.FieldOfView *= fovMultiplier.value;
        }
    }
}
