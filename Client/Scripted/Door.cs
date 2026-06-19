using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public Animator doorAnimator;
    bool open;

    public PlaySound openSound;
    public PlaySound closeSound;
    public PlaySound slamSound;

    Quaternion defaultRotation;
    bool slammed;

    public bool locked;
    public bool lockedAnimationState;
    public PlaySound lockedSfx;
    private void Start()
    {
        description = "Open";
        defaultRotation = doorAnimator.transform.rotation;
        if (locked) { Lock(); }
    }
    public override void Interact()
    {
        base.Interact();
        if (locked) { LockedAttempt(); }
        else { SwitchState(); }
    }
    public void Lock(bool declareLocked = false)
    {
        if (declareLocked) { description = "Locked"; }
        locked = true;
    }
    public void Unlock()
    {
        locked = false;
        description = !open ? "Open" : "Close";
    }
    void LockedAttempt(bool declareLocked = true)
    {
        if (!doorAnimator.GetBool("LockAttempt")) { lockedSfx.Play(); }
        if (lockedAnimationState) { doorAnimator.SetBool("LockAttempt", true); }
        if (declareLocked) { description = "Locked"; }

    }
    void SwitchState()
    {
        if (lockedAnimationState) { doorAnimator.SetBool("LockAttempt", false); }
        open = !open;
        slammed = false;
        if (open) { openSound.Play(); description = "Close"; }
        else { closeSound.Play(); description = "Open"; }
        doorAnimator.SetBool("Open", open);
    }
    private void Update()
    {
        if (!open && !slammed)
        {
            if (doorAnimator.transform.rotation == defaultRotation)
            {
                slammed = true;
                slamSound.Play();
                closeSound.SFX.Stop();
            }
        }
    }
}
