using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Config
{
    public Config()
    {
        quality = 5;
        shadowResolution = 3;
        shadowDistance = 30;
        postFxQuality = 3;
        renderScale = 1;

        antiAliasing = 0;
        capFps = 0;

        volume = 1f;

        handheldBob = true;
        handheldSway = true;
        cameraShake = true;
        dynamicFov = true;
        fieldOfView = 60;
    }

    //Graphics
    #region
    public int quality;
    public int shadowResolution;
    public float shadowDistance;
    public int postFxQuality;
    public float renderScale;
    #endregion

    //Video
    #region
    public System.Numerics.Vector2 screenResolution;
    public int windowedMode;
    public int capFps;
    public int antiAliasing;
    #endregion

    //Audio
    #region
    public float volume;
    public bool music;
    #endregion

    //Game
    #region
    public bool handheldBob; //HandheldRunBobManager.objectRunBob.cs
    public bool handheldSway; //Fuck with handheldrender.handheldSway.cs
    public bool cameraShake; //Vcam.cameraRotationOffsetManager.cs, directionalBob.cs
    public bool dynamicFov;
    public float fieldOfView;
    #endregion
}
