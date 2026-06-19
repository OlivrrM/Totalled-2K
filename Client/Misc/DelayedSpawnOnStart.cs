using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawnOnStart : MonoBehaviour
{
    public float delay;
    public GameObject target;
    private void Start()
    {
        target.SetActive(false);
        StartCoroutine(Spawn());
    }
    IEnumerator Spawn() 
    {
        yield return new WaitForSeconds(delay);
        target.SetActive(true);
    }
}
