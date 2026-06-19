using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPedalTrigger : Interactable
{
    public BrickPedalCarCrashSequence brickPedalCarCrashSequence;

    bool placed;
    public override void Interact()
    {
        if (!placed)
        {
            base.Interact();
            if (interactable){
                placed = true;
                brickPedalCarCrashSequence.Trigger();
                this.enabled = false;
            }
        }
    }
}
