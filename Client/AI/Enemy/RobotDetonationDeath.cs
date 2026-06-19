using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDetonationDeath : RobotDeath
{
    public Detonation detonation;
    bool exploded;
    public override void Die(Damage damage)
    {
        if (!exploded)
        {
            StartCoroutine(DelayedExplosion(damage));
        }
    }
    IEnumerator DelayedExplosion(Damage damage)
    {
        if (!exploded) { exploded = true; }
        yield return new WaitForEndOfFrame();
        detonation.Explode();
        base.Die(damage);
    }
}
