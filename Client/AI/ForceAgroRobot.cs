using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAgroRobot : MonoBehaviour
{
    public Robot robot;
    public void Trigger()
    {
        robot.Agro();
        robot.lastKnownTargetPos = Cache.surfCharacter.transform.position;
    }
}
