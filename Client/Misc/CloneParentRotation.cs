using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneParentRotation : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = transform.parent.rotation;
    }
}
