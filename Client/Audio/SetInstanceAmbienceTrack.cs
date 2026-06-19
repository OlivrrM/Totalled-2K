using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInstanceAmbienceTrack : MonoBehaviour
{
    public void SetTrack(string id)
    {
        Cache.ambienceTrackManager.SetCurrentTrack(id);
    }
}
