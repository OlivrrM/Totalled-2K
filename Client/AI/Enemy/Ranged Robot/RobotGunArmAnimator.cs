using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Totalled;
using Cinemachine.Utility;
using UnityEditor.Rendering;

public class RobotGunArmAnimator : MonoBehaviour
{
    public Robot robot;
    public DuelGunArmManager duelGunArmManager;

    public int armId;
    public Transform shootPoint;
    public Transform target;
    public Transform defaultTargetPos;
    public Transform recoilPoint;
    public ChainIKConstraint constraint;
    public float aimSmoothness;
    public float afterShotSmoothness;
    public bool canShoot;
    public float shootRecoil;

    public Transform shoulder;
    Vector3 defaultLocalShoulderPos;
    Vector3 defaultLocalShoulderRot;
    public Vector3 aimingLocalShoulderPos;
    public Vector3 aimingLocalShoulderRot;

    float shootTime;

    Vector3 targetPos;

    public float leadingAmount;

    public Transform wallClipRayCheckPoint;
    public float wallClickRayCheckDistance;
    enum ShootingPhase
    {
        aiming,
        goingBackToIdle
    }
    ShootingPhase shootingPhase = ShootingPhase.aiming;
    public void StartShootingProcess()
    {
        shootTime = 0f;
        canShoot = true;
        shootingPhase = ShootingPhase.aiming;
    }
    private void Start()
    {
        defaultLocalShoulderPos = shoulder.localPosition;
        defaultLocalShoulderRot = shoulder.localEulerAngles;
    }
    private void Update()
    {
        Vector3 playerLeadingPos = PlayerTargetInfo.pos + (Cache.moveData.Velocity * leadingAmount);
        //if (Input.GetKeyDown(KeyCode.F)) { shootingPhase = ShootingPhase.aiming; }
        //if (Input.GetKeyDown(KeyCode.G)) { shootingPhase = ShootingPhase.goingBackToIdle; }
        if (canShoot && (robot.currentActionState == RobotActionState.Chasing || robot.currentActionState == RobotActionState.Attacking) && robot.stunRegaining)
        {
            constraint.weight = Mathf.Lerp(constraint.weight, 1f, Time.deltaTime * 5f);
            Vector3 direction;
            switch (shootingPhase) 
            {
                case ShootingPhase.aiming:
                    //direction = (Cache.mainCam.position - defaultTargetPos.position).normalized;
                    //targetPos = defaultTargetPos.position+(direction*1.5f);
                    targetPos = playerLeadingPos;
                    target.position = Vector3.Lerp(defaultTargetPos.position, targetPos, Mathf.InverseLerp(0f, 0.6f, shootTime));
                    //target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * aimSmoothness);
                    shoulder.localPosition = Vector3.Lerp(shoulder.localPosition, aimingLocalShoulderPos, Time.deltaTime * 3f);
                    shoulder.localEulerAngles = Quaternion.Lerp(Quaternion.Euler(shoulder.localEulerAngles), Quaternion.Euler(aimingLocalShoulderRot), Time.deltaTime * 3f).eulerAngles;
                    shootTime += Time.deltaTime;
                    if (shootTime > 0.6f){
                        shootingPhase = ShootingPhase.goingBackToIdle;
                        target.position = playerLeadingPos;
                        //Debug.DrawRay(wallClipRayCheckPoint.position, wallClipRayCheckPoint.forward, Color.red, wallClickRayCheckDistance);
                        if (!Physics.Raycast(wallClipRayCheckPoint.position,wallClipRayCheckPoint.forward, wallClickRayCheckDistance, Cache.references.solidLayerMask)){
                            duelGunArmManager.ShootFromGun(armId);
                        }
                    }
                    break;
                case ShootingPhase.goingBackToIdle:
                    if (Vector3.Distance(defaultTargetPos.position, targetPos) > 1.5f){
                        Vector3 d = (playerLeadingPos - defaultTargetPos.position).normalized;
                        targetPos = target.position = defaultTargetPos.position + (d * 1.5f);
                    }
                    direction = (playerLeadingPos - recoilPoint.position).normalized;
                    targetPos = recoilPoint.position + (direction * shootRecoil);
                    target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * afterShotSmoothness);
                    shoulder.localPosition = Vector3.Lerp(shoulder.localPosition, defaultLocalShoulderPos, Time.deltaTime * 3f);
                    shoulder.localEulerAngles = Quaternion.Lerp(Quaternion.Euler(shoulder.localEulerAngles), Quaternion.Euler(defaultLocalShoulderRot), Time.deltaTime * 3f).eulerAngles;
                    constraint.weight = Mathf.Lerp(constraint.weight, 0.25f, Time.deltaTime);
                    shootTime += Time.deltaTime;
                    if (shootTime > 1f){
                        canShoot = false;
                    }
                    break;
            }

        }
        else
        {
            shoulder.localPosition = Vector3.Lerp(shoulder.localPosition, defaultLocalShoulderPos, Time.deltaTime * 3f);
            shoulder.localEulerAngles = Quaternion.Lerp(Quaternion.Euler(shoulder.localEulerAngles), Quaternion.Euler(defaultLocalShoulderRot), Time.deltaTime * 3f).eulerAngles;
            constraint.weight = Mathf.Lerp(constraint.weight, 0.25f, Time.deltaTime * 0.5f);
            targetPos = defaultTargetPos.position;
            target.position = Vector3.Lerp(target.position, defaultTargetPos.position, Time.deltaTime);
        }
    }
}
