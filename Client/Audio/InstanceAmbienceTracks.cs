using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceAmbienceTracks : MonoBehaviour
{
    public List<string> KEY_ambienceTracks = new List<string>();
    public List<AmbienceTrack> VALUE_ambienceTracks = new List<AmbienceTrack>();
    private void Awake()
    {
        Cache.instanceAmbienceTracks = this;
    }
    private void Start()
    {
        for (int i = 0; i < KEY_ambienceTracks.Count; i++)
        {
            Cache.ambienceTrackManager.AddAmbienceTrack(KEY_ambienceTracks[i], VALUE_ambienceTracks[i]);
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < KEY_ambienceTracks.Count; i++)
        {
            Cache.ambienceTrackManager.RemoveAmbienceTrack(KEY_ambienceTracks[i]);
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < KEY_ambienceTracks.Count; i++)
        {
            Cache.ambienceTrackManager.RemoveAmbienceTrack(KEY_ambienceTracks[i]);
        }
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            for (int i = 0; i < KEY_ambienceTracks.Count; i++)
            {
                Cache.ambienceTrackManager.AddAmbienceTrack(KEY_ambienceTracks[i], VALUE_ambienceTracks[i]);
            }
        }
    }
    */
}

