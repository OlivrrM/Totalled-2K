using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGate : Interactable
{
    public PlaySound openSfx;
    public PlaySound lockAttemptSfx;
    public Animator gateAnimatorA;
    public Animator gateAnimatorB;

    public Rigidbody lockRb;

    public Vector3 lockBreakPushForceA;
    public Vector3 lockBreakPushForceB;

    public Vector3 lockBreakTorqueForceA;
    public Vector3 lockBreakTorqueForceB;

    bool locked;

    public PlaySound lockBreakSfx;
    private void Start()
    {
        description = "Locked";
        locked = true;
    }
    public override void Interact()
    {
        base.Interact();
        if (!locked){
            gateAnimatorA.SetBool("Open", true);
            gateAnimatorB.SetBool("Open", true);
            openSfx.Play();
            gameObject.SetActive(false);
        }
        else { lockAttemptSfx.Play(); }
    }
    public void Unlock()
    {
        locked = false;
        description = "Open";
        lockRb.isKinematic = false;
        lockRb.transform.parent = null;
        lockBreakSfx.Play();
        lockRb.AddForce(Utilities.RandomRangeVector3(lockBreakPushForceA,lockBreakPushForceB));
        lockRb.AddTorque(Utilities.RandomRangeVector3(lockBreakTorqueForceA,lockBreakTorqueForceB));
    }
}
