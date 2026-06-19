using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunOnCollision : MonoBehaviour
{
    public float amount;
    public float imobileTime;
    public float regainSpeedTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            IStunnable stunnable = other.gameObject.GetComponent<IStunnable>();
            if (stunnable!=null){
                stunnable.Stun(amount, imobileTime, regainSpeedTime);
            }
        }
    }
}
