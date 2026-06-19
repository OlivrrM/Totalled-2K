using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ResetSequenceOnRespawn : MonoBehaviour // This shit code is the result of not keeping any form of reference to currently loaded entities. Infact, there isnt even an entity base class. Wtf? No planning
{
    public List<SequenceObject> objectClones = new List<SequenceObject>();
    public List<SequenceObject> currentObjects = new List<SequenceObject>();
    short uuidIndex;

    public int savedAmmoAmount;
    public int savedGrenadesAmount;

    public delegate void OnSequenceReset();
    public static event OnSequenceReset onSequenceReset;

    public List<Vector3> forgottenObjectPositions = new List<Vector3>();

    //public List<SequenceObject> clonesToForget = new List<SequenceObject>();
    //public List<SequenceObject> currentToForget = new List<SequenceObject>();
    private void Awake()
    {
        Cache.resetSequenceOnRespawn = this;
    }
    public void RememberGameObject(GameObject target)
    {
        var sequence = new SequenceObject(target, uuidIndex,target.GetComponent<ResetObjectOnRespawn>());

        var backup = Instantiate(target);
        backup.SetActive(false);

        var reg = backup.GetComponent<ResetObjectOnRespawn>().backup = true;

        backup.transform.SetParent(sequence.originalParent, false);
        backup.transform.localPosition = sequence.localPosition;
        backup.transform.localRotation = sequence.localRotation;
        backup.transform.localScale = sequence.localScale;

        objectClones.Add(new SequenceObject(backup, uuidIndex, backup.GetComponent<ResetObjectOnRespawn>()));
        currentObjects.Add(sequence);

        uuidIndex = (short)((uuidIndex + 1) & 0x7FFF);
    }
    public void ForgetGameObject(int i)
    {
        forgottenObjectPositions.Add(objectClones[i].localPosition);
        objectClones.RemoveAt(i);
        currentObjects.RemoveAt(i);
    }
    public void ResetSequence()
    {
        for (int i = currentObjects.Count - 1; i >= 0; i--)
        {
            if (currentObjects[i].obj == null){
                ResetObject(i);
            }
            else{
                //print(currentObjects[i].obj.name + "     " + currentObjects[i].resetObjectOnRespawn.flagForReset);
                if (currentObjects[i].resetObjectOnRespawn.flagForReset){
                    Destroy(currentObjects[i].obj);
                    ResetObject(i);
                }
                else
                {
                    //print(currentObjects[i].obj.name);
                    GameObject resetObj = currentObjects[i].obj;
                    SequenceObject backup = objectClones[i];
                    resetObj.transform.localPosition = backup.localPosition;
                    resetObj.transform.localRotation = backup.localRotation;
                    resetObj.transform.localScale = backup.localScale;
                }
            }
        }
        if (Cache.ammo.amount < savedAmmoAmount) { Cache.ammo.SetAmmo(savedAmmoAmount); }
        if (Cache.grenadeManager.grenades < savedGrenadesAmount) { Cache.grenadeManager.SetGrenades(savedGrenadesAmount); }
        for (int i = 0; i < Cache.inventory.items.Count; i++){
            if (Cache.inventory.items[i] is Firearm){
                Firearm firearm = (Firearm)Cache.inventory.items[i];
                firearm.currentClipAmount = firearm.ammoClipSize;
            }
        }
        onSequenceReset?.Invoke();
    }
    public void SaveSequence()
    {
        for (int i = 0; i < currentObjects.Count; i++)
        {
            if (currentObjects[i].obj == null){
                ForgetGameObject(i);
            }
            else{
                if (currentObjects[i].resetObjectOnRespawn.flagForReset){
                    ForgetGameObject(i);
                }
            }
        }
        savedAmmoAmount = Cache.ammo.amount;
        savedGrenadesAmount = Cache.grenadeManager.grenades;
    }
    void ResetObject(int i)
    {
        var backup = objectClones[i];

        if (backup.obj == null){ // This gon break the loop
            currentObjects.RemoveAt(i);
            objectClones.RemoveAt(i);
            return;
        }

        GameObject resetObj = Instantiate(backup.obj);
        resetObj.SetActive(true);

        // Prevent re-registering
        //var reg = resetObj.GetComponent<ResetObjectOnRespawn>().backup = true;
        //if (reg != null) Destroy(reg);

        resetObj.transform.SetParent(backup.originalParent, false);
        resetObj.transform.localPosition = backup.localPosition;
        resetObj.transform.localRotation = backup.localRotation;
        resetObj.transform.localScale = backup.localScale;

        currentObjects[i] = new SequenceObject(resetObj, currentObjects[i].uuid,resetObj.GetComponent<ResetObjectOnRespawn>());

        objectClones[i].resetObjectOnRespawn.flagForReset = false;
    }
}
