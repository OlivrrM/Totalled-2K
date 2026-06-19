using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObjectOnFallManager : MonoBehaviour
{
    public Vector2 velocityRange;
    public float damage;
    public float impactAmount;
    public float impactRadius;

    ImpactDamage dmg;

    float timeOffGround;
    float timeOnGround;
    private void Start()
    {
        dmg = new ImpactDamage();
        dmg.type = Totalled.DamageType.Fall;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (timeOffGround > 0.1f)
        {
            if (other.gameObject.layer != 6 && other.gameObject.layer != 3 && other.gameObject.layer != 20)
            {
                if (Cache.moveData.PreGroundedVelocity.y < 0)
                {
                    float vel = Mathf.Clamp(Mathf.Abs(Cache.moveData.PreGroundedVelocity.y), 0f, velocityRange.y);
                    if ((vel > velocityRange.x) && (vel <= velocityRange.y))
                    {
                        Breakable breakable = other.gameObject.GetComponent<Breakable>();
                        if (breakable != null)
                        {
                            float force = Mathf.InverseLerp(velocityRange.x, velocityRange.y, vel);
                            dmg.amount = damage * force;
                            //levi was here
                            dmg.impactAmount = impactAmount * force;
                            dmg.radius = impactRadius * force;
                            dmg.raycastHit = new RaycastHit { point = other.ClosestPoint(transform.position) };
                            breakable.Damage(dmg);
                            Cache.moveData.Velocity = new Vector3(Cache.moveData.Velocity.x, Cache.moveData.PreGroundedVelocity.y * 0.75f, Cache.moveData.Velocity.z);
                            Cache.fallDamage.objectBreakFallAmount = 0.5f;
                        }
                    }
                }
            }
        }
    }
    private void Update()
    {
        if (!FragMovementListener.grounded) { timeOffGround += Time.deltaTime; timeOnGround = 0f; }
        else { timeOnGround += Time.deltaTime; if (timeOnGround > 0.1f) { timeOffGround = 0f; Cache.fallDamage.objectBreakFallAmount = 1f; } }
    }
}
