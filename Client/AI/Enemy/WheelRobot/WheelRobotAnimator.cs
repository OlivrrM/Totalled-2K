using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRobotAnimator : MonoBehaviour
{
    public Robot robot;

    public AgentForwardMovementOnly agentForwardMovementOnly;
    public float driftAmount;

    public Transform wheel;
    public float spinSpeed;

    public LocalRotationOffsetManager localRotationOffsetManager;
    private void Start()
    {
        localRotationOffsetManager.AddValue("Woah", Vector3.zero);
    }

    private void Update()
    {
        wheel.Rotate(new Vector3((robot.agent.velocity.magnitude*spinSpeed) * Time.deltaTime, 0f, 0f));
        localRotationOffsetManager.UpdateValue("Woah", new Vector3(0f, 0f, agentForwardMovementOnly.crossProduct.y * driftAmount)); 
    }
}
