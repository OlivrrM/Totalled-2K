using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public float crouchSmoothness;
    float currentCrouchAmount;
    float targetCrouchAmount;

    public float crouchSpeedMultiplier;
    float currentCrouchSpeed;
    float targetCrouchSpeed;

    float defaultCamPosY;
    float defaultColSizeY;

    public float camBobAmount;

    [HideInInspector] public bool crouching;

    public LayerMask checkAboveLayerMask;

    public PostFxVolumeManager forceCrouchPostFxManager;

    public PlayRandomSound crouchSfx;

    public LocalOffsetManager handheldCameraLocalOffsetManager;
    public Vector3 handheldCrouchOffset;
    public float handheldCrouchOffsetSpeed;
    Vector3 targetHandheldCrouchOffset;
    Vector3 currentHandheldCrouchOffset;

    public event Action OnStartCrouch;
    public event Action OnEndCrouch;
    [HideInInspector] public bool enableCrouching = true;

    bool crouchPressed;
    float crouchPressCd;
    bool crouchInput;
    private void Start()
    {
        currentCrouchAmount = 1f;
        targetCrouchAmount = 1f;
        currentCrouchSpeed = 1f;
        targetCrouchSpeed = 1f;
        Cache.playerColliderSizeManager.AddValue("CrouchingColliderSize", Vector3.one);
        Cache.camViewOffsetManager.AddValue("CrouchingCamPos", Vector3.zero);
        Cache.walkSpeedManager.AddValue("CrouchingSpeed", 1f);
        defaultCamPosY = Cache.surfCharacter.ViewOffset.y;
        defaultColSizeY = Cache.surfCharacter.ColliderSize.y;
        Cache.camBobManager.AddNewBob("CrouchingBob", 5, 5, true);
        Cache.crouch = this;
        handheldCameraLocalOffsetManager.AddValue("HandheldCrouchOffset", Vector3.zero);
    }
    private void Update()
    {
        bool forceCrouch = false;
        float forceDistance = defaultColSizeY;
        RaycastHit hit;
        //Debug.DrawRay(Cache.surfCharacter.transform.position, Vector3.up* defaultColSizeY, Color.blue, 5f);
        if (FragMovementListener.grounded)
        {
            if (Physics.Raycast(Cache.surfCharacter.transform.position, Vector3.up, out hit, defaultColSizeY, checkAboveLayerMask))
            {
                forceCrouch = true;
                forceDistance = hit.distance;
            }
        }

        if (forceCrouch) { forceCrouchPostFxManager.Enable(1f); }
        else { forceCrouchPostFxManager.Disable(3f); }

        if (crouchPressCd > 0f)
        {
            crouchPressCd -= Time.deltaTime;
            if (crouchPressCd <= 0f)
            {
                crouchPressed = false;
            }
        }
        if (InputManager.GetCrouchKeyDown())
        {
            if (crouchPressed && crouchPressCd > 0f)
            {
                crouchInput = true;
            }
            crouchPressed = true;
            crouchPressCd = 0.5f;
        }
        if (InputManager.GetCrouchKeyUp())
        {
            crouchInput = false;
        }


        if (((InputManager.GetCrouchKeyDown()/*crouchInput*/ && FragMovementListener.grounded)|| forceCrouch)&&!crouching) { StartCrouching(); }
        else if (/*!crouchInput*/!InputManager.GetCrouchKey() && FragMovementListener.grounded && crouching&& !forceCrouch) { EndCrouching(); }

        targetHandheldCrouchOffset = handheldCrouchOffset * Utilities.BoolToInt(crouching);
        currentHandheldCrouchOffset = Vector3.Lerp(currentHandheldCrouchOffset,targetHandheldCrouchOffset,Time.deltaTime*handheldCrouchOffsetSpeed);
        handheldCameraLocalOffsetManager.UpdateValue("HandheldCrouchOffset",currentHandheldCrouchOffset);

        currentCrouchAmount = Mathf.Lerp(currentCrouchAmount, targetCrouchAmount, Time.deltaTime * crouchSmoothness);
        if (enableCrouching) { Cache.playerColliderSizeManager.UpdateValue("CrouchingColliderSize", new Vector3(1f, currentCrouchAmount, 1f)); }
        Cache.camViewOffsetManager.UpdateValue("CrouchingCamPos", new Vector3(0f, -((defaultCamPosY) * (1 - (currentCrouchAmount))), 0f));
        currentCrouchSpeed = Mathf.Lerp(currentCrouchSpeed, targetCrouchSpeed, Time.deltaTime * crouchSmoothness);
        Cache.walkSpeedManager.UpdateValue("CrouchingSpeed", currentCrouchSpeed);
    }
    public void StartCrouching()
    {
        OnStartCrouch?.Invoke();
        targetCrouchAmount = 0.5f;
        targetCrouchSpeed = crouchSpeedMultiplier;
        Cache.camBobManager.AddBobForce("CrouchingBob", camBobAmount);
        crouching = true;
        crouchSfx.Play(0.8f, 0.9f);
    }
    public void EndCrouching()
    {
        OnEndCrouch?.Invoke();
        targetCrouchAmount = 1f;
        targetCrouchSpeed = 1f;
        Cache.camBobManager.AddBobForce("CrouchingBob", -camBobAmount);
        crouching = false;
        crouchSfx.Play(1.1f, 1.2f);
    }
}
