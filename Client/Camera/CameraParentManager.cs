using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParentManager : MonoBehaviour
{
    private void Update()
    {
        Vector3 childTempPos = transform.GetChild(0).position;
        transform.position = transform.GetChild(0).position;
        transform.GetChild(0).position = childTempPos;
    }
}
