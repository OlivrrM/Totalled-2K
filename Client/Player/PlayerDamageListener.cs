using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageListener : MonoBehaviour, IDamageable
{
    public Health health;
    public void Damage(Damage damage)
    {
        health.Damage(damage);
    }
}
