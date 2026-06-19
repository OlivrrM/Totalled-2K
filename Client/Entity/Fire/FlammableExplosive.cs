using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableExplosive : Explosive
{
    public Flammable flammable;
    bool lit;
    public override void Damage(Damage damage)
    {
        base.Damage(damage);
        if (!lit) { flammable.Light(); lit = true; }
    }
}
