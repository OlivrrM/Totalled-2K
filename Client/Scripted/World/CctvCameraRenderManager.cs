using Fragsurf.Movement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CctvCameraRenderManager : MonoBehaviour
{
    public Camera cctvCamera;
    public List<Transform> cameraPositions;
    public List<float> cameraFovs;
    public List<RenderTexture> renderTargets;
    public float cycleDuration = 1f;
    public float focusDuration = 0.0333f;

    float unfocusTime;
    int previousFocus;

    private int currentIndex = 0;
    private float time = 0f;
    private float timePerCam;
    public int focusIndex = -1;

    [HideInInspector] public bool disableCamMovement;

    public CanvasGroup warningCanvas;
    public TextMeshProUGUI warningCamText;
    public int warningIndex = -1;

    [HideInInspector] public Dictionary<int, CctvInspectableScreen> screensDictionary = new Dictionary<int, CctvInspectableScreen>();

    void OnEnable()
    {
        if (cameraPositions.Count == 0 || renderTargets.Count == 0 || cctvCamera == null)
        {
            Debug.LogWarning("CCTVManager is not properly configured.");
            enabled = false;
            return;
        }

        cctvCamera.enabled = false; // Prevent automatic rendering
        timePerCam = cycleDuration / cameraPositions.Count;
        time = 0f;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (focusIndex <= -1)
        {
            if (time >= timePerCam)
            {
                time -= timePerCam;

                RenderTargetIndex(currentIndex);

                // Do NOT reset targetTexture to null!
                // Keeps camera off the screen and renders only to texture

                currentIndex = (currentIndex + 1) % cameraPositions.Count;
            }
            if (focusIndex == -2)
            {
                unfocusTime += Time.deltaTime;
                if (unfocusTime >= 1f) { focusIndex = -1; }
                if (time >= focusDuration){
                    time -= focusDuration;
                    RenderTargetIndex(previousFocus);
                }
            }
        }
        else
        {
            if (time >= focusDuration){
                time -= focusDuration;
                RenderTargetIndex(focusIndex);
            }
        }

        if (warningIndex != focusIndex && focusIndex>=0 && warningIndex>=0){
            warningCanvas.alpha = Mathf.Lerp(warningCanvas.alpha, 1f, Time.deltaTime * 4f);
        }
        else if (focusIndex >= 0){
            warningCanvas.alpha = Mathf.Lerp(warningCanvas.alpha, 0f, Time.deltaTime * 4f);
        }
        else{
            warningCanvas.alpha = 0f;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SetCamWarning(4,"HELLO");
        }
    }
    public void FocusTarget(int targetIndex) 
    { 
        if (targetIndex == focusIndex && warningIndex != -1 && warningIndex != focusIndex){
            StartCoroutine(CctvInspectableScreen.screenViewerManager.ForceFocusTargetScreen(warningIndex));
            FocusTarget(warningIndex);
            ClearWarning();
            //StartCoroutine(DelayedRefreshAllRenderTargets());
            return;
        }
        focusIndex = targetIndex; 
    }
    public void Unfocus() { previousFocus = focusIndex; focusIndex = -2; unfocusTime = 0f; }// StartCoroutine(DelayedRefreshAllRenderTargets()); }
    public void SetCamWarning(int targetIndex, string camName) {
        warningIndex = targetIndex;
        warningCamText.text = $"<u>WARNING</u>\r\nCAM {camName} TAMPERED WITH";
    }
    IEnumerator DelayedRefreshAllRenderTargets()
    {
        yield return new WaitForEndOfFrame();
        RefreshAllRenderTargets();
    }
    public void RefreshAllRenderTargets()
    {
        for (int i = 0; i < renderTargets.Count; i++){
            RenderTargetIndex(i);
        }
    }
    public void ClearWarning() { warningIndex = -1; }
    public void RenderTargetIndex(int targetIndex)
    {
        cctvCamera.fieldOfView = cameraFovs[targetIndex];
        if (!disableCamMovement){
            cctvCamera.transform.SetPositionAndRotation(
                cameraPositions[targetIndex].position,
                cameraPositions[targetIndex].rotation
            );
        }
        cctvCamera.targetTexture = renderTargets[targetIndex];
        cctvCamera.UpdateVolumeStack();
        cctvCamera.Render();
    }
}
