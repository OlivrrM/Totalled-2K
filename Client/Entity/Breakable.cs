using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour, IDamageable
{
    public float health;
    bool dead;
    public Rigidbody enableRbOnDamage;
    public virtual void Damage(Damage damage)
    {
        health -= damage.amount;
        if (enableRbOnDamage != null) {
            enableRbOnDamage.isKinematic = false;
        }
        if (health <= 0f &&!dead)
        {
            dead = true;
            Break(damage);
        }
    }
    public virtual void Break(Damage damage)
    {
        IDestroy destroy = gameObject.GetComponent<IDestroy>();
        if (destroy != null) { destroy.Destroy(); }
        Destroy(gameObject);
    }
}
