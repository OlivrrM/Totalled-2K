using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAndJumpRobotOnTrigger : MonoBehaviour
{
    public Robot robot;
    public Transform jumpPos;
    private void Start()
    {
        robot.manualControl = true;
    }
    public void Trigger()
    {
        StartCoroutine(robot.PerformJump(jumpPos.position));
        robot.manualControl = false;
    }
}
