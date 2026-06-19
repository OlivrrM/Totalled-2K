using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class SpiderRobotAttack : MonoBehaviour
{
    public Robot robot;
    public float stepAmount;

    public RobotSpeedManager robotSpeedManager;

    float reachedDistance;
    public float requiredReachedDistance;
    bool startedSideMoving;

    public float directionTime;
    float currentDirectionTime;
    float direction;

    public ConstantRotation miniGunSpinnerA;
    public ConstantRotation miniGunSpinnerB;
    public float miniGunSpinAmount;

    float stoppedMovingTime;

    public Transform head;
    public float shootForwardAngle;

    public float searchingSpeedMultiplier;

    [Header("Shooting")]
    float shootingRange;

    bool activateShooting;
    float timeAfterActiveShooting;

    public GameObject projectile;
    public float shootCd;
    float currentShootCd;

    public Transform shootPosA;
    public Transform shootPosB;
    public GameObject muzzleFlashFx;
    bool shootPosRight;

    [Header("Backpeddle")]
    public float backpeddleRadius;

    [Header("Downwards Aim")]
    public OverrideTransform leftGunAim;
    public OverrideTransform rightGunAim;
    public float downwardsAimDistance;

    [Header("Sfx")]
    public GameObject shootSfx;
    public AudioSource machineGunLoopSfx;
    public PlaySound machineGunEndSfx;
    bool playedMgEndSfx;

    [Header("Eyes")]
    public GameObject[] eyes;
    List<Material> eyeMaterials = new List<Material>();
    List<SpriteRenderer> eyeBlooms = new List<SpriteRenderer>();
    private void Start()
    {
        direction = Utilities.CoinFlipn();
        robot.checkAttackingLOS = true;
        shootingRange = robot.agroRadius * 0.8f;
        robotSpeedManager.AddValue("BackPeddleSpeed", 1f);
        robotSpeedManager.AddValue("SearchingSpeed", 1f);

        for (int i = 0; i < eyes.Length; i++){
            eyeMaterials.Add(eyes[i].GetComponent<MeshRenderer>().materials[0]);
            eyeBlooms.Add(eyes[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
        }
    }
    private void Update()
    {
        if (Robot.ghost) { return; }
        if (robot.currentActionState == RobotActionState.Attacking)
        {
            /*
            reachedDistance = Vector3.Distance(transform.position, robot.agent.destination);
            if (reachedDistance < requiredReachedDistance || !startedSideMoving){
                startedSideMoving = true;
                robot.SetDestination(transform.position + ((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.right) * (stepAmount * direction)));
            }
            */
            if (robot.attackingLOS)
            {
                /*
                NavMeshPath path = new NavMeshPath();
                Vector3 newDestination = transform.position + ((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.right) * (stepAmount * direction));
                int pathsChecked = 0;
                while (true)
                {
                    if (robot.agent.CalculatePath(newDestination, path))
                    {
                        robot.SetDestination(newDestination);
                        break;
                    }
                    else
                    {
                        direction = -direction;
                        newDestination = transform.position + ((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.right) * (stepAmount * direction));
                    }
                    pathsChecked++;
                    if (pathsChecked > 100)
                    {
                        print($"100 PATHS CHECKED FOR '{transform.name}' ('SpiderRobotAttack.cs'): NONE FOUND");
                        break;
                    }
                }
                */
                if (robot.distanceFromTarget < backpeddleRadius)
                {
                    robot.SetDestination(transform.position+((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.back) * backpeddleRadius));
                    robotSpeedManager.UpdateValue("BackPeddleSpeed", 1.25f);
                }
                else
                {
                    robot.SetDestination(transform.position + ((Utilities.GetRotationTowards(robot.transform.position, Cache.surfCharacter.transform.position) * Vector3.right) * (stepAmount * direction)));
                    currentDirectionTime += Time.deltaTime;
                    if (currentDirectionTime > directionTime)
                    {
                        direction = -direction;
                        currentDirectionTime = 0f;
                    }

                    if (robot.agent.velocity.magnitude < 1f)
                    {
                        stoppedMovingTime += Time.deltaTime;
                        if (stoppedMovingTime > 0.5f)
                        {
                            direction = -direction;
                            stoppedMovingTime = 0f;
                        }
                    }
                    robotSpeedManager.UpdateValue("BackPeddleSpeed", 1f);
                }
                robotSpeedManager.UpdateValue("SearchingSpeed", 1f);
            }
            else
            {
                robot.SetDestination(robot.lastKnownTargetPos);
                robotSpeedManager.UpdateValue("BackPeddleSpeed", 1f);
                robotSpeedManager.UpdateValue("SearchingSpeed", searchingSpeedMultiplier);
            }
        }
        else
        {
            startedSideMoving = false;
            robotSpeedManager.UpdateValue("BackPeddleSpeed", 1f);
        }
        if (robot.currentActionState == RobotActionState.Attacking || robot.currentActionState == RobotActionState.Chasing)
        {
            for (int i = 0; i < eyes.Length; i++)
            {
                eyeBlooms[i].color = Color.Lerp(eyeBlooms[i].color, new Color(1,0,0,0.5f), Time.deltaTime * 2f);
                eyeMaterials[i].SetColor("_Color", Color.Lerp(eyeMaterials[i].GetColor("_Color"), Color.red, Time.deltaTime * 2f));
            }
        }
        else
        {
            for (int i = 0; i < eyes.Length; i++){
                eyeBlooms[i].color = Color.Lerp(eyeBlooms[i].color, Utilities.Invisible(eyeBlooms[i].color), Time.deltaTime * 2f);
                eyeMaterials[i].SetColor("_Color", Color.Lerp(eyeMaterials[i].GetColor("_Color"), Color.black, Time.deltaTime * 2f));
            }
        }

        if (CanShoot())
        {
            timeAfterActiveShooting += Time.deltaTime;
            miniGunSpinnerA.rotation = Vector3.Lerp(miniGunSpinnerA.rotation, new Vector3(0, miniGunSpinAmount, 0f), Time.deltaTime * 2f);
            miniGunSpinnerB.rotation = Vector3.Lerp(miniGunSpinnerB.rotation, new Vector3(0, miniGunSpinAmount, 0f), Time.deltaTime * 2f);
            if (timeAfterActiveShooting > 1f)
            {
                currentShootCd += Time.deltaTime;
                if (currentShootCd > shootCd)
                {
                    if (shootPosRight) { 
                        Instantiate(projectile, shootPosA.position, shootPosA.rotation); 
                        Instantiate(muzzleFlashFx, shootPosA.position, shootPosA.rotation,shootPosA);
                        Instantiate(shootSfx, shootPosA.position, shootPosA.rotation, shootPosA);
                    }
                    else {
                        Instantiate(projectile, shootPosB.position, shootPosB.rotation); 
                        Instantiate(muzzleFlashFx, shootPosB.position, shootPosB.rotation,shootPosB);
                        Instantiate(shootSfx, shootPosB.position, shootPosB.rotation, shootPosB);
                    }
                    shootPosRight = !shootPosRight;
                    currentShootCd = 0f;
                }
            }
            playedMgEndSfx = false;
            machineGunLoopSfx.volume += Time.deltaTime;
        }
        else
        {
            timeAfterActiveShooting -= Time.deltaTime;
            miniGunSpinnerA.rotation = Vector3.Lerp(miniGunSpinnerA.rotation, Vector3.zero, Time.deltaTime * 2f);
            miniGunSpinnerB.rotation = Vector3.Lerp(miniGunSpinnerB.rotation, Vector3.zero, Time.deltaTime * 2f);
            if (!playedMgEndSfx){
                machineGunEndSfx.Play();
                playedMgEndSfx = true;
            }
            machineGunLoopSfx.volume -= Time.deltaTime;
        }
        timeAfterActiveShooting = Mathf.Clamp(timeAfterActiveShooting, 0f, 2f);

        if (robot.distanceFromTarget < downwardsAimDistance)
        {
            float downwardsAmount = Mathf.InverseLerp(0f,downwardsAimDistance, robot.distanceFromTarget);
            downwardsAmount = 1 - downwardsAmount;
            leftGunAim.weight = downwardsAmount;
            rightGunAim.weight = downwardsAmount;
        }
        else
        {
            leftGunAim.weight = Mathf.Lerp(leftGunAim.weight,0f,Time.deltaTime);
            rightGunAim.weight = Mathf.Lerp(rightGunAim.weight, 0f, Time.deltaTime);
        }
    }
    bool CanShoot()
    {
        if (Vector3.Distance(transform.position, Cache.surfCharacter.transform.position) < shootingRange){
            if (robot.chasingLOS || robot.attackingLOS){
                Vector3 directionToAgrovator = Cache.vcam.transform.position - head.position;
                directionToAgrovator.Normalize();
                float angle = Vector3.Angle(head.forward, directionToAgrovator);
                if (angle <= shootForwardAngle / 2){
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
