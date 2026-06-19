using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandheldsCamSway : MonoBehaviour
{
    Quaternion currentRotation;
    public float swaySpeed;
    private void Update()
    {
        currentRotation = Quaternion.Lerp(currentRotation, transform.parent.rotation, Time.deltaTime * swaySpeed);
        transform.rotation = currentRotation;
    }
}
