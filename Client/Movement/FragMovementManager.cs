using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragMovementManager : MonoBehaviour
{
    public float rbToFragVelFactor;
    public float fragVelToRbFactor;

    public PhysicMaterial physicMaterial;
    IEnumerator Start()
    {
        Cache.fragMovementManager = this;
        yield return new WaitForEndOfFrame();
        Cache.rb = GetComponent<Rigidbody>();
        Cache.col = GetComponent<Collider>();
        Cache.col.material = physicMaterial;
    }
    public static void SetActive(bool active)
    {
        if (active) { Cache.moveData.Origin = Cache.moveData.PreviousOrigin = Cache.surfCharacter.transform.position; }
        Cache.surfCharacter.active = active;
        if (active) { Cache.rb.isKinematic = Cache.col.isTrigger = true; }
    }
    public static void SetFixedBodyRotation(bool active)
    {
        Cache.surfCharacter.fixedBodyRotation = active;
    }
    public void SetRigidbody(bool active)
    {
        Vector3 rbVel = Cache.rb.velocity;
        Vector3 fragVel = Cache.moveData.Velocity;
        Cache.rb.isKinematic = Cache.col.isTrigger = !active;
        SetActive(!active);
        if (active) { Cache.rb.velocity = fragVel * rbToFragVelFactor; }
        else { Cache.moveData.Velocity = rbVel * fragVelToRbFactor; }
    }
    public static void Teleport(Vector3 pos)
    {
        SetActive(false);
        Cache.surfCharacter.transform.position = pos;
        SetActive(true);
    }
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetRigidbody(Cache.rb.isKinematic);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            SetActive(!Cache.surfCharacter.active);
        }
    }
    */
}
