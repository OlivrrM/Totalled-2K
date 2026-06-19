using System;
using System.Collections;
using System.Collections.Generic;
using Totalled;
using UnityEngine;
using static Cinemachine.CinemachineFreeLook;

public class BarbedWire : MonoBehaviour
{
    public float damage;
    public float pushAmount;
    public float upwardsPushAmount;

    public PlaySound sfx;
    public PlayRandomSound rsfx;

    public Totalled.CamDamageVisualValues camDamageVisualValues;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==6){
            Vector3 objectForward = transform.forward;
            Vector3 toPlayer = (other.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(objectForward, toPlayer);
            DamagePlayer damagePlayer = new DamagePlayer();
            damagePlayer.amount = damage;
            #region
            damagePlayer.direction = dotProduct > 0f ? transform.right : -transform.right;
            damagePlayer.directionAmount = camDamageVisualValues.camDirectionAmount;
            damagePlayer.directionRandomnessAmount = camDamageVisualValues.camDirectionRandomnessAmount;
            damagePlayer.directionReturnSpeed = camDamageVisualValues.camDirectionReturnSpeed;
            damagePlayer.directionSpeed = camDamageVisualValues.camDirectionSpeed;
            damagePlayer.postFxAmount = camDamageVisualValues.postFxAmount;
            damagePlayer.postFxDisableTimeMultiplier = camDamageVisualValues.postFxDisableTimeMultiplier;
            #endregion
            damagePlayer.type = DamageType.Environment;
            Cache.health.Damage(damagePlayer);
            if (sfx != null) { sfx.Play(); }
            if (rsfx != null) { rsfx.Play(); }
            Cache.moveData.Velocity += ((transform.forward * (dotProduct  > 0f ? 1f : -1f)) * pushAmount) + (Cache.surfCharacter.transform.up * upwardsPushAmount);
        }
    }
}
