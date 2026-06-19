using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSpeedManager : ValueMultiplierManager
{
    float defaultSpeed;
    private void Start()
    {
        defaultSpeed = Cache.moveData.WalkFactor;
        Cache.walkSpeedManager = this;
    }
    private void Update()
    {
        Cache.moveData.WalkFactor = defaultSpeed;
        foreach (KeyValuePair<string, object> value in values)
        {
            Cache.moveData.WalkFactor *= System.Convert.ToSingle(value.Value);
        }
    }
}
