using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnInboundsClipBrush : MonoBehaviour
{
    Collider col;

    bool inbounds;

    public UnityEvent onEnterboundsEvent;
    public UnityEvent onExitboundsEvent;
    private void Start()
    {
        col = GetComponent<Collider>();
    }
    private void Update()
    {
        if (col.bounds.Intersects(Cache.surfCharacter.Collider.bounds))
        {
            onEnterboundsEvent.Invoke();
            inbounds = true;
        }
        if (inbounds)
        {
            if (!col.bounds.Intersects(Cache.surfCharacter.Collider.bounds))
            {
                onExitboundsEvent.Invoke();
                inbounds = false;
            }
        }
    }
}
