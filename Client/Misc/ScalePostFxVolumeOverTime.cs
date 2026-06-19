using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScalePostFxVolumeOverTime : MonoBehaviour
{
    public Vector3 target;
    public float speed;

    public Volume fxVolume;
    //public PostProcessVolume fxVolume;
    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
        fxVolume.blendDistance = Mathf.Lerp(fxVolume.blendDistance, target.magnitude, Time.deltaTime * speed);
    }
}
