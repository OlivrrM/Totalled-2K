using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderSizeManager : ValueMultiplierManager
{
    Vector3 defaultSize;
    private void Start()
    {
        defaultSize = Cache.surfCharacter.ColliderSize;
        Cache.playerColliderSizeManager = this;
    }
    private void Update()
    {
        Cache.surfCharacter.ColliderSize = defaultSize;
        foreach (KeyValuePair<string, object> value in values)
        {
            Cache.surfCharacter.ColliderSize = Vector3.Scale((Vector3)value.Value,Cache.surfCharacter.ColliderSize);
        }
    }
}
