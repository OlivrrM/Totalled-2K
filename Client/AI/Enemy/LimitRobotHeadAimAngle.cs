using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LimitRobotHeadAimAngle : MonoBehaviour
{
    public RobotHeadAim robotHeadAim;
    public Transform forward;
    public Transform target;
    public float angleAmount;

    public MultiAimConstraint aim;
    public float behindWeight;
    public float weightSpeed;
    float targetWeight;
    private void Update()
    {
        Vector3 directionToTarget = target.position - forward.position;
        directionToTarget.Normalize();
        float angle = Vector3.Angle(-forward.forward, directionToTarget);
        if (angle <= angleAmount / 2)
        {
            robotHeadAim.paused = true;
            targetWeight = behindWeight;
        }
        else
        {
            robotHeadAim.paused = false;
            targetWeight = 1f;
        }

        aim.weight = Mathf.Lerp(aim.weight, targetWeight, Time.deltaTime * weightSpeed);
    }
}
