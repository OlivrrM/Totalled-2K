using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDuelFirearmShoot : MonoBehaviour
{
    public Robot robot;

    public float shootSpeed;
    float currentShootCd;

    bool shootRight;

    public Transform shootPosA;
    public Transform shootPosB;

    public GameObject projectile;
    public GameObject muzzleFlash;
    public GameObject shootSfx;

    float shootingRange;
    public float shootForwardAngle;

    private void Start()
    {
        shootingRange = robot.agroRadius * 0.8f;
        currentShootCd = shootSpeed;
    }
    private void Update()
    {
        if (CanShoot())
        {
            currentShootCd -= Time.deltaTime;
            if (currentShootCd < 0f)
            {
                Transform shootPos = shootRight ? shootPosA : shootPosB;
                Instantiate(projectile, shootPos.position, shootPos.rotation);
                Instantiate(muzzleFlash, shootPos.position, shootPos.rotation);
                Instantiate(shootSfx, shootPos.position, shootPos.rotation);

                shootRight = !shootRight;
                currentShootCd = shootSpeed;
            }
        }
    }
    bool CanShoot()
    {
        if ((Vector3.Distance(transform.position, Cache.surfCharacter.transform.position) < shootingRange) && robot.timeSinceLastReachablePos <= 0f)
        {
            if (robot.chasingLOS || robot.attackingLOS)
            {
                Vector3 directionToAgrovator = Cache.vcam.transform.position - shootPosA.position;
                directionToAgrovator.Normalize();
                float angle = Vector3.Angle(shootPosA.forward, directionToAgrovator);
                if (angle <= shootForwardAngle / 2)
                {
                    //if (robot.distanceFromTarget > backpeddleRadius/2f){
                    //    return true;
                    //}
                    return true;
                }
            }
        }
        return false;
    }
}
