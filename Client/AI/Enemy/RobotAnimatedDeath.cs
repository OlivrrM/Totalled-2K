using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RobotAnimatedDeath : RobotDeath
{
    public Robot robot;
    public Animator animator;

    public float pushAmount;
    public float explosionPushResistance;

    Vector3 currentVelocity;
    public float velocityDecaySpeed;

    public GameObject spawnOnDeath;
    public Behaviour[] disableComponentsOnDeath;

    Quaternion targetDirection;

    public override void Die(Damage damage)
    {
        //Check what damage IS for different behaviour types
        if (damage is ExplosionDamage){
            ExplosionDamage expDamage = (ExplosionDamage)damage;
            Vector3 dir = transform.position - expDamage.explosionPos;
            targetDirection = Quaternion.Euler(dir);
            currentVelocity = dir * (expDamage.explosionForce * explosionPushResistance);
        }
        else{
            currentVelocity = robot.agent.velocity * pushAmount;
        }
        animator.SetBool("Death", true);

        robot.agent.enabled = false;
        robot.enabled = false;

        if (spawnOnDeath != null) { Instantiate(spawnOnDeath, transform.position, Quaternion.identity); }

        for (int i = 0; i < disableComponentsOnDeath.Length; i++){
            disableComponentsOnDeath[i].enabled = false;
        }

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 2.5f;
        rb.drag = 0f;
    }
    private void Update()
    {
        if (currentVelocity != Vector3.zero) { transform.position += currentVelocity * Time.deltaTime; }
        currentVelocity = Vector3.Lerp(currentVelocity,Vector3.zero,velocityDecaySpeed*Time.deltaTime);
        if (targetDirection != Quaternion.identity) {
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetDirection, Time.deltaTime * 10f); //OLD
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetDirection,
                360f * Time.deltaTime // maxDegreesDelta
            );
        }
    }
}
