using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotgunPropel : MonoBehaviour
{
    public float power;
    public KeyCode shootKey;

    bool keyDown;
    float keyDownTime;

    public Transform gun;
    public Vector3 hidePos;
    public Vector3 showPos;
    public float showSpeed;
    public float hideSpeed;
    public float spinShowSpeed;
    public float spinHideSpeed;

    public float rotateAmount;
    public float flipSpeed;
    Quaternion defaultRotation;

    public Animator gunAnimator;

    float chargeTime;
    public float requiredTimeToShoot;
    float timeAfterShoot;

    public PlaySound shootSfx;
    public PlaySound equipSfx;

    public RectTransform[] crosshairRects;
    public Image[] crosshairImages;
    float currentCrosshairSize;
    float targetCrosshairSize;
    public float crossHairSizeSpeed;
    Color defaultCrosshairColor;
    public float crosshairColorSpeed;
    Color targetCrosshairColor;
    private void Start()
    {
        defaultCrosshairColor = crosshairImages[0].GetComponent<Image>().color;
    }
    private void Update()
    {
        if (Input.GetKeyDown(shootKey)) { keyDown = true; keyDownTime = 0f; Cache.armManager.Hide(4f); equipSfx.Play(); currentCrosshairSize = 0; }
        else if (Input.GetKeyUp(shootKey))
        {
            keyDown = false;
            if (keyDownTime > requiredTimeToShoot) {  StartCoroutine(DelayedShowArms()); }
            else { Cache.armManager.Show(3f); targetCrosshairSize = 0f; targetCrosshairColor = Utilities.Invisible(defaultCrosshairColor); }
        }
        if (keyDown)
        {
            gunAnimator.SetBool("Return", false);
            gun.localPosition = Vector3.Lerp(gun.localPosition, showPos, Time.deltaTime * showSpeed);
            keyDownTime += Time.deltaTime;
            targetCrosshairSize = 1f;
            targetCrosshairColor = defaultCrosshairColor;
        }
        else
        {
            if (timeAfterShoot > 0.8f){
                gunAnimator.SetBool("Return", true);
                gun.localPosition = Vector3.Lerp(gun.localPosition, hidePos, Time.deltaTime * hideSpeed);
            }
            else
            {
                gun.localPosition = Vector3.Lerp(gun.localPosition, hidePos, Time.deltaTime * hideSpeed/5f);
            }
            targetCrosshairColor = Utilities.Invisible(defaultCrosshairColor);
        }
        gunAnimator.SetBool("Shoot", false);
        timeAfterShoot += Time.deltaTime;
        if (keyDownTime > requiredTimeToShoot){
            if (!Input.GetKey(shootKey)){
                Shoot();
                gunAnimator.SetBool("Return", false);
                gunAnimator.SetBool("Shoot", true);
                timeAfterShoot = 0f;
                keyDownTime = 0f;
            }
        }
        if (timeAfterShoot > 0.8f) { gunAnimator.SetBool("Active", keyDown); }
        gunAnimator.SetFloat("ShowSpeed", spinShowSpeed);
        gunAnimator.SetFloat("HideSpeed", spinHideSpeed);


        currentCrosshairSize = Mathf.Lerp(currentCrosshairSize, targetCrosshairSize, Time.deltaTime * crossHairSizeSpeed);
        for (int i = 0; i < crosshairRects.Length; i++){
            crosshairRects[i].localScale = Utilities.V3All(currentCrosshairSize);
        }
        for (int i = 0; i < crosshairImages.Length; i++){
            crosshairImages[i].color = Color.Lerp(crosshairImages[i].color, targetCrosshairColor, Time.deltaTime * crosshairColorSpeed);
        }
    }
    void Shoot()
    {
        Cache.moveData.Velocity += -(Cache.mainCam.forward * power);
        shootSfx.Play();
        targetCrosshairSize = 2;
        targetCrosshairColor = Utilities.Invisible(defaultCrosshairColor);
    }
    IEnumerator DelayedShowArms()
    {
        yield return new WaitForSeconds(1f);
        Cache.armManager.Show(2.5f);
    }
}
