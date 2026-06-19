using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Breakable
{
    public Detonation detonation;
    bool detonated;
    public override void Break(Damage damage)
    {
        detonation.Explode();
        base.Break(damage);
    }
}
