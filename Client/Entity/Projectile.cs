using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float impactForce;
    public float damage;
    public Vector2 damageDistanceAB = Vector2.one;
    public Vector2 damageDistanceMultiplierAB = Vector2.one;
    Vector3 startPoint;
    public float lifetime;

    public bool canHitPlayer;
    [HideInInspector] public LayerMask layerMask;

    public bool customDamageVisualValues;
    public CamDamageVisualValues damageVisualValues;
    private void Start()
    {
        startPoint = transform.position;
        Destroy(gameObject, lifetime);
        if (canHitPlayer) { layerMask = Cache.references.enemyBulletLayerMask; }
        else { layerMask = Cache.references.bulletLayerMask; }
    }
    private void Update()
    {
        Vector3 lastPos = transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(lastPos, transform.position), Color.green, 0.1f);
        if (Physics.Raycast(transform.position,transform.forward,out hit, Vector3.Distance(lastPos, transform.position), layerMask))
        {
            Hit(hit.point,hit.transform, hit.normal);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerMask)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position,Utilities.GetRotationTowards(transform.position, other.ClosestPoint(transform.position)).eulerAngles,out hit, 999f, layerMask))
            {
                Hit(other.ClosestPoint(transform.position), other.transform,hit.normal);
            }
            else { Hit(other.ClosestPoint(transform.position), other.transform,transform.eulerAngles); }
        }
    }
    void Hit(Vector3 point,Transform hitTransform, Vector3 normal)
    {
        float traveledDistance = Vector3.Distance(startPoint, point);
        float t = Mathf.InverseLerp(damageDistanceAB.x, damageDistanceAB.y, traveledDistance);
        float finalMultiplier = Mathf.Lerp(damageDistanceMultiplierAB.x, damageDistanceMultiplierAB.y, t);
        damage = damage * finalMultiplier;

        if (customDamageVisualValues) { ImpactManager.Hit(this, point, hitTransform, normal,damageVisualValues); }
        else { ImpactManager.Hit(this, point, hitTransform, normal); }
        Destroy(gameObject);
    }
}
