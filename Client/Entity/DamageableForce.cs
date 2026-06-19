using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableForce : MonoBehaviour
{
    public Rigidbody rb;
    public float damage;
    public Vector2 velocityRange;
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.velocity.magnitude > velocityRange.x || (collision.gameObject.layer == 12&& rb.velocity.magnitude>1f))// && rb.velocity.magnitude < velocityRange.y)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Damage dmg = new Damage();
                float force = Mathf.InverseLerp(velocityRange.x, velocityRange.y, rb.velocity.magnitude);
                dmg.amount = damage * force;
                dmg.attacker = Cache.surfCharacter.gameObject;
                dmg.type = Totalled.DamageType.Force;
                damageable.Damage(dmg);
                if (collision.gameObject.layer == 12)
                {
                    RobotBodyPart robotBodyPart = collision.gameObject.GetComponent<RobotBodyPart>();
                    if (robotBodyPart != null)
                    {
                        robotBodyPart.robot.robot.Stun((1 - force)/5f, 0f, ((1-force)+1)/3f);
                    }
                }
            }
            if (Cache.references.meleeImpacts.ContainsKey(collision.gameObject.tag))
            {
                Instantiate(Cache.references.meleeImpacts[collision.gameObject.tag], collision.contacts[0].point, Quaternion.LookRotation(collision.GetContact(0).normal));
            }
        }
    }
}
