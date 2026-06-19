using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SkyManager : RenderFeatureManager
{
    Volume skyVolume;
    private void Awake()
    {
        Cache.skyManager = this;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        skyVolume = GameObject.Find("VolumetricClouds").GetComponent<Volume>();
        enabled = skyVolume.enabled;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public override void Enable()
    {
        skyVolume.enabled = true;
        base.Enable();
    }
    public override void Disable()
    {
        skyVolume.enabled = false;
        base.Disable();
    }
}
