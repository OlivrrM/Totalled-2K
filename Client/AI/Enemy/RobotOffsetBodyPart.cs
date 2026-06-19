using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RobotOffsetBodyPart : RobotBodyPart
{
    public OverrideTransform overrideTransform;

    public float offsetSmoothness;
    public float offsetSpeed;
    float targetOffset;
    float currentOffset;

    public bool explosionDamage; //Mistake. Needs to be in base class
    public bool fireDamage;

    public override void Damage(Damage damage)
    {
        if (damage is ExplosionDamage)
        {
            if (explosionDamage)
            {
                base.Damage(damage);
            }
            return;
        }
        else if (damage.type == Totalled.DamageType.FireTick)
        {
            if (fireDamage)
            {
                base.Damage(damage);
            }
            return;
        }
        else { base.Damage(damage); }
        targetOffset = 1f;
    }
    private void Update()
    {
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, Time.deltaTime * offsetSmoothness);
        targetOffset = Mathf.Lerp(targetOffset, 0f, Time.deltaTime * offsetSpeed);
        overrideTransform.weight = currentOffset;
    }
}
