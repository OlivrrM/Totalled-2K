using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public float fallSpeed;
    float currentFall;

    float currentSpeed;

    Vector2 currentMouseSensMultiplier;

    public static bool dead;

    public PostFxVolumeManager deathFx;
    public PostFxVolumeManager blankDeathFx;

    public Inventory inventory;

    public FlammablePlayer flammablePlayer;
    private void Start()
    {
        dead = false;
        Cache.camViewOffsetManager.AddValue("Death", Vector3.one);
        Cache.walkSpeedManager.AddValue("Death", 1f);
        Cache.jumpHeightManagerScript.AddValue("Death", 1f);
        Cache.mouseSensitivityManager.AddValue("Death", Vector2.one);
        currentMouseSensMultiplier = Vector2.one;
    }
    public void Die()
    {
        dead = true;
        deathFx.Enable(3);
        inventory.UnequipItem();
    }
    public void Respawn()
    {
        dead = false;
        deathFx.Disable(99f);
        blankDeathFx.Disable(3f);
        flammablePlayer.Extinguish();
        if (Cache.resetSequenceOnRespawn != null) { Cache.resetSequenceOnRespawn.ResetSequence(); }
        if (Cache.setLoadedInstancesOnRespawn != null) { Cache.setLoadedInstancesOnRespawn.LoadInstances(); }
    }
    private void Update()
    {
        if (dead)
        {
            currentFall = Mathf.Lerp(currentFall, -1.45f, Time.deltaTime * fallSpeed);
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 10);
            Cache.jumpHeightManagerScript.UpdateValue("Death", 0f);
            currentMouseSensMultiplier = Vector2.Lerp(currentMouseSensMultiplier, Vector2.zero, Time.deltaTime * 3f);
        }
        else
        {
            currentFall = 0;
            currentSpeed = 1f;
            currentMouseSensMultiplier = Vector2.one;
            Cache.jumpHeightManagerScript.UpdateValue("Death", 1f);
        }
        Cache.camViewOffsetManager.UpdateValue("Death", new Vector3(0, currentFall, 0));
        Cache.walkSpeedManager.UpdateValue("Death", currentSpeed);
        Cache.mouseSensitivityManager.UpdateValue("Death", currentMouseSensMultiplier);
    }
}
