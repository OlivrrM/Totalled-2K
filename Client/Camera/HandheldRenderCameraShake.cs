using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HandheldRenderCameraShake : MonoBehaviour
{
    public Transform cam;
    public float shakeResistance;
    Vector3 camPos;
    public float lerpSmoothness;
    private void FixedUpdate()
    {
        camPos = cam.localPosition;
    }
    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, camPos * shakeResistance,Time.deltaTime* lerpSmoothness);
    }
}
