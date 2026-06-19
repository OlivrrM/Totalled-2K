using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public int inventorySize;
    public List<Item> items = new List<Item>();

    [HideInInspector] public Item equippedItem;
    [HideInInspector] public int equippedItemIndex;

    public InventoryUIManager inventoryUIManager;

    public ObjectRunBob itemRunBobManager;

    public float throwForce;
    public float throwTorqueForce;
    private void Start()
    {
        Cache.inventory = this;
        Cache.itemRunBobManager = itemRunBobManager;
        equippedItemIndex = -1;
        Cache.walkSpeedManager.AddValue("EquippedItem", 1f);
    }
    public void PickupItem(AcquireableItem item)
    {
        if (items.Count < inventorySize)
        {
            GameObject pickup = Instantiate(item.item, transform.position, Quaternion.identity,transform);
            pickup.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Item pickupItem = pickup.GetComponent<Item>();
            if (pickupItem is Firearm && item is AcquireableFirearm){
                int ammoInFirearm = ((AcquireableFirearm)item).ammo;
                Firearm fireArmItem = (Firearm)pickupItem;
                if (ammoInFirearm == -1) { fireArmItem.currentClipAmount = fireArmItem.ammoClipSize; }
                else { fireArmItem.currentClipAmount = ammoInFirearm; }
            }
            items.Add(pickupItem);
            pickup.name += items.Count.ToString();
            inventoryUIManager.AddSlot(pickupItem.inventorySlot);
            EquipItem(items.Count-1);
        }
    }
    public void PickupItem(GameObject item)
    {
        if (items.Count < inventorySize)
        {
            GameObject pickup = Instantiate(item, transform.position, Quaternion.identity, transform);
            pickup.transform.localRotation = Quaternion.Euler(Vector3.zero);
            Item pickupItem = pickup.GetComponent<Item>();
            items.Add(pickupItem);
            pickup.name += items.Count.ToString();
            inventoryUIManager.AddSlot(pickupItem.inventorySlot);
            EquipItem(items.Count - 1);
        }
    }

    public void EquipItem(int index)
    {
        if (index < items.Count && index != equippedItemIndex)
        {
            if (items.ElementAt(index) != null)
            {
                UnequipItem();
                equippedItemIndex = index;
                equippedItem = items.ElementAt(index);
                items[index].OnEquip();
                inventoryUIManager.EquipSlot(index);
            }
        }
    }
    public void ThrowItem(int index)
    {
        if (equippedItemIndex >= 0)
        {
            if (index < items.Count)
            {
                if (items.ElementAt(index) != null)
                {
                    int ammo = 0;
                    bool isFirearm = items[index] is Firearm;
                    if (isFirearm) { ammo = ((Firearm)items[index]).currentClipAmount; }
                    if (index == equippedItemIndex) { UnequipItem(); }
                    GameObject thrownItem = Instantiate(items[index].thrownItem, Cache.vcam.transform.position, items[index].transform.rotation);
                    if (isFirearm) { thrownItem.GetComponent<AcquireableFirearm>().ammo = ammo; }
                    Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
                    rb.AddTorque(Utilities.RandomRangeVector3(Utilities.V3All(-throwTorqueForce), Utilities.V3All(throwTorqueForce)));
                    float force = throwForce * Mathf.Clamp(FragMovementListener.hSpeedPercentage > 1f ? ((FragMovementListener.hSpeedPercentage - 1f) / 3.33f) + 1f : FragMovementListener.hSpeedPercentage, 0.75f, 99f);
                    rb.AddForce(Cache.vcam.transform.forward * force);
                    Destroy(items[index].gameObject);
                    items.RemoveAt(index);
                    inventoryUIManager.RemoveSlot(index);
                }
            }
        }
    }
    public void ClearHeldItem()
    {
        ClearItem(equippedItemIndex);
    }
    public void ClearItem(int index)
    {
        if (equippedItemIndex >= 0)
        {
            if (index < items.Count)
            {
                if (items.ElementAt(index) != null)
                {
                    if (index == equippedItemIndex) { UnequipItem(); }
                    Destroy(items[index].gameObject);
                    items.RemoveAt(index);
                    inventoryUIManager.RemoveSlot(index);
                }
            }
        }   
    }
    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            items[equippedItemIndex].OnUnequip();
            equippedItem = null;
            equippedItemIndex = -1;
            inventoryUIManager.UnequipSlot();
            Cache.walkSpeedManager.UpdateValue("EquippedItem", 1f);
        }
    }
    private void Update()
    {
        if (!Death.dead)
        {
            /*
            for (int i = 0; i <= inventorySize; i++)
            {
                KeyCode keyCode = KeyCode.Alpha1 + (i - 1);

                if (Input.GetKeyDown(keyCode))
                {
                    EquipItem(Mathf.Clamp(i - 1, 0, 99));
                }
            }
            */
            int targetSlotIndex = equippedItemIndex;
            if (InputManager.GetInventorySlotOneKeyDown()) { targetSlotIndex = 0; }
            else if (InputManager.GetInventorySlotTwoKeyDown()) { targetSlotIndex = 1; }
            else if (InputManager.GetInventorySlotThreeKeyDown()) { targetSlotIndex = 2; }
            else if (InputManager.GetInventorySlotFourKeyDown()) { targetSlotIndex = 3; }
            else if (InputManager.GetInventorySlotFiveKeyDown()) { targetSlotIndex = 4; }
            else if (InputManager.GetInventorySlotSixKeyDown()) { targetSlotIndex = 5; }
            else if (InputManager.GetInventorySlotSevenKeyDown()) { targetSlotIndex = 6; }
            else if (InputManager.GetInventorySlotEightKeyDown()) { targetSlotIndex = 7; }
            else if (InputManager.GetInventorySlotNineKeyDown()) { targetSlotIndex = 8; }
            if (items.Count > 0&&InputManager.Active())
            {
                if (Input.mouseScrollDelta.y > 0) { targetSlotIndex++; if (targetSlotIndex > items.Count-1) { targetSlotIndex = 0; } }
                if (Input.mouseScrollDelta.y < 0) { targetSlotIndex--; if (targetSlotIndex < 0) { targetSlotIndex = items.Count-1; } }
            }
            if (targetSlotIndex <= items.Count) { EquipItem(targetSlotIndex); }
        }

        if (InputManager.GetUnequipKeyDown()) { UnequipItem(); }
        if (InputManager.GetThrowItemKeyDown()) { ThrowItem(equippedItemIndex); }
    }
}
