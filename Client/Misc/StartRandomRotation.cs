using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRandomRotation : MonoBehaviour
{
    public Vector3 rotationA;
    public Vector3 rotationB;

    public bool local;
    private void Start()
    {
        if (local) { transform.localRotation = Quaternion.Euler(new Vector3(Random.Range(rotationA.x, rotationB.x), Random.Range(rotationA.y, rotationB.y), Random.Range(rotationA.z, rotationB.z))); }
        else { transform.rotation = Quaternion.Euler(new Vector3(Random.Range(rotationA.x, rotationB.x), Random.Range(rotationA.y, rotationB.y), Random.Range(rotationA.z, rotationB.z))); }
    }
}
