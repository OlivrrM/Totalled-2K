using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableLightOnParticle : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public LightIntensityManager lightIntensityManager;
    private void Update()
    {
        if (particleSystem.isPlaying) { lightIntensityManager.SetIntensity(1f, 2f); }
        else { lightIntensityManager.SetIntensity(0f, 2f); }
    }
}
