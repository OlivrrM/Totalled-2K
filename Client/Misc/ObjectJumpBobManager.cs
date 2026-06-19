using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectJumpBobTrigger : MonoBehaviour
{
    public AnimationCurve bounceCurve;
    public float bounceAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            ObjectJumpBob objectJumpBob = other.gameObject.GetComponent<ObjectJumpBob>();
            if (objectJumpBob != null)
            {
                objectJumpBob.Bounce(FragMovementListener.preGroundedVelocity.y, transform.position);
            }
        }
    }
}
