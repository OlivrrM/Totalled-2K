using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WheelWrench : Item
{
    public float damage;
    public float impactForce;
    public float hitDelay;

    public float attackCd;
    float currentAttackCd;
    bool attackHit;

    public Animator animator;

    public PlayRandomSound swingSound;
    public PlayRandomSound hitSound;

    public float attackRange;

    //public LayerMask layerMask;

    float timeAfterHit;
    public CamItemOffsetBob camItemOffsetBob;

    float defaultBobSmoothness;
    private void Start()
    {
        defaultBobSmoothness = camItemOffsetBob.bobSmoothness;
        timeAfterHit = 99f;
    }
    public override void OnMainAction()
    {
        if (currentAttackCd <= 0)
        {
            currentAttackCd = attackCd;
            timeAfterHit = 99f;
            attackHit = false;
            Attack();
            StartCoroutine(EndAttack());
        }
    }
    IEnumerator EndAttack()
    {
        yield return new WaitForEndOfFrame();
        animator.SetInteger("Attack", 0);
        animator.SetBool("Hit", false);
    }
    public override void Update()
    {
        animator.SetBool("Inspect", false);

        base.Update();

        if ((currentAttackCd>0.1f)&&(currentAttackCd < (attackCd-hitDelay)) &&!attackHit) 
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, attackRange, Cache.references.bulletLayerMask))
            {
                animator.SetBool("Hit", true);
                timeAfterHit = 0f;
                attackHit = true;
                DelayHit(hit);
            }
        }

        if (currentAttackCd > 0f)
        {
            currentAttackCd -= Time.deltaTime;
            if (currentAttackCd <= 0f){
                //animator.SetBool("Hit", false);
            }
        }

        timeAfterHit += Time.deltaTime;
        camItemOffsetBob.bobSmoothness = timeAfterHit <= 1f ? 25f : defaultBobSmoothness;
    }
    public override void OnInspect()
    {
        base.OnInspect();
        animator.SetBool("Inspecting", true);
        animator.SetBool("Inspect", true);
    }
    public override void OnInspectFinish()
    {
        base.OnInspectFinish();
        animator.SetBool("Inspecting", false);
    }
    public override void OnInspectCancel()
    {
        base.OnInspectCancel();
        animator.SetBool("Inspecting", false);
    }
    void Attack()
    {
        animator.SetInteger("Attack", Random.RandomRange(1, 3));
        /*
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward,out hit, attackRange, Cache.references.bulletLayerMask))
        {
            animator.SetBool("Hit", true);
            timeAfterHit = 0f;
            StartCoroutine(DelayHit(hit));
        }
        */
        swingSound.Play();
    }
    void DelayHit(RaycastHit hit) 
    {
        //yield return new WaitForSeconds(hitDelay);
        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(-hit.normal * impactForce);
        }
        hitSound.Play();
        IDamageable damageable = hit.transform.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (rb != null)
            {
                ImpactDamage impactDamage = new ImpactDamage();
                impactDamage.type = Totalled.DamageType.Melee;
                impactDamage.amount = damage;
                impactDamage.impactAmount = impactForce;
                impactDamage.raycastHit = hit;
                impactDamage.radius = 5f;
                impactDamage.attacker = Cache.surfCharacter.gameObject;
                damageable.Damage(impactDamage);
            }
            else
            {
                Damage incomingDamage = new Damage();
                incomingDamage.type = Totalled.DamageType.Melee;
                incomingDamage.amount = damage;
                incomingDamage.attacker = Cache.surfCharacter.gameObject;
                damageable.Damage(incomingDamage);
            }
        }

        GameObject hole;
        if (Cache.references.meleeImpacts.ContainsKey(hit.transform.tag))
        {
            hole = Instantiate(Cache.references.meleeImpacts[hit.transform.tag], hit.point, Quaternion.LookRotation(hit.normal));
        }
        else
        {
            Debug.LogError("Shot object does not have bullet hole tag reference! ('" + hit.transform.name + "')");
            hole = Instantiate(Cache.references.meleeImpacts["Null"], hit.point, Quaternion.LookRotation(hit.normal));
        }
        hole.transform.parent = hit.transform;
    }
}
