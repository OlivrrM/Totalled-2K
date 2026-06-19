using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlashSmoke : MonoBehaviour
{
    public float speed;
    private void Update()
    {
        if (Firearm.curentMuzzleFlashPos != null) { transform.position = Vector3.Lerp(transform.position, Firearm.curentMuzzleFlashPos.position, Time.deltaTime * speed); }
    }
}
