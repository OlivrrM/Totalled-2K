using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableDoodad : MonoBehaviour, IDamageable
{
    public float health;
    public KillableDoodad killableDoodad;
    public virtual void Damage(Damage damage)
    {
        health -= damage.amount;
        if (health <= 0) { killableDoodad.Die(); }
    }
}
