using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingerPlayerCollision : MonoBehaviour
{
    public Swinger swinger;

    Vector3 velocityLastTick;
    private void FixedUpdate()
    {
        velocityLastTick = Cache.rb.velocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (swinger.pulling)
        {
            Cache.rb.velocity = velocityLastTick;
            swinger.OnDetach();
        }
    }
    
}
