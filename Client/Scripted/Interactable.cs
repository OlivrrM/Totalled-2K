using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public bool interactable = true;
    public string description;
    public float interactableRange;

    public bool showBindKey = true;
    public string uninteractableDescription;

    public UnityEvent eventTriggerOnPickup;
    public virtual void Interact() {
        if (!interactable) { return; }
        if (eventTriggerOnPickup != null) { eventTriggerOnPickup.Invoke(); }
    }
    public virtual void HeldInteract() { }
}
