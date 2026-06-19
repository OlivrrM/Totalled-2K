using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Detonation
{
    public Rigidbody rb;
    public override void Start()
    {
        base.Start();
        rb.AddForce(transform.forward * Cache.grenadeManager.currentThrowForce,ForceMode.Impulse);
    }
}
