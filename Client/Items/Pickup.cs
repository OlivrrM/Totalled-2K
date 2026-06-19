using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
    public string description;
    public float pickupGraphicSpeed;

    public Collider col;

    [HideInInspector] public bool pickedUp;

    public UnityEvent eventTriggerOnPickup;
    public virtual void Start()
    {
        col = gameObject.GetComponent<Collider>();
    }
    public virtual void Take()
    {
        pickedUp = true;
        col.enabled = false;
        if (eventTriggerOnPickup != null) { eventTriggerOnPickup.Invoke(); }
    }
    public virtual void CancelPickup()
    {
        pickedUp = false;
        col.enabled = true;
    }
    private void Update()
    {
        if (pickedUp)
        {
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position, Time.deltaTime * pickupGraphicSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * pickupGraphicSpeed);
            if (transform.localScale.x < 0.01f)
            {
                Destroy(gameObject);
            }
        }
    }
}
