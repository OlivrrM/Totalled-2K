using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch16 : MonoBehaviour
{
    public static bool active;
    bool crouching;
    bool groundedOnCrouch;
    bool groundedOnUncrouch;
    float currentCrouchAmount;
    float targetCrouchAmount;
    private void Awake()
    {
        Cache.crouch16 = this;
    }
    private void Start()
    {
        Cache.playerColliderSizeManager.AddValue("Crouch16",Vector3.one);
        if (!active) { enabled = false; }
        else { Cache.crouch.enableCrouching = false; }
    }

    private void OnEnable()
    {
        if (Cache.crouch != null)
        {
            Cache.crouch.OnStartCrouch += HandleStartCrouch;
            Cache.crouch.OnEndCrouch += HandleEndCrouch;
        }
    }

    private void OnDisable()
    {
        if (Cache.crouch != null)
        {
            Cache.crouch.OnStartCrouch -= HandleStartCrouch;
            Cache.crouch.OnEndCrouch -= HandleEndCrouch;
        }
    }
    private void Update()
    {
        if (crouching)
        {
            targetCrouchAmount = 0.5f;
            groundedOnUncrouch = false;
            if (FragMovementListener.grounded && groundedOnCrouch){
                currentCrouchAmount = Mathf.Lerp(currentCrouchAmount, targetCrouchAmount, Time.deltaTime * 2f);
            }
            else{
                currentCrouchAmount = targetCrouchAmount;
            }
        }
        else
        {
            targetCrouchAmount = 1f;
            groundedOnCrouch = false;
            if (FragMovementListener.grounded && groundedOnUncrouch){
                currentCrouchAmount = Mathf.Lerp(currentCrouchAmount, targetCrouchAmount, Time.deltaTime * 6f);
            }
            else{
                currentCrouchAmount = targetCrouchAmount;
            }
        }
        Cache.playerColliderSizeManager.UpdateValue("Crouch16", new Vector3(1f, currentCrouchAmount, 1f));
    }

    void HandleStartCrouch()
    {
        crouching = true;
        if (FragMovementListener.grounded) { groundedOnCrouch = true; }
    }

    void HandleEndCrouch()
    {
        crouching = false;
        if (FragMovementListener.grounded) { groundedOnUncrouch = true; }
    }
}
