using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceIndoorsToggler : MonoBehaviour
{
    public void Enable()
    {
        Cache.ambienceManager.EnableChunkParticleSystems();
    }
    public void Disable()
    {
        Cache.ambienceManager.DisableChunkParticleSystems();
    }
}
