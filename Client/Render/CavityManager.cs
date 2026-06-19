using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using ScreenSpaceCavityCurvature.Universal;
using UnityEngine.SceneManagement;

public class CavityManager : RenderFeatureManager
{
    Volume cavityVolume;
    Volume handheldCavityVolume;
    ScreenSpaceCavityCurvature.Universal.SSCC sSCC;
    ScreenSpaceCavityCurvature.Universal.SSCC sSCCHandheld;

    private void Awake()
    {
        Cache.cavityManager = this;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;   
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cavityVolume = GameObject.Find("Cavity").GetComponent<Volume>();
        handheldCavityVolume = GameObject.Find("HandheldCavity").GetComponent<Volume>();
        if (handheldCavityVolume.profile.TryGet<SSCC>(out sSCCHandheld)){}
        if (cavityVolume.profile.TryGet<SSCC>(out sSCC)){
            enabled = sSCC.active;
        }
        else { enabled = false; }
    }
    public override void Enable()
    {
        sSCC.effectIntensity.value = 1f;
        sSCCHandheld.effectIntensity.value = 1f;
        base.Enable();
    }
    public override void Disable()
    {
        sSCC.effectIntensity.value = 0f;
        sSCCHandheld.effectIntensity.value = 0f;
        base.Disable();
    }
}
