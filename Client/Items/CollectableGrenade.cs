using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableGrenade : Pickup
{
    public PlaySound pickupSfx;
    public override void Take()
    {
        base.Take();
        if (Cache.grenadeManager.CanPickup()) { Cache.grenadeManager.PickupGrenade(); pickupSfx.Play(); }
        else { CancelPickup(); Cache.grenadeManager.CancelPickup(); }
    }
}
