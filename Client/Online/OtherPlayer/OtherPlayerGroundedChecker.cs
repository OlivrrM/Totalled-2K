using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerGroundedChecker : MonoBehaviour
{
    public bool grounded;

    Collider collider;
    float triggerExitTimer;
    [HideInInspector] public GameObject objectTouching;

    GameObject triggeredObject;
    bool triggered;
    private void Start()
    {
        collider = gameObject.GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 3 && other.gameObject.layer != 7 && other.gameObject.layer != 8 && other.gameObject.layer != 10 && other.gameObject.layer != 13)
        {
            objectTouching = other.gameObject;
            grounded = true;
            triggered = true;
            triggeredObject = other.gameObject;
            triggerExitTimer = 600;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 3 && other.gameObject.layer != 7 && other.gameObject.layer != 8 && other.gameObject.layer != 10 && other.gameObject.layer != 13)
        {
            triggerExitTimer = 0;
            collider.enabled = false;
            collider.enabled = true;
        }
    }
    private void Update()
    {
        if (triggeredObject == null && triggered)
        {
            triggerExitTimer = 0;
            collider.enabled = false;
            collider.enabled = true;
            triggered = false;
        }
        if (triggerExitTimer < 0.1f)
        {
            triggerExitTimer += Time.deltaTime;
        }
        if (triggerExitTimer >= 0.1f && triggerExitTimer < 500)
        {
            grounded = false;
            objectTouching = null;
            triggerExitTimer = 600;
        }
    }
}
