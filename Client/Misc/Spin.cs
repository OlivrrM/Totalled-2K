using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 spinSpeed;
    public bool unscaledDeltaTime;
    private void Update()
    {
        transform.Rotate(spinSpeed * (unscaledDeltaTime?Time.unscaledDeltaTime:Time.deltaTime));
        
    }
}
