using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotationAxis : MonoBehaviour
{
    public Vector3 constrains;
    private void Update()
    {
        if (constrains.x != 0) { transform.eulerAngles = new Vector3(constrains.x, transform.eulerAngles.y, transform.eulerAngles.z); }
        if (constrains.y != 0) { transform.eulerAngles = new Vector3(transform.eulerAngles.x, constrains.y, transform.eulerAngles.z); }
        if (constrains.z != 0) { transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, constrains.z); }
    }
}
