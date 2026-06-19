using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fragsurf.Movement;

public class CameraWorldSpaceRotationOffsetManager : MonoBehaviour
{
    Dictionary<string, Vector3> offsets = new Dictionary<string, Vector3>();

    Transform cam;
    private void Start()
    {
        Cache.cameraWorldSpaceRotationOffsetManager = this;
        Cache.mainCam = GameObject.Find("Cam").transform;
        cam = Cache.mainCam.parent;
    }
    public void AddOffset(string name, Vector3 offset)
    {
        if (!offsets.ContainsKey(name)){
            offsets.Add(name, offset);
        }
        else { Debug.LogError("Attempted to add new camera world space rotation offset with name '" + name + "', however offset already existed."); }
    }
    public void UpdateOffset(string name, Vector3 offset)
    {
        offsets[name] = offset;
    }
    public void ResetOffset(string offsetName)
    {
        foreach (KeyValuePair<string, Vector3> offset in offsets){
            if (offset.Key == offsetName){
                offsets[offsetName] = Vector3.zero;
                break;
            }
        }
    }
    private void Update()
    {
        cam.localEulerAngles = Vector3.zero;
        foreach (KeyValuePair<string,Vector3> offset in offsets){
            cam.localEulerAngles += offset.Value;
            //cam.localEulerAngles += new Vector3(offset.Value.x,0, offset.Value.z);
        }
    }
}
