using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPostFxManager : MonoBehaviour
{
    public PostFxVolumeManager waterVolumeManager;

    public AudioSource ambience;
    float defaultVolume;
    public float ambienceVolumeChangeSpeed;
    bool underWater;
    private void Start()
    {
        waterVolumeManager.Disable(20f);
        defaultVolume = ambience.volume;
        ambience.volume = 0f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            waterVolumeManager.Enable(2f);
            underWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            waterVolumeManager.Disable(5f);
            underWater = false;
        }
    }
    private void Update()
    {
        if (underWater) { ambience.volume = Mathf.Lerp(ambience.volume, defaultVolume, Time.deltaTime * ambienceVolumeChangeSpeed); }
        else { ambience.volume = Mathf.Lerp(ambience.volume, 0f, Time.deltaTime * ambienceVolumeChangeSpeed); }
    }
}
