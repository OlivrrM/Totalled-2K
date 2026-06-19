using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrackManager : MonoBehaviour
{
    public static bool musicTrackPlaying;
    private void Awake()
    {
        musicTrackPlaying = false;
    }
}
