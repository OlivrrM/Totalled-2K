using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polaroid : Item
{
    [Header("Polaroid")]
    public Light flash;
    float defaultIntensity;
    public float flashTime;
    bool turningOffFlash;
    public float flashTurnOffSpeed;

    public PlaySound flashSfx;
    public PlaySound flashChargeSfx;
    public PlaySound printSfx;

    public float timeUntilPrint;

    public GameObject polaroidFilm;
    PolaroidFilm currentPolaroidFilm;
    public Transform polaroidSpawnPos;

    [Header("Stun")]
    public GameObject stunCollider;
    public override void Start()
    {
        base.Start();
        defaultIntensity = flash.intensity;
        flash.intensity = 0f;
        targetCamera.enabled = false;
        turningOffFlash = true;
        stunCollider.SetActive(false);
    }
    public override void OnMainAction()
    {
        base.OnMainAction();
        StartCoroutine(Flash());
    }
    public override void Update()
    {
        base.Update();
        flash.intensity = turningOffFlash ? Mathf.Lerp(flash.intensity, 0f, Time.deltaTime * flashTurnOffSpeed) : defaultIntensity;
    }
    IEnumerator Flash()
    {
        flash.intensity = defaultIntensity;
        turningOffFlash = false;
        flashSfx.Play();
        flashChargeSfx.Play();
        GameObject newPolaroidFilm = Instantiate(polaroidFilm, polaroidSpawnPos.position, polaroidSpawnPos.rotation, polaroidSpawnPos);
        currentPolaroidFilm = newPolaroidFilm.GetComponent<PolaroidFilm>();
        currentPolaroidFilm.rb.isKinematic = true;
        stunCollider.SetActive(true);
        TakeScreenshot();
        yield return new WaitForSeconds(flashTime);
        turningOffFlash = true;
        stunCollider.SetActive(false);
        yield return new WaitForSeconds(timeUntilPrint);
        printSfx.Play();
        StartCoroutine(currentPolaroidFilm.Print());
    }
    [Header("Render")]
    public Camera targetCamera; 

    public int resolutionWidth = 64; 
    public int resolutionHeight = 64; 

    void TakeScreenshot()
    {
        targetCamera.enabled = true;

        RenderTexture renderTexture = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        renderTexture.filterMode = FilterMode.Point;
        targetCamera.targetTexture = renderTexture;

        RenderTexture.active = renderTexture;
        targetCamera.Render();

        Texture2D screenshotTexture = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        screenshotTexture.Apply();

        currentPolaroidFilm.AssignTexture(screenshotTexture);

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        targetCamera.enabled = false;
    }
}
