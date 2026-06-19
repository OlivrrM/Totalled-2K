using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item")]
    public string identifier; //Should be the offical name of the item
    public Transform equipTransform;
    [HideInInspector] public bool equip;

    public bool holdKeyMain;
    public bool holdKeySecondary;

    public Vector3 unequipPos;
    public float equipSpeed;

    public Vector3 unequipRot;
    public float unequipSpeed;

    float timeAfterEquip;
    float timeAfterEquipSpeedIncrease;

    public PlayRandomSound equipSfx;

    bool hasUsedThisEquip;

    public GameObject inventorySlot;

    public float holdWalkSpeedMultiplier = 1f;

    public float onMainActionCd;
    float currentOnMainActionCd;
    public float onSecondaryActionCd;
    float currentOnSecondaryActionCd;

    public GameObject thrownItem;

    [Header("Inspect")]
    public AnimationClip inspectAnimation;
    [HideInInspector] public bool hasInspect;
    [HideInInspector] public float currentInspectTime;
    public float inspectSpeed;
    private void Awake()
    {
        equipTransform.localPosition = unequipPos;
        equipTransform.localRotation = Quaternion.Euler(unequipRot);
        hasInspect = inspectAnimation!=null;
    }
    public virtual void Start()
    {

    }
    public virtual void Update()
    {
        if (equip)
        {
            if (holdKeyMain) { if (InputManager.GetMainActionKey() && currentOnMainActionCd <= 0f) { OnMainAction(); } }
            else { if (InputManager.GetMainActionKeyDown() && currentOnMainActionCd <= 0f) { OnMainAction(); } }
            if (holdKeySecondary) { if (InputManager.GetSecondaryActionKey() && currentOnSecondaryActionCd <= 0f) { OnSecondaryAction(); } }
            else { if (InputManager.GetSecondaryActionKeyDown() && currentOnSecondaryActionCd <= 0f) { OnSecondaryAction(); } }
        }

        timeAfterEquip += Time.deltaTime;
        if (timeAfterEquip > 1f)
        {
            timeAfterEquipSpeedIncrease += Time.deltaTime * 100;
        }
        else
        {
            timeAfterEquipSpeedIncrease = 1f;
        }

        if (equip)
        {
            equipTransform.localPosition = Vector3.Lerp(equipTransform.localPosition, Vector3.zero, Time.deltaTime * equipSpeed*(hasUsedThisEquip ? 5f : 1f));
            equipTransform.localRotation = Quaternion.Lerp(equipTransform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * equipSpeed* timeAfterEquipSpeedIncrease * (hasUsedThisEquip ? 5f : 1f));
        }
        else
        {
            equipTransform.localPosition = Vector3.Lerp(equipTransform.localPosition, unequipPos, Time.deltaTime * equipSpeed);
            equipTransform.localRotation = Quaternion.Lerp(equipTransform.localRotation, Quaternion.Euler(unequipRot), Time.deltaTime * unequipSpeed* timeAfterEquipSpeedIncrease);
        }

        currentOnMainActionCd -= Time.deltaTime;
        currentOnSecondaryActionCd -= Time.deltaTime;

        if (Cache.inventory.equippedItem == this){
            Cache.walkSpeedManager.UpdateValue("EquippedItem", Cache.crouch.crouching||Cache.stabilize.stabilized?1f:holdWalkSpeedMultiplier);
        }

        if (hasInspect){
            bool canceled = false;
            if (currentInspectTime > 0f){
                currentInspectTime -= Time.deltaTime;
                if (currentInspectTime <= 0f){
                    OnInspectFinish();
                }
                if (InputManager.GetInspectKeyDown()){
                    canceled = true;
                    OnInspectCancel();
                }
            }
            if (InputManager.GetInspectKeyDown()){
                if (currentInspectTime <= 0f && !canceled){
                    OnInspect();
                }
            }
        }
    }
    public virtual void OnInspect()
    {
        currentInspectTime = inspectAnimation.length/inspectSpeed;
    }
    public virtual void OnInspectFinish()
    {

    }
    public virtual void OnInspectCancel()
    {
        currentInspectTime = 0f;
    }
    public virtual void OnMainAction()
    {
        hasUsedThisEquip = true;
        currentOnMainActionCd = onMainActionCd;
    }
    public virtual void OnSecondaryAction()
    {
        currentOnSecondaryActionCd = onSecondaryActionCd;
    }
    public virtual void OnEquip()
    {
        equip = true;
        timeAfterEquip = 0;
        equipSfx.Play(0.9f,1f);
        hasUsedThisEquip = false;
        //Cache.walkSpeedManager.UpdateValue("EquippedItem", holdWalkSpeedMultiplier);
    }
    public virtual void OnUnequip()
    {
        equip = false;
        hasUsedThisEquip = false;
        timeAfterEquip = 0;
        equipSfx.Play(0.75f,0.85f);
    }
}
