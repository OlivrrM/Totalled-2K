using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FogManager;

public class FogManager : MonoBehaviour
{
    public Material fog;
    public static bool active = true;
    private void Awake()
    {
        Cache.fogManager = this;
        if (active) { Enable(); }
        else { Disable(); }
    }

    public struct FogSettings
    {
        public float maxDistance;
    }
    public FogSettings fogSettings = new FogSettings
    {
        maxDistance = 50f
    };

    public void Toggle()
    {
        active = !active;
        fogSettings.maxDistance = active ? 50f : 0f; // Pretty hard coded will improve when more fog related logic comes up
        RefreshFog();
    }
    public void Enable()
    {
        fogSettings.maxDistance = 50f;
        RefreshFog();
    }
    public void Disable()
    {
        fogSettings.maxDistance = 0f;
        RefreshFog();
    }
    public void RefreshFog()
    {
        //fog.SetFloat("_MaxDistance", fogSettings.maxDistance);
        Utilities.ApplyStructToMaterial(fogSettings, fog);
    }
}
