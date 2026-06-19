using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public RectTransform inventoryUI;
    int itemSlots;
    int equippedItemSlot;
    [HideInInspector] public List<InventoryUISlot> slots = new List<InventoryUISlot>();

    public float slotSpacing;
    public float equippedSlotSpacing;
    public float slotSpacingSmoothness;

    [HideInInspector] public float timeSinceLastEquip;
    private void Start()
    {
        Cache.inventoryUIManager = this;
    }
    public void AddSlot(GameObject slot)
    {
        itemSlots++;
        GameObject newSlot = Instantiate(slot, inventoryUI.position, Quaternion.identity, inventoryUI);
        InventoryUISlot inventoryUISlot = newSlot.GetComponent<InventoryUISlot>();
        inventoryUISlot.numberText.text = itemSlots.ToString();
        slots.Add(inventoryUISlot);
    }
    public void RemoveSlot(int slotIndex)
    {
        Destroy(slots[slotIndex].gameObject);
        for (int i = 0; i < slots.Count; i++){
            if (i > slotIndex) { slots[i].numberText.text = (int.Parse(slots[i].numberText.text) - 1).ToString(); }
        }
        slots.RemoveAt(slotIndex);
        itemSlots--;
    }
    public void EquipSlot(int index)
    {
        if (equippedItemSlot >= 0) { slots[equippedItemSlot].Disable(); }
        equippedItemSlot = index;
        slots[index].Enable();
        timeSinceLastEquip = 0f;
    }
    public void UnequipSlot()
    {
        slots[equippedItemSlot].Disable();
        equippedItemSlot = -1;
    }
    private void Update()
    {
        timeSinceLastEquip += Time.unscaledDeltaTime;
        bool hide = timeSinceLastEquip > 3f;
        inventoryUI.anchoredPosition = Vector2.Lerp(inventoryUI.anchoredPosition, new Vector2(inventoryUI.anchoredPosition.x, hide ? 110f : 0f), Time.unscaledDeltaTime * (hide ? 5f : 10f));

        if (itemSlots % 2 == 0)
        {
            int half = slots.Count / 2;
            float first = (slotSpacing / 2) * (itemSlots == 1 ? 0 : 1);
            for (int i = 0; i < half; i++){
                slots[i].targetX = (-slotSpacing * (half - i))+first;
            }
            for (int i = 0; i < slots.Count - half; i++){
                slots[half+i].targetX = (slotSpacing * i)+first;
            }
        }
        else
        {
            int half = (slots.Count-1) / 2;
            for (int i = 0; i < half; i++){
                slots[i].targetX = -slotSpacing * (half - i);
            }
            for (int i = 0; i < (slots.Count - half); i++){
                slots[half + i].targetX = (slotSpacing * i)*(i==0?0f:1f);
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            float equippedSpacing = (((i < equippedItemSlot)&& (equippedItemSlot!=-1)) ? -equippedSlotSpacing : 0f) + (((i > equippedItemSlot) && (equippedItemSlot != -1)) ? equippedSlotSpacing : 0f);
            slots[i].rectTransform.anchoredPosition = Vector2.Lerp(slots[i].rectTransform.anchoredPosition, new Vector2(slots[i].targetX+equippedSpacing, slots[i].rectTransform.anchoredPosition.y), Time.unscaledDeltaTime * slotSpacingSmoothness);
        }
    }
}
