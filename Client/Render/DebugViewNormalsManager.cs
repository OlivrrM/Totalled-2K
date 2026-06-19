using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using ScreenSpaceCavityCurvature.Universal;
using UnityEngine.SceneManagement;

public class DebugViewNormalsManager : RenderFeatureManager
{
    Volume cavityVolume;
    ScreenSpaceCavityCurvature.Universal.SSCC sSCC;

    private void Awake()
    {
        Cache.debugViewNormalsManager = this;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cavityVolume = GameObject.Find("Cavity").GetComponent<Volume>();
        if (cavityVolume.profile.TryGet<SSCC>(out sSCC)){
            enabled = sSCC.debugMode.value == SSCC.DebugMode.ViewNormals;
        }
        else { enabled = false; }
    }
    public override void Enable()
    {
        sSCC.debugMode.value = SSCC.DebugMode.ViewNormals;
        base.Enable();
    }
    public override void Disable()
    {
        sSCC.debugMode.value = SSCC.DebugMode.Disabled;
        base.Disable();
    }
}
