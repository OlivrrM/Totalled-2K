using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAlarmTrigger : MonoBehaviour
{
    public bool active;
    public PlaySound alarmSfx;

    public float alarmTime;
    float currentAlarmTime;
    public void Trigger()
    {
        if (active)
        {
            alarmSfx.Play();
            currentAlarmTime = 30f;
            Collider[] colliders = Physics.OverlapSphere(transform.position, alarmSfx.SFX.maxDistance * 0.75f, Cache.references.enemyLayerMask);
            foreach (Collider col in colliders)
            {
                RobotBodyPart robotBodyPart = col.GetComponent<RobotBodyPart>();
                if (robotBodyPart != null)
                {
                    robotBodyPart.robot.robot.alarmAgroMultiplier = 6f;
                }
            }
        }
    }
    private void Update()
    {
        if (currentAlarmTime > 0f)
        {
            currentAlarmTime -= Time.deltaTime;
            if (currentAlarmTime <= 0f) { alarmSfx.SFX.Stop(); }
        }
    }
}
