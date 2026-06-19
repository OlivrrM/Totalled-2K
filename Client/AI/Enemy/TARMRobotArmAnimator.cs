using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class TARMRobotArmAnimator : MonoBehaviour
{
    [Header("General")]
    public Robot robot;
    public Transform target;

    float time;

    Vector3 posTarget;
    Quaternion rotTarget;

    public RobotSpeedManager robotSpeedManager;

    [Header("Idle")]
    public Vector3 idlePosA;
    public Vector3 idlePosB;
    public Vector3 idleRotA;
    public Vector3 idleRotB;
    public float idleSpeed;

    [Header("Chasing")]
    public Vector3 attackChargePos;
    public Vector3 attackChargeRot;
    public float attackChargeSmoothness;
    public Vector2 attackChargeDistanceMinMax;

    [Header("Attacking")]
    public float attackCd;
    public Vector3 attackPos;
    public Vector3 attackRot;
    float attackCdTime;
    public float attackSmoothness;
    public float attackDamageDelay;
    bool dealtDamage;
    public float attackAnimationTime;
    float currentAttackAnimationTime;

    [Header("Damage")]
    public float damage;
    public float speedAfterDamage;
    public float speedRegainSmoothness;
    public Vector2 pushAmount;
    float currentSpeed;

    [Header("DamagePlayer")]
    public float camDirectionAmount;
    public float camDirectionRandomnessAmount;
    public float camDirectionReturnSpeed;
    public float camDirectionSpeed;
    public float postFxAmount;
    public float postFxDisableTimeMultiplier;

    [Header("SFX")]
    public PlaySound attackSfx;
    public PlaySound attackHitSfx;

    [Header("Drill")]
    public float drillDistance;
    public PlaySound drillStartSfx;
    public AudioSource drillingSfx;
    float targetDrillVolumeSfx;
    public PlaySound drillEndSfx;
    bool drilling;
    bool drillEnabled;
    public ConstantRotation drillRotatorA;
    public ConstantRotation drillRotatorB;
    public float drillSpeed;

    [HideInInspector] public bool dontEffectPlayer;
    private void Start()
    {
        if (speedAfterDamage != 0)
        {
            robotSpeedManager.AddValue("AttackSpeed", 1f);
            currentSpeed = 1f;
        }
        if (drillingSfx != null) { drillEnabled = true; }
    }

    private void Update()
    {
        if (currentAttackAnimationTime > 0f)
        {
            currentAttackAnimationTime -= Time.deltaTime;
            posTarget = attackPos;
            target.localPosition = Vector3.Lerp(target.localPosition, posTarget, Time.deltaTime * attackSmoothness);
            Attacking();
        }
        else
        {
            switch (robot.currentActionState)
            {
                case RobotActionState.Idle:
                    Idle();
                    break;
                case RobotActionState.Chasing:
                    ChargingAttack();
                    break;
                case RobotActionState.Attacking:
                    Attacking();
                    break;
            }
        }

        if (attackCdTime > 0f){
            attackCdTime -= Time.deltaTime;
            dealtDamage = false;
            if (attackCdTime < 0f) { attackCdTime = 0f; }
        }

        if (speedAfterDamage != 0){
            currentSpeed = Mathf.Lerp(currentSpeed, 1f, Time.deltaTime * speedRegainSmoothness);
            robotSpeedManager.UpdateValue("AttackSpeed", currentSpeed);
        }

        if (drillEnabled)
        {
            if (Vector3.Distance(robot.transform.position, Cache.surfCharacter.transform.position) < drillDistance && !Death.dead && robot.currentActionState != RobotActionState.Idle && robot.stunRegaining)
            {
                if (!drilling)
                {
                    drillStartSfx.Play();
                    targetDrillVolumeSfx = 0.5f;
                    drilling = true;
                }
            }
            else
            {
                if (drilling)
                {
                    drillEndSfx.Play();
                    targetDrillVolumeSfx = 0f;
                    //drillingSfx.volume = 0f;
                    drillStartSfx.SFX.Stop();
                    drilling = false;
                }
            }
            if (drilling) {
                drillingSfx.volume = Mathf.Lerp(drillingSfx.volume, targetDrillVolumeSfx, Time.deltaTime * 3f);
                drillRotatorA.rotation = Vector3.Lerp(drillRotatorA.rotation, new Vector3(drillRotatorA.rotation.x, drillRotatorA.rotation.y, drillSpeed), Time.deltaTime * 3f);
                drillRotatorB.rotation = Vector3.Lerp(drillRotatorB.rotation, new Vector3(drillRotatorB.rotation.x, drillRotatorB.rotation.y, drillSpeed), Time.deltaTime * 3f);
            }
            else {
                drillingSfx.volume = Mathf.Lerp(drillingSfx.volume, targetDrillVolumeSfx, Time.deltaTime * 5f);
                drillRotatorA.rotation = Vector3.Lerp(drillRotatorA.rotation, new Vector3(drillRotatorA.rotation.x, drillRotatorA.rotation.y, 0f), Time.deltaTime * 3f);
                drillRotatorB.rotation = Vector3.Lerp(drillRotatorB.rotation, new Vector3(drillRotatorB.rotation.x, drillRotatorB.rotation.y, 0f), Time.deltaTime * 3f);
            }
        }
    }
    void Idle()
    {
        float pong = Mathf.PingPong(Time.time * idleSpeed, 1);
        posTarget = Vector3.Lerp(idlePosA, idlePosB, pong);
        target.localPosition = Vector3.Lerp(target.localPosition, posTarget, Time.deltaTime * idleSpeed);
        rotTarget = Quaternion.Lerp(Quaternion.Euler(idleRotA), Quaternion.Euler(idleRotB), pong);
        target.localRotation = Quaternion.Lerp(target.localRotation, rotTarget, Time.deltaTime * idleSpeed);
    }
    void ChargingAttack()
    {
        float distancePercent = Mathf.InverseLerp(attackChargeDistanceMinMax.x, attackChargeDistanceMinMax.y, Vector3.Distance(robot.transform.position, Cache.surfCharacter.transform.position));
        distancePercent = 1 - distancePercent;
        if (distancePercent > 0f){
            Vector3 targetPos = Vector3.Lerp(idlePosA, attackChargePos, distancePercent);
            Quaternion targetRot = Quaternion.Lerp(Quaternion.Euler(idleRotA), Quaternion.Euler(attackChargeRot), distancePercent);

            posTarget = Vector3.Lerp(posTarget, targetPos, Time.deltaTime * attackChargeSmoothness);
            rotTarget = Quaternion.Lerp(rotTarget, targetRot, Time.deltaTime * attackChargeSmoothness);

            target.localPosition = Vector3.Lerp(target.localPosition, posTarget, Time.deltaTime * attackChargeSmoothness);
            target.localRotation = Quaternion.Lerp(target.localRotation, rotTarget, Time.deltaTime * attackChargeSmoothness);
        }
        else{
            Idle();
        }
    }
    void Attacking()
    {
        if (attackCdTime <= 0f&&!Death.dead&&robot.stunRegaining)
        {
            if (attackCdTime == 0f) {
                currentAttackAnimationTime = attackAnimationTime; 
                if (attackSfx != null) { attackSfx.Play(); }
            }
            attackCdTime -= Time.deltaTime;
            if (attackCdTime < -attackDamageDelay)
            {
                if (!dealtDamage) {
                    if (speedAfterDamage != 0) { currentSpeed = speedAfterDamage; }
                    if (damage != 0f)
                    {
                        DamagePlayer damagePlayer = new DamagePlayer();
                        damagePlayer.amount = damage;
                        damagePlayer.direction = robot.transform.right;
                        damagePlayer.directionAmount = camDirectionAmount;
                        damagePlayer.directionRandomnessAmount = camDirectionRandomnessAmount;
                        damagePlayer.directionReturnSpeed = camDirectionReturnSpeed;
                        damagePlayer.directionSpeed = camDirectionSpeed;
                        damagePlayer.postFxAmount = postFxAmount;
                        damagePlayer.postFxDisableTimeMultiplier = postFxDisableTimeMultiplier;
                        damagePlayer.type = DamageType.Melee;

                        if (!dontEffectPlayer) {
                            Cache.health.Damage(damagePlayer);
                            attackHitSfx.Play();
                        }

                    }

                    if (!dontEffectPlayer) { Cache.moveData.Velocity += (robot.transform.forward * pushAmount.x) + new Vector3(0f, pushAmount.y, 0f); }
                    dealtDamage = true;
                    attackCdTime = attackCd;
                }
            }
        }
        else
        {
            ChargingAttack();
        }
    }
}
