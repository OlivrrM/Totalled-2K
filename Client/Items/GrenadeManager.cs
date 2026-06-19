using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrenadeManager : MonoBehaviour
{
    public GameObject grenade;
    public float throwForce;
    [HideInInspector] public float currentThrowForce;

    public int maxGrenades;
    public int grenades;

    public CounterGraphicManager counterGraphicManager;
    private void Start()
    {
        Cache.grenadeManager = this;
        counterGraphicManager.SetCounter(grenades);
    }
    public void PickupGrenade(int amount = 1, bool ignoreLimit = false)
    {
        if ((!ignoreLimit && CanPickup())||ignoreLimit)
        {
            grenades += amount;
            counterGraphicManager.SetCounter(grenades);
        }
        else
        {
            CancelPickup();
            //Deny grenade pickup code
        }
    }
    public void SetGrenades(int amount)
    {
        grenades = amount;
        counterGraphicManager.SetCounter(amount);
    }
    public bool CanPickup()
    {
        return grenades < maxGrenades;
    }
    public void CancelPickup()
    {
        counterGraphicManager.DenyCounter();
    }
    public void ThrowGrenade()
    {
        currentThrowForce = throwForce * Mathf.Clamp(FragMovementListener.hSpeedPercentage > 1f ? ((FragMovementListener.hSpeedPercentage-1f)/2.5f)+1f : FragMovementListener.hSpeedPercentage, 0.75f, 99f);
        Instantiate(grenade, Camera.main.transform.position, Camera.main.transform.rotation);
        grenades--;
        counterGraphicManager.SetCounter(grenades);
        counterGraphicManager.BobCounter(0.75f, 0.2f);
    }
    private void Update()
    {
        if (InputManager.GetGrenadeKeyDown())
        {
            if (grenades > 0)
            {
                ThrowGrenade();
            }
            else
            {
                counterGraphicManager.DenyCounter();
            }
        }
    }
}
