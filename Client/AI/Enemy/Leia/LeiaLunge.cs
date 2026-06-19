using System.Collections;
using System.Collections.Generic;
using Totalled;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;
using UnityEngine.ProBuilder;

public class LeiaLunge : MonoBehaviour
{
    public Robot robot;
    public Animator animator;
    public RobotHealth robotHealth;

    public static PostFxVolumeManager blankScreenPostFxVolumeManager;

    [Header("Lunge Requirements")]
    public float distanceToLunge;
    public float requiredAngleToLunge;

    [Header("Lunge Info")]
    public float lungeDistance;
    public float lungePossibleHeight;
    public float lungeTime;
    public float lungeEndPushAmount;

    [Header("Attack Info")]
    public float attackDistance;
    public float attackTime;
    bool attacking;
    float currentAttackingTime;
    public CinemachineVirtualCamera jumpscareCam;

    bool lunging;
    Vector3 startPos;
    Vector3 endPos;
    float currentLungeTime;

    float forceSearchTime;

    [Header("Hit Wall")]
    public float hitWallCheckDistance;
    float hitHeadTime;
    public float hitWallPushBackAmount;
    public float hitWallForce;
    public float hitWallDamage;
    public float hitWallImpactRadius;

    [Header("Sfx")]
    public PlaySound attackSfx;
    public PlaySound attackDamageSfx;
    bool playedAttackDamageSfx;
    public PlaySound hitWallSfx;
    public PlaySound hitWallBashSfx;
    public PlaySound leapSfx;
    private void Start()
    {
        hitHeadTime = 99f;
        forceSearchTime = 99f;
        if (!Cache.walkSpeedManager.values.ContainsKey("JumpscareSpeed")) { Cache.walkSpeedManager.AddValue("JumpscareSpeed", 1f); }
        if (blankScreenPostFxVolumeManager == null) { blankScreenPostFxVolumeManager = GameObject.Find("BlankScreenDeathPostFxVolume").GetComponent<PostFxVolumeManager>(); }
    }

    void Update()
    {
        if (CanLunge())
        {
            Vector3 targetLungePos = Vector3.zero;
            RaycastHit xzHit;
            Debug.DrawRay(PlayerTargetInfo.pos, transform.forward* lungeDistance, UnityEngine.Color.red,1f);
            if (Physics.Raycast(Cache.surfCharacter.transform.position + new Vector3(0f,0.1f,0f), transform.forward,out xzHit, lungeDistance, Cache.references.solidLayerMask)){
                targetLungePos = xzHit.point + (-transform.forward / 2f);
            }
            else{
                targetLungePos = (Cache.surfCharacter.transform.position + new Vector3(0f, 0.1f, 0f)) - (-transform.forward * lungeDistance);
            }
            /*
            RaycastHit yHit;
            if (Physics.Raycast(targetLungePos + new Vector3(0f,lungePossibleHeight,0f),Vector3.down,out yHit, lungePossibleHeight, Cache.references.solidLayerMask)){
                targetLungePos = new Vector3(targetLungePos.x, yHit.point.y, targetLungePos.z);
            }
            print(targetLungePos);
            */
            if (!lunging) { leapSfx.Play(); }
            lunging = true;
            startPos = robot.transform.position;
            endPos = targetLungePos;
            robot.agent.enabled = false;
        }
        if (lunging && !attacking && hitHeadTime >=0.5f)
        {
            robot.currentActionState = RobotActionState.Custom;
            currentLungeTime += Time.deltaTime;
            robot.transform.position = Vector3.Lerp(startPos, endPos, currentLungeTime / lungeTime);
            if (currentLungeTime >= lungeTime) {
                robot.agent.enabled = true;
                currentLungeTime = 0f;
                lunging = false;
                robot.currentActionState = RobotActionState.Chasing;
                robot.agent.velocity += robot.transform.forward * lungeEndPushAmount;
            }
            if (Vector3.Distance(robot.head.transform.position,Cache.mainCam.position) <= attackDistance) {
                attacking = true;
                playedAttackDamageSfx = false;
                attackSfx.Play();
                robot.agent.velocity += robot.transform.forward * lungeEndPushAmount;
            }

            RaycastHit wallHit;
            Debug.DrawRay(robot.head.position, -robot.head.forward * hitWallCheckDistance, UnityEngine.Color.magenta, 0.33f);
            if (Physics.Raycast(robot.head.position,-robot.head.forward,out wallHit, hitWallCheckDistance, Cache.references.solidLayerMask))
            {
                if (Cache.references.bulletHoles.ContainsKey(wallHit.transform.tag)){
                    Instantiate(Cache.references.meleeImpacts[wallHit.transform.tag], wallHit.point, Quaternion.LookRotation(wallHit.normal));
                }
                else{
                    Instantiate(Cache.references.meleeImpacts["Null"], wallHit.point, Quaternion.LookRotation(wallHit.normal));
                }
                Rigidbody rb = wallHit.transform.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.AddForce(transform.forward * hitWallForce);
                }
                Breakable breakable = wallHit.transform.gameObject.GetComponent<Breakable>();
                if (breakable != null)
                {
                    float force = hitWallForce;
                    ImpactDamage dmg = new ImpactDamage();
                    dmg.type = DamageType.Fall;
                    dmg.amount = hitWallDamage;
                    dmg.impactAmount = force;
                    dmg.radius = hitWallImpactRadius/3f;
                    dmg.raycastHit = wallHit;
                    breakable.Damage(dmg);
                }
                robotHealth.Damage(new Damage { type = DamageType.Fall, amount = 4f,attacker = gameObject });
                hitHeadTime = 0f;
                robot.agent.velocity += -robot.transform.forward * hitWallPushBackAmount;
                hitWallSfx.Play();
                hitWallBashSfx.Play();
            }
        }
        if (hitHeadTime < 0.5f)
        {
            hitHeadTime += Time.deltaTime;
            animator.SetBool("HitWall", true);
            transform.position += -transform.forward * (hitWallPushBackAmount * (1-Mathf.InverseLerp(0f,0.5f, hitHeadTime))) * Time.deltaTime;
            if (hitHeadTime >= 0.5f)
            {
                animator.SetBool("HitWall", false);
                robot.agent.enabled = true;
                currentLungeTime = 0f;
                lunging = false;
                robot.currentActionState = RobotActionState.Chasing;
            }
        }
        if (!lunging && !attacking) {
            if (robot.currentActionState == RobotActionState.Attacking || robot.currentActionState == RobotActionState.Chasing) {
                if (robot.distanceFromTarget < (distanceToLunge * 0.75f)){
                    forceSearchTime = 0f;
                }
            }
        }
        if (forceSearchTime< 3.33f)
        {
            forceSearchTime += Time.deltaTime;
            robot.forceSearch = true;
        }
        else { robot.forceSearch = false; }

