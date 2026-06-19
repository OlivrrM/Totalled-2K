using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint : MonoBehaviour
{
    public float sprintSpeedMultiplier;

    float targetSpeedMultiplier;
    float currentSpeedMultiplier;

    public float sprintSmoothness;

    public float fovIncrease;
    float targetFov;
    float currentFov;

    public float runBobSpeed;
    float currentRunBobSpeed;
    float targetRunBobSpeed;

    public float runBobAmount;
    float currentRunBobAmount;
    float targetRunBobAmount;

    bool started;

    public PostFxVolumeManager sprintingPostFxVolumeManager;

    public Crouch crouch;

    bool forwardPressed;
    float forwardPressCd;
    bool sprintInput;

    float timeSprinting;
    bool sprinting;
    private void Start()
    {
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        Cache.walkSpeedManager.AddValue("SprintingWalkSpeed", 1f);
        Cache.fovManager.AddValue("SprintingFov", new FovMultiplier { value = 1f });

        targetSpeedMultiplier = 1f;
        currentFov = 1f;
        currentSpeedMultiplier = 1f;
        targetFov = 1f;

        Cache.runBob.speedMultipliers.Add("Sprinting", 1f);
        Cache.runBob.amountMultipliers.Add("Sprinting", 1f);

        started = true;
    }
    private void Update()
    {
        if (started)
        {
            if (forwardPressCd > 0f)
            {
                forwardPressCd -= Time.deltaTime;
                if (forwardPressCd <= 0f){
                    forwardPressed = false;
                }
            }
            if (InputManager.GetMoveForwardKeyDown()){
                if (forwardPressed && forwardPressCd > 0f){
                    sprintInput = true;
                }
                forwardPressed = true;
                forwardPressCd = 0.5f;
            }
            if (InputManager.GetMoveForwardKeyUp())
            {
                sprintInput = false;
            }

            if (sprinting){
                timeSprinting += Time.deltaTime;
            }

            if ((/*sprintInput||*/InputManager.GetSprintKey()) && FragMovementListener.grounded && /*FragMovementListener.hSpeedPercentage>0.1f&&*/!crouch.crouching&&!Firearm.globalCurrentlyReloading&&!Cache.stabilize.stabilized) { StartSprinting(); }
            else if (((!InputManager.GetSprintKey()/* && !sprintInput*/) && FragMovementListener.grounded)||(FragMovementListener.hSpeedPercentage<0.1f&&timeSprinting>0.5f)||crouch.crouching||Firearm.globalCurrentlyReloading||Cache.stabilize.stabilized) { StopSprinting(); }

            currentSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetSpeedMultiplier, Time.deltaTime * sprintSmoothness);
            Cache.walkSpeedManager.UpdateValue("SprintingWalkSpeed", currentSpeedMultiplier);

            currentFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * (sprintSmoothness*0.75f));
            Cache.fovManager.UpdateValue("SprintingFov", new FovMultiplier { value = currentFov });

            currentRunBobSpeed = Mathf.Lerp(currentRunBobSpeed, targetRunBobSpeed, Time.deltaTime * sprintSmoothness);  
            Cache.runBob.speedMultipliers["Sprinting"] = currentRunBobSpeed;
            currentRunBobAmount = Mathf.Lerp(currentRunBobAmount, targetRunBobAmount, Time.deltaTime * sprintSmoothness);
            Cache.runBob.amountMultipliers["Sprinting"] = currentRunBobAmount;
        }
    }
    void StartSprinting()
    {
        targetSpeedMultiplier = sprintSpeedMultiplier;
        targetFov = fovIncrease;
        targetRunBobSpeed = runBobSpeed;
        sprintingPostFxVolumeManager.Enable(sprintSmoothness);
        targetRunBobAmount = runBobAmount;
        sprinting = true;
    }
    void StopSprinting()
    {
        sprintInput = false;
        targetSpeedMultiplier = 1f;
        targetFov = 1f;
        targetRunBobSpeed = 1f;
        sprintingPostFxVolumeManager.Disable(sprintSmoothness);
        targetRunBobAmount = 1f;
        sprinting = false;
        timeSprinting = 0f;
    }
    
}
