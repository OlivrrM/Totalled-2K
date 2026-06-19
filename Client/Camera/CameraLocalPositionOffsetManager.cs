using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLocalPositionOffsetManager : MonoBehaviour
{
    Dictionary<string, Vector3> offsets = new Dictionary<string, Vector3>();

    CinemachineTransposer vcamTransposer;

    public Transform[] exclude;
    Vector3[] excludedPosesCache;
    private void Start()
    {
        Cache.cameraLocalPositionOffsetManager = this;
        vcamTransposer = Cache.vcam.GetCinemachineComponent<CinemachineTransposer>();
        excludedPosesCache = new Vector3[exclude.Length];
    }
    public void AddOffset(string name, Vector3 offset)
    {
        if (!offsets.ContainsKey(name)){
            offsets.Add(name, offset);
        }
        else { Debug.LogError("Attempted to add new camera local position offset with name '" + name + "', however offset already existed."); } 
    }
    public void UpdateOffset(string name, Vector3 offset)
    {
        offsets[name] = offset;
    }
    private void Update()
    {
        for (int i = 0; i < excludedPosesCache.Length; i++) { excludedPosesCache[i] = exclude[i].position; }
        vcamTransposer.m_FollowOffset = new Vector3(0f, 0f, -0.01f);
        foreach (KeyValuePair<string,Vector3> offset in offsets){vcamTransposer.m_FollowOffset += offset.Value;}
        for (int i = 0; i < excludedPosesCache.Length; i++) { exclude[i].position = excludedPosesCache[i]; }
    }
}
