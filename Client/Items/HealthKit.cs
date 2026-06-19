using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : Pickup
{
    public float health;
    public PlaySound sfxOnPickup;
    public override void Take()
    {
        base.Take();
        HealPlayer healPlayer = new HealPlayer();
        float healAmount = Mathf.InverseLerp(0f, Cache.health.maxHealth, health);
        healPlayer.amount = health;
        healPlayer.postFxAmount = healAmount;
        healPlayer.postFxDisableSmoothness = 2f;
        healPlayer.postFxTime = healAmount;
        healPlayer.type = Totalled.HealType.Consumable;
        Cache.health.Heal(healPlayer);
        sfxOnPickup.Play();
    }
}
