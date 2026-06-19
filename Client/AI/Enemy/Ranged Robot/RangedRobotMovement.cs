using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class RangedRobotMovement : MonoBehaviour
{
    public Robot robot;
    public float kiteDistance;

    public float directionTime;
    float currentDirectionTime;
    float direction;

    public float stepAmount;
    public float rotationSpeed;

    float timekiting;
    bool kiting;

    private void Start()
    {
        direction = Utilities.CoinFlipn();
    }
    private void LateUpdate()
    {
        if (!kiting) { timekiting = 0f; }
        kiting = false;
        if (robot.distanceFromTarget < kiteDistance && robot.stunRegaining)
        {
            if (robot.currentActionState == RobotActionState.Chasing)
            {
                if (robot.chasingLOS)
                {
                    timekiting += Time.deltaTime;
                    kiting = true;

                    robot.SetDestination(transform.position + ((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.right) * (stepAmount * direction)));
                    currentDirectionTime += Time.deltaTime;
                    if (currentDirectionTime > directionTime)
                    {
                        direction = -direction;
                        currentDirectionTime = 0f;
                    }

                    Vector3 directionToPlayer = (robot.lastKnownTargetPos - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (rotationSpeed * Mathf.Clamp(timekiting,0f,1.5f)));
                }
            }
        }
    }
}
