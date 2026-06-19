using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerClipBrush : ClipBrush
{
    public UnityEvent eventToTrigger;
    public bool disableOnTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Cache.surfCharacter.gameObject) {
            eventToTrigger.Invoke();
            if (disableOnTrigger) { gameObject.SetActive(false); }
        }
    }
}
