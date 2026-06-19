using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCopyCamFov : MonoBehaviour
{
    Camera camera;
    private void Start()
    {
        camera = gameObject.GetComponent<Camera>();
    }
    private void Update()
    {
        camera.fieldOfView = Camera.main.fieldOfView;
    }
}
