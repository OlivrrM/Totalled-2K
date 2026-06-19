using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnDamage : MonoBehaviour, IDamageable
{
    public UnityEvent unityEvent;
    public bool uniqueInvoke;
    bool invoked;
    public void Damage(Damage damage)
    {
        if ((uniqueInvoke && !invoked) || !uniqueInvoke){
            unityEvent.Invoke();
            invoked = true;
        }
    }
}
