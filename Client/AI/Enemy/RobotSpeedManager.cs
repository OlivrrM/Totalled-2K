using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSpeedManager : ValueMultiplierManager
{
    public Robot robot;
    private void Update()
    {
        robot.agent.speed = robot.baseSpeed;
        foreach (KeyValuePair<string, object> value in values)
        {
            robot.agent.speed *= System.Convert.ToSingle(value.Value);
        }
    }
}
