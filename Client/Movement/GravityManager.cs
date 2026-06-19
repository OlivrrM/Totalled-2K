using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : ValueMultiplierManager
{
    float defaultGravity;
    private void Start()
    {
        Cache.gravityManager = this;
        defaultGravity = Cache.moveData.GravityFactor;
    }
    void Update()
    {
        Cache.moveData.GravityFactor = defaultGravity;
        foreach (KeyValuePair<string,object> value in values){
            Cache.moveData.GravityFactor *= System.Convert.ToSingle(value.Value);
        }
    }
}
