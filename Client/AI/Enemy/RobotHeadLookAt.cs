using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHeadLookAt : MonoBehaviour
{
    public Transform target;

    public bool lerpRotation;

    public Transform head;
    public float smoothness;
    public Vector3 rotationOffset;
    private void Update()
    {
        Quaternion targetDirection = Utilities.GetRotationTowards(head.position, target.position);
        if (lerpRotation) { head.rotation = Quaternion.Lerp(head.rotation, targetDirection * Quaternion.Euler(rotationOffset), Time.deltaTime * smoothness); }
        else { head.rotation = Quaternion.RotateTowards(head.rotation, targetDirection * Quaternion.Euler(rotationOffset), Time.deltaTime * smoothness); }
    }
}
