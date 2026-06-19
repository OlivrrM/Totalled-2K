using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamViewOffsetManager : ValueMultiplierManager
{
    Vector3 defaultOffset;
    private void Start()
    {
        defaultOffset = Cache.surfCharacter.ViewOffset;
        Cache.camViewOffsetManager = this;
    }
    private void Update()
    {
        Cache.surfCharacter.ViewOffset = defaultOffset;
        foreach (KeyValuePair<string, object> value in values)
        {
            Cache.surfCharacter.ViewOffset += (Vector3)value.Value;
        }
    }
}
