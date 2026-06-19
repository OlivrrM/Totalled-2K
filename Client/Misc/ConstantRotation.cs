using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public Vector3 rotation;
    private void Update()
    {
        transform.Rotate(rotation*Time.deltaTime);
    }
}
