using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsColliding : MonoBehaviour
{
    [HideInInspector] public bool colliding;

    private void Start()
    {
        colliding = true;
    }
}
