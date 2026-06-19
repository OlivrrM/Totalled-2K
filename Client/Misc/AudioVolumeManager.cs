using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeManager : ValueMultiplierManager
{
    float volume;
    private void Start()
    {
        Cache.audioVolumeManager = this;
        volume = 1f;
    }
    private void Update()
    {
        volume = 1f;
        foreach (KeyValuePair<string, object> value in values)
        {
            volume *= System.Convert.ToSingle(value.Value);
        }
        AudioListener.volume = volume;
    }
}
