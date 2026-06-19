using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonBodyController : MonoBehaviour
{
    public float turnSpeed;

    Vector3 velocityDirection;
    Quaternion targetRotation;
    private void Start()
    {
        SetDirection();
    }
    private void Update()
    {
        if (Cache.moveData.ForwardMove != 0 || Cache.moveData.SideMove != 0){
            if (Cache.moveData.Velocity.magnitude > 0.1f){
                SetDirection();
            }
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    void SetDirection()
    {
        velocityDirection = Cache.moveData.Velocity.normalized;
        velocityDirection.y = 0f;
        targetRotation = Quaternion.LookRotation(velocityDirection, Vector3.up);
    }
}
