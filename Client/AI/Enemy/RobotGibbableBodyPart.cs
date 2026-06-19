using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class RobotGibbableBodyPart : RobotBodyPart
{
    public RobotGibbablePartsManager robotGibbablePartsManager;
    public GameObject gib;
    public Transform pushPos;
    public float pushForce;
    public float pushRadius;
    public override void Damage(Damage damage)
    {
        base.Damage(damage);
        if (robotGibbablePartsManager.gibbedGibs < robotGibbablePartsManager.maxGibbables)
        {
            robotGibbablePartsManager.gibbedGibs++;
            gib.SetActive(true);
            gib.GetComponent<Rigidbody>().AddExplosionForce(pushForce, pushPos.position, pushRadius);
            gib.transform.parent = null;
            gameObject.SetActive(false);
        }
    }
}
