using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractureable : Breakable
{
    public GameObject aliveGraphic;
    public GameObject shatteredGraphic;

    public float destroyImpactMultiplier;
    public override void Break(Damage damage)
    {
        base.Break(damage);
        if (damage is ImpactDamage)
        {
            ImpactDamage impactDamage = (ImpactDamage)damage;
            GameObject shattered = Instantiate(shatteredGraphic, aliveGraphic.transform.position, aliveGraphic.transform.rotation);
            //shattered.transform.localScale = transform.localScale;
            Destroy(aliveGraphic);
            Collider[] colliders = Physics.OverlapSphere(impactDamage.raycastHit.point, impactDamage.radius,Cache.references.breakableForceIncludeLayerMask);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    if (rb.isKinematic) { rb.isKinematic = false; }
                    rb.AddExplosionForce(impactDamage.impactAmount * destroyImpactMultiplier, impactDamage.raycastHit.point, impactDamage.radius);
                }
            }
        }
        else if (damage is ExplosionDamage)
        {
            ExplosionDamage explosionDamage = (ExplosionDamage)damage;
            GameObject shattered = Instantiate(shatteredGraphic, aliveGraphic.transform.position, aliveGraphic.transform.rotation);
            Destroy(aliveGraphic);
            Collider[] colliders = Physics.OverlapSphere(explosionDamage.explosionPos, explosionDamage.explosionRadius);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(explosionDamage.explosionForce * destroyImpactMultiplier, explosionDamage.explosionPos, explosionDamage.explosionRadius);
                }
            }
        }
        else
        {
            GameObject shattered = Instantiate(shatteredGraphic, aliveGraphic.transform.position, aliveGraphic.transform.rotation);
            Destroy(aliveGraphic);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (Collider hit in colliders)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddExplosionForce(50f * destroyImpactMultiplier, transform.position, 2.5f);
                }
            }
        }
        Destroy(gameObject);
    }
}
