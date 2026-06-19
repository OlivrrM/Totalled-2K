using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBelowOnStart : MonoBehaviour
{
    public float distance;
    public GameObject spawn;
    public bool spawnWithNormalDirection;
    public LayerMask layerMask;
    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,Vector3.down,out hit,distance,layerMask)){
            Instantiate(spawn, hit.point, spawnWithNormalDirection ? Quaternion.LookRotation(hit.normal) : Quaternion.identity);
        }
    }
}
