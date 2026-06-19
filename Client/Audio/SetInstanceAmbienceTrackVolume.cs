using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInstanceAmbienceTrackVolume : MonoBehaviour
{
    public string targetTrackId;
    public void SetTrackVolume(float volume)
    {
        Cache.ambienceTrackManager.SetTrackVolume(targetTrackId, volume);
    }
}
