using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedGravity : MonoBehaviour
{
    public Rigidbody rb;
    public float gravity;
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * (gravity * 4), ForceMode.Force);
    }
}
