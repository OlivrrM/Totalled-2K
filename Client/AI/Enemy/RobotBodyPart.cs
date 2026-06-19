using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBodyPart : MonoBehaviour, IDamageable, IStunnable
{
    public RobotHealth robot; //This should be called RobotHealth
    public float damageMultiplier = 1f;
    public bool critical;
    public virtual void Damage(Damage damage)
    {
        damage.amount *= damageMultiplier;
        robot.Damage(damage, critical);
    }
    public void Stun(float amount, float imobileTime, float regainSpeedTime)
    {
        robot.robot.Stun(amount,imobileTime, regainSpeedTime);
    }
}
