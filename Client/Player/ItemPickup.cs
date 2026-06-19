using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public float pickupRange;
    public Inventory inventory;
    public void Pickup(GameObject item)
    {
        if (Cache.inventory.items.Count < Cache.inventory.inventorySize)
        {
            Pickup pickup = item.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.Take();
                if (pickup is AcquireableItem)
                {
                    AcquireableItem pickup1 = (AcquireableItem)pickup;
                    StartCoroutine(PickupHandheldDelay(pickup1));
                }
            }
        }
    }
    IEnumerator PickupHandheldDelay(AcquireableItem pickup)
    {
        yield return new WaitForSeconds(0.3f);
        inventory.PickupItem(pickup);
    }
}
