using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankBarrier : MonoBehaviour, IDamageable
{
    public BoxCollider thisCollider;
    [HideInInspector] public bool broken;

    public Rigidbody plankA;
    public Rigidbody plankB;

    public Vector3 plankATorqueForce;
    public Vector3 plankBTorqueForce;

    public Vector3 pushForce;

    public void Damage(Damage damage)
    {
        if (damage.type==Totalled.DamageType.Bullet|| damage.type == Totalled.DamageType.Melee || damage.type == Totalled.DamageType.Force || damage.type == Totalled.DamageType.Explosion)
        {
            if (!broken){
                thisCollider.enabled = false;
                broken = true;

                plankA.isKinematic = false;
                plankB.isKinematic = false;

                plankA.AddTorque(plankATorqueForce);
                plankB.AddTorque(plankBTorqueForce);

                plankA.AddForce(pushForce);
                plankB.AddForce(pushForce);
            }
        }
    }
}
