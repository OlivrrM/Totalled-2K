using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationVelocity : MonoBehaviour
{
    float velocity;
    public float drag;
    public void AddForce(float force)
    {
        velocity += force;
    }
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, velocity * Time.deltaTime));
        velocity = Mathf.Lerp(velocity, 0f, Time.deltaTime * drag);
    }
}
