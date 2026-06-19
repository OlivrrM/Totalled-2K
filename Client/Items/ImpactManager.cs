using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using System.Windows.Forms;
public class ImpactManager : MonoBehaviour
{
    public static CamDamageVisualValues camDamageVisualValues = new CamDamageVisualValues
    {
        camDirectionAmount = 50f,
        camDirectionRandomnessAmount = 0.05f,
        camDirectionReturnSpeed = 5f,
        camDirectionSpeed = 10f,
        postFxAmount = 0f, // Dynamic
        postFxDisableTimeMultiplier = 1f
    };

    public static void Hit(Firearm firearm, RaycastHit hit)
    {
        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        bool isKinematic = false;
        if (rb != null){
            if (rb.isKinematic) { isKinematic = true; }
            else { rb.AddForce(-hit.normal * firearm.impactForce); }
        }

        IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        // levi was here
        // lwvi was kitten

        if (damageable != null)
        {
            if (rb != null)
            {
                ImpactDamage impactDamage = new ImpactDamage();
                impactDamage.type = Totalled.DamageType.Bullet;
                impactDamage.amount = firearm.damage;
                impactDamage.impactAmount = firearm.impactForce;
                impactDamage.raycastHit = hit;
                impactDamage.radius = 5f;
                impactDamage.attacker = Cache.surfCharacter.gameObject;
                damageable.Damage(impactDamage);
            }
            else
            {
                Damage incomingDamage = new Damage();
                incomingDamage.type = Totalled.DamageType.Bullet;
                incomingDamage.amount = firearm.damage;
                incomingDamage.attacker = Cache.surfCharacter.gameObject;
                damageable.Damage(incomingDamage);
            }
        }
        if (isKinematic){ //If target was kinematic, add force after Damage is called, as some objects dont have kinematics disabled until after they take damaage.
            if (rb != null){
                rb.AddForce(-hit.normal * firearm.impactForce);
            }
        }
        GameObject hole;
        if (Cache.references.bulletHoles.ContainsKey(hit.transform.tag)){
            hole = Instantiate(Cache.references.bulletHoles[hit.transform.tag], hit.point, Quaternion.LookRotation(hit.normal));
        }
        else{
            //Debug.LogError("Shot object does not have bullet hole tag reference! ('" + hit.transform.name + "')");
            hole = Instantiate(Cache.references.bulletHoles["Null"], hit.point, Quaternion.LookRotation(hit.normal));
        }
        hole.transform.parent = hit.transform;
    }
    //get hacked by levi
    // david is kitten
    public static void Hit(Projectile projectile,Vector3 point, Transform hitTransform, Vector3 normal, CamDamageVisualValues? camDmgVisualValues = null)
    {
        camDmgVisualValues ??= camDamageVisualValues;
        Rigidbody rb = hitTransform.GetComponent<Rigidbody>();
        if (rb != null){
            rb.AddForce(-projectile.transform.eulerAngles.normalized * projectile.impactForce);
        }

        IDamageable damageable = hitTransform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (rb != null)
            {
                if (hitTransform.gameObject.layer == 6)
                {
                    DamagePlayer incomingDamage = new DamagePlayer();
                    incomingDamage.type = Totalled.DamageType.Bullet;
                    incomingDamage.amount = projectile.damage;

                    incomingDamage.direction = projectile.transform.up;
                    incomingDamage.directionAmount = camDmgVisualValues.Value.camDirectionAmount;
                    incomingDamage.directionReturnSpeed = camDmgVisualValues.Value.camDirectionReturnSpeed;
                    incomingDamage.directionSpeed = camDmgVisualValues.Value.camDirectionSpeed;
                    incomingDamage.directionRandomnessAmount = camDmgVisualValues.Value.camDirectionRandomnessAmount;
                    incomingDamage.postFxAmount = Mathf.InverseLerp(0f, 100f, projectile.damage*2);
                    incomingDamage.postFxDisableTimeMultiplier = camDmgVisualValues.Value.postFxDisableTimeMultiplier;
                    incomingDamage.attacker = projectile.gameObject; //This should point to actual attacker and not projectile, currently it doenst

                    damageable.Damage(incomingDamage);
                }
                else
                {
                    ImpactDamage impactDamage = new ImpactDamage();
                    impactDamage.type = Totalled.DamageType.Bullet;
                    impactDamage.amount = projectile.damage;
                    impactDamage.impactAmount = projectile.impactForce;
                    impactDamage.raycastHit = new RaycastHit { point = point };
                    impactDamage.radius = 5f;
                    impactDamage.attacker = projectile.gameObject;
                    damageable.Damage(impactDamage);
                    //Cache.hitMarkerManager.Hit(projectile.damage);
                }
            }
            else
            {
                Damage incomingDamage = new Damage();
                incomingDamage.type = Totalled.DamageType.Bullet;
                incomingDamage.amount = projectile.damage;
                incomingDamage.attacker = projectile.gameObject;
                damageable.Damage(incomingDamage);
            }
        }

        GameObject hole;
        if (Cache.references.bulletHoles.ContainsKey(hitTransform.tag)){
            hole = Instantiate(Cache.references.bulletHoles[hitTransform.tag], point, Quaternion.LookRotation(normal));
        }
        else{
            //Debug.LogError("Shot object does not have bullet hole tag reference! ('" + hitTransform.name + "')");
            hole = Instantiate(Cache.references.bulletHoles["Null"], point, Quaternion.LookRotation(normal));
        }
        hole.transform.parent = hitTransform;
    }
}