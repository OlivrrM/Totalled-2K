using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SequenceObject
{
    public GameObject obj;
    public ResetObjectOnRespawn resetObjectOnRespawn; // For clones
    public short uuid;

    public Transform originalParent;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;

    public SequenceObject(GameObject obj, short uuid, ResetObjectOnRespawn resetObjectOnRespawn = null)
    {
        this.obj = obj;
        this.uuid = uuid;
        this.resetObjectOnRespawn = resetObjectOnRespawn;

        if (obj != null)
        {
            var t = obj.transform;
            originalParent = t.parent;
            localPosition = t.localPosition;
            localRotation = t.localRotation;
            localScale = t.localScale;
        }
    }
}
