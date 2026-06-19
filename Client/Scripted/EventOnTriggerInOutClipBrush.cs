using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnTriggerInOutClipBrush : MonoBehaviour // This has odd behaviour when using both enter & exit together
{
    public UnityEvent eventToTriggerIn;
    public UnityEvent eventToTriggerOut;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Cache.surfCharacter.gameObject){
            eventToTriggerIn.Invoke();
            print(0);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Cache.surfCharacter.gameObject){
            eventToTriggerOut.Invoke();
            print(1);
        }
    }
}
