using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

public class FallDamage : MonoBehaviour
{
    public float minVelocity;
    public float deathVelocity;

    public float bobAmount;

    public Health health;

    public PlayRandomSound mediumFallSounds;
    public PlayRandomSound hardFallSounds;

    public AudioSource fallingIdleSfx;
    public float fallingIdleSfxSmoothness;
    float currentFallingSfxVolume;
    float targetFallingSfxVolume;

    [HideInInspector] public float objectBreakFallAmount =1f;
    private void Start()
    {
        objectBreakFallAmount = 1f;
        Cache.fallDamage = this;
    }
    private void LateUpdate()
    {
        if (FragMovementListener.justGrounded)
        {
            if (-Cache.moveData.PreGroundedVelocity.y >= minVelocity)
            {
                DamagePlayer damagePlayer = new DamagePlayer();
                float hitForce = Mathf.InverseLerp(minVelocity, deathVelocity, -Cache.moveData.PreGroundedVelocity.y);
                damagePlayer.amount = (hitForce * 100)* objectBreakFallAmount;
                damagePlayer.direction = Camera.main.transform.right;
                damagePlayer.directionAmount = bobAmount * hitForce;
                damagePlayer.directionRandomnessAmount = 0.02f;
                damagePlayer.directionSpeed = 10 * (Mathf.Clamp(hitForce, 0, 1) * 2);
                damagePlayer.directionReturnSpeed = 1f;
                damagePlayer.postFxDisableTimeMultiplier = 4-(3*hitForce);
                damagePlayer.type = DamageType.Fall;
                damagePlayer.postFxAmount = Mathf.Clamp(Mathf.Clamp(hitForce * 3, 0f, 1f),0.4f,1f);
                health.Damage(damagePlayer);
                if (-Cache.moveData.PreGroundedVelocity.y > 25f) { hardFallSounds.Play(); }
                else { mediumFallSounds.Play(); }
            }
        }
    }
    private void Update()
    {
        targetFallingSfxVolume = Mathf.InverseLerp(-10, -200, Cache.moveData.Velocity.y);
        currentFallingSfxVolume = Mathf.Lerp(currentFallingSfxVolume, targetFallingSfxVolume, Time.deltaTime * fallingIdleSfxSmoothness);
        fallingIdleSfx.volume = currentFallingSfxVolume;
    }
}
