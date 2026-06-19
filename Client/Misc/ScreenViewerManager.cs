using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenViewerManager : MonoBehaviour
{
    public bool active = true;

    CctvInspectableScreen currentHighlighted;

    bool currentlyInspectingCam;

    bool uiHiddenPriorToView;

    public Material[] uiMats;

    float timeSinceLastInspect;

    [HideInInspector] public CctvCameraRenderManager camRenderManager;
    private void Start()
    {
        camRenderManager = GameObject.Find("CctvCam").GetComponent<CctvCameraRenderManager>();
    }
    public void FixedUpdate() // ScreenPointToRay REQUIRES it to be used in FixedUpdate, due to cinemachine blend update being in fixed update. Weird ass shit
    {
        if (active && StartTime.instanceTime>10f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!currentlyInspectingCam)
            {
                if (Physics.Raycast(ray, out hit, 99f, Cache.references.solidLayerMask))
                {
                    //print(hit.transform.name);
                    //Instantiate(TEST, hit.point, Quaternion.identity);
                    if (hit.collider.CompareTag("Screen"))
                    {
                        if (currentHighlighted == null || currentHighlighted != hit.transform)
                        {
                            if (currentHighlighted != null) { currentHighlighted.highlighted = false; }
                            CctvInspectableScreen screen = hit.transform.GetComponent<CctvInspectableScreen>();
                            screen.highlighted = true;
                            currentHighlighted = screen;
                        }
                    }
                    else
                    {
                        if (currentHighlighted != null) { currentHighlighted.highlighted = false; currentHighlighted = null; }
                    }
                }
                else
                {
                    if (currentHighlighted != null) { currentHighlighted.highlighted = false; currentHighlighted = null; }
                }
            }
            timeSinceLastInspect += Time.deltaTime;
            if (currentlyInspectingCam)
            {
                if (((InputManager.GetEscapeKeyDown() || Utilities.GetCursorDirection().y < -0.8f) && Cache.activeVcamManager.currentVcam != Cache.vcam) || (Cache.activeVcamManager.currentVcam == Cache.vcam && InputManager.GetInteractKeyDown()))
                {
                    currentHighlighted.staticFadeOutSfx.Play();
                    currentHighlighted.vcam.Priority = -1;
                    currentlyInspectingCam = false;
                    if (!uiHiddenPriorToView) { Cache.hideUi.hidden = false; }
                    currentHighlighted.staticRenderer.color = Color.white;
                    Utilities.ResizeRenderTexture(currentHighlighted.renderTexture, 64, 64);
                    camRenderManager.RenderTargetIndex(currentHighlighted.renderIndex);
                    camRenderManager.Unfocus();
                    timeSinceLastInspect = 0f;
                }
            }
        }
        if ((((InputManager.GetMainActionKey()&&Cache.activeVcamManager.currentVcam!=Cache.vcam)) || ((Cache.activeVcamManager.currentVcam == Cache.vcam)&&InputManager.GetInteractKeyDown()))&&timeSinceLastInspect>0.2f)
        {
            if (currentHighlighted != null)
            {
                currentHighlighted.vcam.Priority = 100;
                currentlyInspectingCam = true;
                currentHighlighted.highlighted = false;
                uiHiddenPriorToView = Cache.hideUi.hidden;
                currentHighlighted.staticRenderer.color = Color.white;
                Utilities.ResizeRenderTexture(currentHighlighted.renderTexture, 256, 256);
                camRenderManager.RenderTargetIndex(currentHighlighted.renderIndex);
                camRenderManager.FocusTarget(currentHighlighted.renderIndex);
                if (active) { currentHighlighted.staticFadeInSfx.Play(); }
                timeSinceLastInspect = 0f;
            }
        }
    }
    public IEnumerator ForceFocusTargetScreen(int id)
    {
        currentHighlighted.staticFadeOutSfx.Play();
        currentHighlighted.vcam.Priority = -1;
        currentlyInspectingCam = false;
        if (!uiHiddenPriorToView) { Cache.hideUi.hidden = false; }
        currentHighlighted.staticRenderer.color = Color.white;
        Utilities.ResizeRenderTexture(currentHighlighted.renderTexture, 64, 64);
        camRenderManager.RenderTargetIndex(currentHighlighted.renderIndex);
        camRenderManager.Unfocus();
        //timeSinceLastInspect = 0f;

        yield return new WaitForSeconds(0.5f);

        currentHighlighted = camRenderManager.screensDictionary[id];
        camRenderManager.screensDictionary[id].vcam.Priority = 100;
        currentlyInspectingCam = true;
        camRenderManager.screensDictionary[id].highlighted = false;
        uiHiddenPriorToView = Cache.hideUi.hidden;
        camRenderManager.screensDictionary[id].staticRenderer.color = Color.white;
        Utilities.ResizeRenderTexture(camRenderManager.screensDictionary[id].renderTexture, 256, 256);
        camRenderManager.RenderTargetIndex(camRenderManager.screensDictionary[id].renderIndex);
        //camRenderManager.FocusTarget(camRenderManager.screensDictionary[id].renderIndex);
        if (active) { camRenderManager.screensDictionary[id].staticFadeInSfx.Play(); }
        //timeSinceLastInspect = 0f;
        
    }
    private void Update()
    {
        if (Cache.activeVcamManager.currentVcam != Cache.vcam){
            for (int i = 0; i < uiMats.Length; i++){
                uiMats[i].color = Color.Lerp(uiMats[i].color, currentlyInspectingCam ? Color.white : Utilities.Invisible(Color.white), Time.deltaTime * 5f);
            }
        }
    }
}
