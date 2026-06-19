using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadingSpeedManager : MonoBehaviour
{
    public float reloadingSpeedMultiplier;
    private void Start(){
        Cache.walkSpeedManager.AddValue("ReloadingSpeed", 1f);
    }
    private void Update(){
        Cache.walkSpeedManager.UpdateValue("ReloadingSpeed",Firearm.globalCurrentlyReloading&&!Cache.crouch.crouching?reloadingSpeedMultiplier:1f);
    }
}
