using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotStepSFX : MonoBehaviour
{
    public Robot robot;
    public float idleSpeedMultiplier;
    public float chasingSpeedMultiplier;
    public PlayRandomSound sfx;

    float currentTime;

    public GameObject shakeImpulse;
    private void Update()
    {
        if (robot.agent.velocity.magnitude > 0.1f)
        {
            currentTime -= Time.deltaTime * (robot.agent.velocity.magnitude * (robot.currentActionState==Totalled.RobotActionState.Idle? idleSpeedMultiplier : chasingSpeedMultiplier));
            if (currentTime < 0f){ 
                sfx.Play();
                if (shakeImpulse != null) { Instantiate(shakeImpulse, transform.position, Quaternion.identity); }
                currentTime = 1f;
            }
        }
        else
        {
            currentTime = 1f;
        }
    }
}
