using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : ValueMultiplierManager
{
    Vector3 defaultSize;
    private void Start()
    {
        defaultSize = transform.localScale;
    }
    private void Update()
    {
        transform.localScale = defaultSize;
        foreach (KeyValuePair<string, object> value in values){
            transform.localScale = Vector3.Scale((Vector3)value.Value, transform.localScale);
        }
    }
}
