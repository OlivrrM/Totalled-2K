using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTargetInTargetDirection : MonoBehaviour
{
    public Transform direction;
    public Rigidbody target;
    public float force;
    bool inside;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target){
            inside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == target){
            inside = false;
        }
    }
    private void FixedUpdate()
    {
        if (inside){
            print("pushed" + Random.RandomRange(0, 999999));
            target.AddForce(direction.forward * force);
        }
    }
}
