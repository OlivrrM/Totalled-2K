using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxOnCollide : MonoBehaviour
{
    public GameObject fx;
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(fx, collision.contacts[0].point, Quaternion.identity);
    }
}
