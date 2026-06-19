using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothDisableSfx : MonoBehaviour
{
    public AudioSource sfx;
    bool disabling;
    float currentSmoothness;
    public void Disable(float smoothness)
    {
        disabling = true;
        currentSmoothness = smoothness;
    }
    private void Update()
    {
        if (disabling)
        {
            sfx.volume = Mathf.Lerp(sfx.volume, 0f, currentSmoothness * Time.deltaTime);
        }
    }
}
