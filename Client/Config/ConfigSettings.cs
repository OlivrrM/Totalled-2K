using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;
using UnityEngine.Rendering.PostProcessing;

public class ConfigSettings : MonoBehaviour
{
    UniversalRenderPipelineAsset urpSettings;

    [Header("Cavity Profiles")]
    public VolumeProfile lowCavity;
    public VolumeProfile mediumCavity;
    public VolumeProfile highCavity;
    //public UniversalRendererData urpRenderData;

    [Header("Cached")]
    public Cinemachine.NoiseSettings vcamNoisePreset;
    private void Start()
    {
        Cache.configSettings = this;

        urpSettings = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

        fpsCap.maxValue = (float)Screen.currentResolution.refreshRateRatio.value;

        vcamNoisePreset = Cache.vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_NoiseProfile;

        LoadSettings();
    }
    public void LoadSettings()
    {
        /*
        MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        for (int i = 0; i < methodInfos.Length; i++)
        {
            string method = methodInfos[i].Name.Substring(3);
            method = char.ToUpper(method[0]) + method.Substring(1);
            Invoke(method,0f);
        }
        */
        if (ConfigSave.config != null)
        {
            FieldInfo[] fieldInfos = ConfigSave.config.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            ConfigSave.edittingConfig = Utilities.DeepCopy(ConfigSave.config);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                string method = fieldInfos[i].Name;
                method = char.ToUpper(method[0]) + method.Substring(1);
                method = "Set" + method;
                MethodInfo methodInfo = this.GetType().GetMethod(method);
                methodInfo.Invoke(this, new object[] { fieldInfos[i].GetValue(ConfigSave.config) }); ///Thread may stop here if there is an error within method.
            }
        }
        else
        {
            Debug.LogError("settings.config not found");
        }
    }
    [Header("UI")]
    public TMP_Dropdown quality;
    public void SetQuality(int set)
    {
        try
        {
            switch (set)
            {
                case 0:
                    QualitySettings.SetQualityLevel(0);
                    urpSettings = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                    urpSettings.shadowDistance = shadowDistance.value;
                    SetShadowResolution(shadowResolution.value);
                    SetPostFxQuality(postFxQuality.value);
                    SetRenderScale(renderScale.value);
                    SetAntiAliasing(antiAliasing.value);
                    //Utilities.SetTargetPipelineRendererType(urpSettings, 2);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(3);
                    urpSettings = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                    urpSettings.shadowDistance = shadowDistance.value;
                    SetShadowResolution(shadowResolution.value);
                    SetPostFxQuality(postFxQuality.value);
                    SetRenderScale(renderScale.value);
                    SetAntiAliasing(antiAliasing.value);
                    //Utilities.SetTargetPipelineRendererType(urpSettings, 1);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(5);
                    urpSettings = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
                    urpSettings.shadowDistance = shadowDistance.value;
                    SetShadowResolution(shadowResolution.value);
                    SetPostFxQuality(postFxQuality.value);
                    SetRenderScale(renderScale.value);
                    SetAntiAliasing(antiAliasing.value);
                    //Utilities.SetTargetPipelineRendererType(urpSettings, 0);
                    break;
            }
            quality.SetValueWithoutNotify(set);
            ConfigSave.edittingConfig.quality = set;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting quality level: {e.Message}");
        }
    }
    public TMP_Dropdown shadowResolution;
    public void SetShadowResolution(int set)
    {
        ShadowResoltionSetter shadowResoltionSetter = new ShadowResoltionSetter();
        switch (set)
        {
            case 0:
                shadowResoltionSetter.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._256;
                break;
            case 1:
                shadowResoltionSetter.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._512;
                break;
            case 2:
                shadowResoltionSetter.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._1024;
                break;
            case 3:
                shadowResoltionSetter.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._2048;
                break;
            case 4:
                shadowResoltionSetter.MainLightShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution._4096;
                break;
        }
        shadowResolution.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.shadowResolution = set;
    }

    public Slider shadowDistance;
    public void SetShadowDistance(float set)
    {
        urpSettings.shadowDistance = set;
        shadowDistance.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.shadowDistance = set;
    }
    public TMP_Dropdown postFxQuality;
    public void SetPostFxQuality(int set)
    {
        Volume cavityVolume = GameObject.Find("Cavity").GetComponent<Volume>();
        switch (set)
        {
            case 0:
                if (cavityVolume != null) { cavityVolume.profile = lowCavity; }
                urpSettings.colorGradingLutSize = 16;
                break;
            case 1:
                if (cavityVolume != null) { cavityVolume.profile = mediumCavity; }
                urpSettings.colorGradingLutSize = 24;
                break;
            case 2:
                if (cavityVolume != null) { cavityVolume.profile = highCavity; }
                urpSettings.colorGradingLutSize = 32;
                break;
        }
        postFxQuality.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.postFxQuality = set;
    }
    public Slider renderScale;
    public TextMeshProUGUI renderScaleText;
    bool renderScaleDown;
    public void SetRenderScale(float set)
    {
        renderScaleDown = true;
        set = (float)System.Math.Round(set, 1);
        urpSettings.renderScale = set;
        renderScale.value = set;
        ConfigSave.edittingConfig.renderScale = set;
    }
    public TMP_InputField screenResolutionX;
    public TMP_InputField screenResolutionY;
    bool ScreenResolutionInputChanged;
    Vector2 currentScreenResSet;
    public void ScreenResolutionInputChangeX(string set)
    {
        int setInt = 0;
        if (int.TryParse(set, out setInt))
        {
            ScreenResolutionInputChanged = true;
            currentScreenResSet = new Vector2(setInt, currentScreenResSet.y);
        }
        else
        {
            screenResolutionX.SetTextWithoutNotify("");
        }
    }
    public void ScreenResolutionInputChangeY(string set)
    {
        int setInt = 0;
        if (int.TryParse(set, out setInt))
        {
            ScreenResolutionInputChanged = true;
            currentScreenResSet = new Vector2(currentScreenResSet.x, setInt);
        }
        else
        {
            screenResolutionY.SetTextWithoutNotify("");
        }
    }
    public void SetScreenResolution(System.Numerics.Vector2 dummy)
    {
        if (currentScreenResSet.x < 100||currentScreenResSet.y < 100) { currentScreenResSet = new Vector2(Display.displays[Utilities.GetCurrentDisplayNumber()].renderingWidth, Display.displays[Utilities.GetCurrentDisplayNumber()].renderingHeight); }
        if ((ScreenResolutionInputChanged && currentScreenResSet.x > 0 && currentScreenResSet.y > 0) || dummy != System.Numerics.Vector2.Zero) //Jank invoke param boolean opperator substitute workaround
        {
            if (dummy!= System.Numerics.Vector2.Zero){currentScreenResSet = new Vector2(dummy.X, dummy.Y);}
            currentScreenResSet = new Vector2((int)currentScreenResSet.x, (int)currentScreenResSet.y);
            Screen.SetResolution((int)currentScreenResSet.x, (int)currentScreenResSet.y, ConfigSave.edittingConfig.windowedMode < 3);
            Cache.terminal.Print($"Set window resolution to {currentScreenResSet.x}x{currentScreenResSet.y}");
            screenResolutionX.SetTextWithoutNotify(currentScreenResSet.x.ToString());
            screenResolutionY.SetTextWithoutNotify(currentScreenResSet.y.ToString());
            ConfigSave.edittingConfig.screenResolution = Utilities.UnityVec2ToSysVec2(currentScreenResSet);
            ScreenResolutionInputChanged = false;
        }
    }
    public TMP_Dropdown windowedMode;
    public void SetWindowedMode(int set)
    {
        switch (set)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
        windowedMode.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.windowedMode = set;
    }
    public Slider fpsCap;
    public TextMeshProUGUI fpsCapText;
    bool fpsCapDown;
    public void SetCapFps(float set)
    {
        fpsCapDown = true;
        fpsCap.maxValue = (float)Screen.currentResolution.refreshRateRatio.value;
        Application.targetFrameRate = (int)set;
        ConfigSave.edittingConfig.capFps = (int)set;
        fpsCap.SetValueWithoutNotify(set);
    }

    public TMP_Dropdown antiAliasing;
    public void SetAntiAliasing(int set)
    {
        switch (set)
        {
            case 0:
                urpSettings.msaaSampleCount = 0;
                break;
            case 1:
                urpSettings.msaaSampleCount = 2;
                break;
            case 2:
                urpSettings.msaaSampleCount = 4;
                break;
            case 3:
                urpSettings.msaaSampleCount = 8;
                break;
        }
        antiAliasing.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.antiAliasing = set;
    }
    public Slider volumeSlider;
    public void SetVolume(float set)
    {
        if (Cache.audioVolumeManager.values.ContainsKey("ConfigVolume")){Cache.audioVolumeManager.UpdateValue("ConfigVolume", set);}
        else{Cache.audioVolumeManager.AddValue("ConfigVolume", set);}
        volumeSlider.SetValueWithoutNotify(set);
        ConfigSave.edittingConfig.volume = set;
    }
    public Toggle music;
    public void SetMusic(bool set)
    {
        //Music TOGGLE CODE
        music.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.music = set;
    }
    public Toggle handheldBob;
    public void SetHandheldBob(bool set)
    {
        ObjectRunBob objectRunBob = GameObject.Find("HandheldRunBobManager").GetComponent<ObjectRunBob>();
        objectRunBob.currentYBob = 0;
        objectRunBob.currentXBob = 0;
        objectRunBob.enabled = set;
        handheldBob.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.handheldBob = set;
    }
    public Toggle handheldSway;
    public void SetHandheldSway(bool set)
    {
        HandheldSway handheldSwayxxz = GameObject.Find("HandheldRender").GetComponent<HandheldSway>();
        handheldSwayxxz.transform.localRotation = Quaternion.identity;
        handheldSwayxxz.enabled = set;
        handheldSway.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.handheldSway = set;
    }
    public Toggle cameraShake;
    public void SetCameraShake(bool set)
    {
        CameraRotationOffsetManager cameraRotationOffsetManager = Cache.vcam.GetComponent<CameraRotationOffsetManager>();
        //cameraRotationOffsetManager.cinemachineCameraRotationFixer.m_Offset = Vector3.zero;
        //cameraRotationOffsetManager.surfCharacter.camZRotation = 0;
        //cameraRotationOffsetManager.enabled = set;
        cameraRotationOffsetManager.forcedOffsetsOnly = !set;
        DirectionalBob directionalBob = Cache.vcam.GetComponent<DirectionalBob>();
        directionalBob.currentDirection = Vector3.zero;
        directionalBob.cameraWorldSpaceRotationOffsetManager.UpdateOffset("DirectionalBob", directionalBob.currentDirection);
        directionalBob.enabled = set;
        Cache.vcam.GetComponent<Cinemachine.CinemachineImpulseListener>().enabled = set;
        Cache.vcam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = set ? vcamNoisePreset : new Cinemachine.NoiseSettings();
        cameraShake.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.cameraShake = set;
    }
    public Toggle dynamicFov;
    public void SetDynamicFov(bool set)
    {
        Cache.fovManager.forcedOffsetsOnly = !set;
        dynamicFov.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.dynamicFov = set;
    }
    //public Slider fieldOfView; // Add to settings interface later
    public void SetFieldOfView(float set)
    {
        Cache.fovManager.defaultFov = set;
        //fieldOfView.SetIsOnWithoutNotify(set);
        ConfigSave.edittingConfig.fieldOfView = set;
    }
    private void Update()
    {
        if (renderScaleDown)
        {
            if (Input.GetMouseButtonUp(0))
            {
                renderScaleDown = false;
            }
            renderScaleText.text = (renderScale.value * 100) + "%";
        }
        else
        {
            renderScaleText.text = "Render Scale";
        }
        if (fpsCapDown)
        {
            if (Input.GetMouseButtonUp(0))
            {
                fpsCapDown = false;
            }
            if (fpsCap.value == 0) { fpsCapText.text = "Unlimited"; }
            else { fpsCapText.text = fpsCap.value.ToString(); }
        }
        else
        {
            fpsCapText.text = "Fps Cap";
        }
    }
    public void SetSettingsOnExitPage()
    {
        SetScreenResolution(System.Numerics.Vector2.Zero);
    }
}
