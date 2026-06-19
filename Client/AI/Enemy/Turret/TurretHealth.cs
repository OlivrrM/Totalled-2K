using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TurretHealth : MonoBehaviour
{
    public TransformTurret turret;

    public float health;

    public Transform turretHead;

    public float damageHeadKnockbackAmount;
    public Vector2 damageRangeMultiplier;
    Quaternion currentDamageHeadKnockbackAmount;
    Quaternion targetDamageHeadKnockbackAmount;
    public float knockbackSmoothness;
    public float knockbackSpeed;

    [Header("Death")]
    public Animator headAnimator;
    public GameObject deathFx;
    public Transform deathFxPos;
    [HideInInspector] public bool dead;
    private void Start()
    {
        currentDamageHeadKnockbackAmount = Quaternion.identity;
        targetDamageHeadKnockbackAmount = Quaternion.identity;
    }
    public void Damage(Damage damage, bool head)
    {
        health -= damage.amount;
        if (damage.attacker == Cache.surfCharacter.gameObject) { Cache.hitMarkerManager.Hit(damage.amount, false); }
        if (head){
            if (damage.attacker != null){
                //print((damageHeadKnockbackAmount * Mathf.InverseLerp(damageRangeMultiplier.x, damageRangeMultiplier.y, damage.amount)));
                Vector3 direction = (Cache.surfCharacter.transform.position - turretHead.position).normalized * (damageHeadKnockbackAmount * Mathf.InverseLerp(damageRangeMultiplier.x, damageRangeMultiplier.y, damage.amount));
                Quaternion rotation = Quaternion.LookRotation(direction);
                //print(rotation.eulerAngles);
                targetDamageHeadKnockbackAmount *= rotation;
            }
            else{
                Debug.LogError($"Turret '{gameObject.name}' just received damage with damagetype '{damage.type.ToString()}', however attacker was null. All attacks should be assigned an attacker by standard");
            }
        }
        if (health <= 0 && !dead){
            Die();
        }
    }
    public void Die()
    {
        dead = true;
        Instantiate(deathFx, deathFxPos.position, Quaternion.identity);
        headAnimator.SetBool("dead", true);
        for (int i = 0; i < turret.legAnimators.Length; i++){
            turret.legAnimators[i].SetBool("dead", true);
        }
        //gameObject.GetComponent<ResetObjectOnRespawn>().flagForReset = true; Disabled for now because its just broken
        
        //if (Cache.resetSequenceOnRespawn != null) { Cache.resetSequenceOnRespawn.objectsToReset}
    }

    private void Update()
    {
        currentDamageHeadKnockbackAmount = Quaternion.Lerp(currentDamageHeadKnockbackAmount, targetDamageHeadKnockbackAmount, Time.deltaTime * knockbackSmoothness);
        targetDamageHeadKnockbackAmount = Quaternion.Lerp(targetDamageHeadKnockbackAmount, Quaternion.identity, Time.deltaTime * knockbackSpeed);
        turretHead.localRotation = currentDamageHeadKnockbackAmount;
    }
}
