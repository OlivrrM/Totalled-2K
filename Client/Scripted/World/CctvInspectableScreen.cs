using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

public class CctvInspectableScreen : MonoBehaviour
{
    public SpriteRenderer bloom;
    public CinemachineVirtualCamera vcam;
    Color defaultBloomCol;

    public bool highlighted;

    public SpriteRenderer staticRenderer;
    public RenderTexture renderTexture;

    public AudioSource hummingSfx;
    public PlaySound staticFadeInSfx;
    public PlaySound staticFadeOutSfx;

    public int renderIndex;

    public bool forceStatic;
    public AudioSource forceStaticSfx;

    public float targetForceStaticVolume = 1f;

    public MeshRenderer screenRenderer;

    public string camName;

    public static ScreenViewerManager screenViewerManager;
    void Start()
    {
        defaultBloomCol = bloom.color;
        Utilities.ResizeRenderTexture(renderTexture, 64, 64);
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        if (screenViewerManager == null) { screenViewerManager = GameObject.Find("ScreenViewer").GetComponent<ScreenViewerManager>(); }
        screenViewerManager.camRenderManager.screensDictionary.Add(renderIndex, this);
    }
    void Update()
    {
        bloom.color = Color.Lerp(bloom.color, highlighted ? Color.white : defaultBloomCol, Time.deltaTime * 5f);
        staticRenderer.color = Color.Lerp(staticRenderer.color, forceStatic?Utilities.Visible(staticRenderer.color) : Utilities.Invisible(staticRenderer.color), Time.deltaTime * 4f);
        hummingSfx.volume = Mathf.Lerp(hummingSfx.volume, highlighted ? 0.25f : 0f, Time.deltaTime * 4f);
        forceStaticSfx.volume = Mathf.Lerp(forceStaticSfx.volume, forceStatic ? targetForceStaticVolume : 0f, Time.deltaTime * 10f);
    }
}
