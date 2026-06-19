using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    public float damagePerTick;
    public float tps;

    public bool triggerPlayerOnly;
    private void OnTriggerEnter(Collider other)
    {
        if ((triggerPlayerOnly&&other.gameObject==Cache.surfCharacter.gameObject)||!triggerPlayerOnly)
        {
            Kindle kindle = new Kindle
            {
                tickDamage = damagePerTick,
                ticksPerSecond = tps,
                litTime = 99999f
            };
            IFlammable flammable = other.gameObject.GetComponent<IFlammable>();
            if (flammable != null)
            {
                flammable.Light(kindle);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((triggerPlayerOnly && other.gameObject == Cache.surfCharacter.gameObject) || !triggerPlayerOnly)
        {
            Kindle kindle = new Kindle
            {
                tickDamage = damagePerTick / 2f,
                ticksPerSecond = tps / 2f,
                litTime = 1.5f
            };
            IFlammable flammable = other.gameObject.GetComponent<IFlammable>();
            if (flammable != null)
            {
                flammable.Light(kindle);
            }
        }
    }
}
