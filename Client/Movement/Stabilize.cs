using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilize : MonoBehaviour
{
    public float stabilizedSpeedMultiplier;
    public bool stabilized;
    private void Start()
    {
        Cache.stabilize = this;
        Cache.walkSpeedManager.AddValue("Stabilized", 1f);
    }
    private void Update()
    {
        if (InputManager.GetStabilizeKey() && FragMovementListener.grounded && !Cache.crouch.crouching){
            Cache.walkSpeedManager.UpdateValue("Stabilized", stabilizedSpeedMultiplier);
            stabilized = true;
        }
        else{
            Cache.walkSpeedManager.UpdateValue("Stabilized", 1f);
            stabilized = false;
        }
    }
}
