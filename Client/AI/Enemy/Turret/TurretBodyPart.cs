using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBodyPart : MonoBehaviour, IDamageable
{
    public TurretHealth turretHealth;
    public float damageMultiplier;
    public bool head;
    public void Damage(Damage damage)
    {
        damage.amount *= damageMultiplier;
        turretHealth.Damage(damage,head);
    }
}
