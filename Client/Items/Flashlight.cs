using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light light;
    bool on;
    public float onIntensity;
    public PlaySound clickSfx;
    private void Update()
    {
        if (InputManager.GetFlashlightKeyDown()){
            on = !on;
            light.intensity = on ? onIntensity : 0f;
            clickSfx.Play(true, on ? 1f : 0.8f);
        }   
    }
}
