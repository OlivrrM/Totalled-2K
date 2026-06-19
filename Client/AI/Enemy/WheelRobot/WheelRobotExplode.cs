using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRobotExplode : MonoBehaviour
{
    public Robot robot;
    public RobotDeath robotDeath;
    public float explodeRadius;
    private void Update()
    {
        if (robot.distanceFromTarget < explodeRadius) { if (!Robot.ghost && robot.chasingLOS) robotDeath.Die(new Damage()); }
    }
}
