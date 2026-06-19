using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfBob : MonoBehaviour
{
    public float bobIncrease;
    Vector3 targetOffset;
    private void Start()
    {
        Cache.cameraWorldSpaceRotationOffsetManager.AddOffset("SurfBob", Vector3.zero);
    }
    /*
    private void Update()
    {
        if (Cache.moveData.Surfing) { targetOffset = Cache.moveData.SurfNormal; }
        else { targetOffset = Vector3.zero; }
        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("SurfBob", targetOffset * bobIncrease);
        print(Cache.moveData.surf+Random.RandomRange(1,100000).ToString());
    }
    */
}
