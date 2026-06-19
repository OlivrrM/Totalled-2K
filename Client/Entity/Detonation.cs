using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonation : MonoBehaviour
{
    public GameObject explosionFx;

    public float radius;
    public float damageAmount;
    public float force;

    [Tooltip("Assign -1 for no delay")]
    public float ignitionDelay = -1;

    public bool destroyOnExplosion;

    public bool incendiary;

    public List<GameObject> ignoreObjects;
    public virtual void Start()
    {
        if (ignitionDelay != -1) { Invoke("Explode", ignitionDelay); }
    }
    public void Explode()
    {
        Instantiate(explosionFx, transform.position, Quaternion.identity);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject != gameObject && !ignoreObjects.Contains(hit.gameObject)) //Prevents STACK OVERFLOW
            {
                Rigidbody hitRb = hit.GetComponent<Rigidbody>();

                if (hitRb != null)
                {
                    hitRb.AddExplosionForce(force, transform.position, radius);
                }

                if (hit.gameObject == Cache.surfCharacter.gameObject)
                {
                    float wallBangThickness = 0f;

                    RaycastHit playerHit;
                    Vector3 direction = (Cache.surfCharacter.transform.position + new Vector3(0f, (Cache.surfCharacter.ColliderSize.y / 2), 0f)) - transform.position;
                    direction.Normalize();
                    //Debug.DrawRay(transform.position, direction * 10, Color.green, 10f);
                    if (Physics.Raycast(transform.position, direction, out playerHit, radius, Cache.references.playerLayerMask + Cache.references.solidLayerMask))
                    {
                        Vector3 wallbangA = playerHit.point;
                        RaycastHit wallHit;
                        Debug.DrawRay(Cache.surfCharacter.transform.position + new Vector3(0f, (Cache.surfCharacter.ColliderSize.y / 2), 0f), -direction * 10, Color.blue, 10f);
                        if (Physics.Raycast(Cache.surfCharacter.transform.position + new Vector3(0f, (Cache.surfCharacter.ColliderSize.y / 2), 0f), -direction, out wallHit, radius, Cache.references.solidLayerMask))
                        {
                            if (wallHit.transform.gameObject.layer != gameObject.layer)
                            {
                                Vector3 wallbangB = wallHit.point;
                                wallBangThickness = Vector3.Distance(wallbangA, wallbangB);
                            }
                        }
                    }

                    Vector3 pushForce = (Cache.surfCharacter.transform.position - transform.position).normalized * force;
                    Cache.moveData.Velocity += pushForce / 6f;

                    IDamageable damageable = hit.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        ExplosionDamage damage = new ExplosionDamage();
                        damage.amount = (damageAmount * (1 - Mathf.InverseLerp(0f, radius, Vector3.Distance(transform.position, hit.transform.position)))) * Mathf.InverseLerp(2f, 0f, wallBangThickness);
                        damage.type = Totalled.DamageType.Explosion;
                        damage.explosionRadius = radius;
                        damage.explosionPos = transform.position;
                        damage.explosionForce = force;
                        damage.attacker = Cache.surfCharacter.gameObject;
                        damageable.Damage(damage);
                    }
                }
                else
                {
                    IDamageable damageable = hit.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        ExplosionDamage damage = new ExplosionDamage();
                        damage.amount = damageAmount * (1 - Mathf.InverseLerp(0f, radius, Vector3.Distance(transform.position, hit.transform.position)));
                        damage.type = Totalled.DamageType.Explosion;
                        damage.explosionRadius = radius;
                        damage.explosionPos = transform.position;
                        damage.explosionForce = force;
                        damage.attacker = Cache.surfCharacter.gameObject;
                        damageable.Damage(damage);
                    }

                    if (incendiary) {
                        IFlammable flammable = hit.gameObject.GetComponent<IFlammable>();
                        if (flammable != null)
                        {
                            Kindle kindle = new Kindle();
                            kindle.litTime = Random.RandomRange(20f, 30f);
                            kindle.tickDamage = 5f;
                            kindle.ticksPerSecond = 2f;
                            flammable.Light(kindle);
                        }
                    }
                }
            }
        }
        if (destroyOnExplosion) { Destroy(gameObject); }
    }
}
