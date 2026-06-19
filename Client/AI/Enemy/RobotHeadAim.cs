using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RobotHeadAim : MonoBehaviour
{
    public Robot robot;
    public Transform headRig;
    public Transform target;

    public float turnSmoothness;

    [HideInInspector] public Vector3 currentPos;
    [HideInInspector] public bool paused;

    float headHeight;

    public bool lookAtPlayerOnAttack;

    private void Start()
    {
        headHeight = Vector3.Distance(robot.transform.position, headRig.position);
    }
    private void Update()
    {
        if (robot.currentDesination != Vector3.zero && !robot.forceStandStill && !paused && robot.stunRegaining){
            currentPos = robot.currentDesination + new Vector3(0f, headHeight, 0f);
        }
        target.position = Vector3.Lerp(target.position, currentPos, Time.deltaTime * turnSmoothness) + ((currentPos - headRig.position).normalized);
        if (robot.currentActionState == Totalled.RobotActionState.Idle || !robot.stunRegaining) { target.position = new Vector3(target.position.x, headHeight, target.position.z); }
        else if (((robot.currentActionState == Totalled.RobotActionState.Attacking || robot.currentActionState == Totalled.RobotActionState.Chasing) && lookAtPlayerOnAttack) || robot.isJumping) { target.position = Cache.vcam.transform.position; }
    }
}
