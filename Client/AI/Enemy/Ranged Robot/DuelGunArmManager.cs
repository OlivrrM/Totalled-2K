using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using Unity.Burst.Intrinsics;

public class DuelGunArmManager : MonoBehaviour
{
    public Robot robot;

    public RobotGunArmAnimator[] robotGunArmAnimators;

    public GameObject projectile;
    public GameObject shootMuzzleFlash;
    public GameObject shootSfx;

    public float shootTime;
    public float shootRange;

    float currentShootTime;
    int armToShoot;

    bool shooting;

    private void Update()
    {
        if (!shooting)
        {
            if (robot.currentActionState == RobotActionState.Chasing)
            {
                if (robot.chasingLOS)
                {
                    if (robot.distanceFromTarget < shootRange)
                    {
                        currentShootTime += Time.deltaTime;
                        if (currentShootTime > shootTime)
                        {
                            StartCoroutine(ShootingPhase(armToShoot));
                            armToShoot++;
                            if (armToShoot >= robotGunArmAnimators.Length) { armToShoot = 0; }
                            currentShootTime = 0f;
                            shooting = true;
                        }
                    }
                }
            }
        }
    }
    public void ShootFromGun(int armId)
    {
        Instantiate(projectile, robotGunArmAnimators[armId].shootPoint.position, robotGunArmAnimators[armId].shootPoint.rotation);
        Instantiate(shootSfx, robotGunArmAnimators[armId].shootPoint.position, robotGunArmAnimators[armId].shootPoint.rotation);
        Instantiate(shootMuzzleFlash, robotGunArmAnimators[armId].shootPoint.position, robotGunArmAnimators[armId].shootPoint.rotation);
    }
    IEnumerator ShootingPhase(int armId)
    {
        robotGunArmAnimators[armId].StartShootingProcess();
        yield return new WaitUntil(() => !robotGunArmAnimators[armId].canShoot);
        shooting = false;
    }
}
