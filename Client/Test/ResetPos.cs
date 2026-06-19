using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPos : MonoBehaviour
{
    public Transform player;
    public Vector3 pos;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            player.position = pos;
        }
    }
}
