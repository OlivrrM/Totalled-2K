using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AmbienceTrackTrigger : MonoBehaviour // UNUSED
{
    public string enterTrackId;
    public AmbienceTrack ambienceTrackEnter;

    public string exitTrackId;
    public AmbienceTrack ambienceTrackExit;
    private void Start()
    {
        Cache.ambienceTrackManager.AddAmbienceTrack(enterTrackId, ambienceTrackEnter);
        Cache.ambienceTrackManager.AddAmbienceTrack(exitTrackId, ambienceTrackEnter);
    }
    public void SetEnterTrackCurrentTrack()
    {
        Cache.ambienceTrackManager.SetCurrentTrack(enterTrackId);
    }
    public void SetExitTrackCurrentTrack()
    {
        Cache.ambienceTrackManager.SetCurrentTrack(exitTrackId);
    }
}