        if (attacking)
        {
            jumpscareCam.Priority = 99;
            animator.SetBool("Jumpscaring", true);
            currentAttackingTime += Time.deltaTime;
            Cache.walkSpeedManager.UpdateValue("JumpscareSpeed", 0.1f);
            if (!playedAttackDamageSfx)
            {
                if (currentAttackingTime > (attackTime - 0.4f))
                {
                    attackDamageSfx.Play();
                    playedAttackDamageSfx = true;
                }
            }
            if (currentAttackingTime >= attackTime || robotHealth.health<=0f){

                DamagePlayer damagePlayer = new DamagePlayer();
                damagePlayer.amount = 999f;
                damagePlayer.direction = robot.transform.right;
                damagePlayer.directionAmount = 30f;
                damagePlayer.directionRandomnessAmount = 1f;
                damagePlayer.directionReturnSpeed = 3f;
                damagePlayer.directionSpeed = 3f;
                damagePlayer.postFxAmount = 1f;
                damagePlayer.postFxDisableTimeMultiplier = 1f;
                damagePlayer.type = DamageType.Melee;

                if (robotHealth.health > 0f){
                    Cache.health.Damage(damagePlayer);
                    if (!Cache.health.god) { StartCoroutine(BlankScreenTiming()); }
                }

                animator.SetBool("Jumpscaring", false);
                robot.agent.enabled = true;
                currentLungeTime = 0f;
                lunging = false;
                robot.currentActionState = RobotActionState.Chasing;
                attacking = false;
                jumpscareCam.Priority = -999;
                currentAttackingTime = 0f;

                Cache.walkSpeedManager.UpdateValue("JumpscareSpeed", 1f);
            }
        }
        if (robotHealth.health <= 0f)
        {
            attackSfx.SFX.Stop();
        }

        animator.SetBool("Lunging", lunging);
    }
    IEnumerator BlankScreenTiming()
    {
        blankScreenPostFxVolumeManager.Enable(99f);
        yield return new WaitForSeconds(0.5f);
        if (Death.dead){
            blankScreenPostFxVolumeManager.Disable(0.5f);
        }
    }
    bool CanLunge()
    {
        if (lunging||Death.dead||Robot.ghost) { return false; }
        if (robot.distanceFromTarget < distanceToLunge){
            if (robotHealth.health > 0f){
                Vector3 directionToAgrovator = Cache.vcam.transform.position - robot.head.position;
                directionToAgrovator.Normalize();
                float angle = Vector3.Angle(-robot.head.forward, directionToAgrovator);
                if (angle <= requiredAngleToLunge / 2){
                    RaycastHit hit;
                    if (!Physics.Raycast(robot.head.position, directionToAgrovator, out hit, robot.distanceFromTarget, Cache.references.solidLayerMask)){
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
