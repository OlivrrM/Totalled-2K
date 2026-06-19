using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotExplosionDeath : RobotDeath
{
    public GameObject[] bodyPartGibs;
    public float explosionForce;
    public float explosionRadius;
    public float shootBackForce;

    public GameObject explosionFx;
    public Transform fxPos;
    private void Start()
    {
        for (int i = 0; i < bodyPartGibs.Length; i++){
            bodyPartGibs[i].SetActive(false);
        }
    }
    public override void Die(Damage damage)
    {
        for (int i = 0; i < bodyPartGibs.Length; i++){
            bodyPartGibs[i].SetActive(true);
            bodyPartGibs[i].transform.parent = null;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();

            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                if (damage.type == Totalled.DamageType.Bullet){
                    hitRb.AddForce(Cache.surfCharacter.transform.forward * shootBackForce);
                }
            }
        }
        Instantiate(explosionFx, fxPos.position, Quaternion.identity);
        base.Die(damage);
    }
}
