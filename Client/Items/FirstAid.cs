using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstAid : Interactable
{
    public Slider slider;

    public float maxHealth;
    public float health = 100;

    public float healSpeed;

    float postAmount;
    bool interacting;

    public PlaySound startSfx;
    public PlaySound startHolding;
    public PlaySound startEnd;
    bool stoppedSfx;
    bool startedSfx;

    private void Start()
    {
        slider.maxValue = maxHealth;
        //health = maxHealth;
        slider.value = health;
    }
    private void OnEnable()
    {
        ResetSequenceOnRespawn.onSequenceReset += OnSequenceReset;
    }
    private void OnDisable()
    {
        ResetSequenceOnRespawn.onSequenceReset -= OnSequenceReset;
    }
    public void OnSequenceReset()
    {
        if (health < maxHealth / 2f) { health = maxHealth / 2f; }
        slider.value = health;
    }
    private void LateUpdate()
    {
        if (!interacting) {
            postAmount = Mathf.Clamp(postAmount- Time.deltaTime,0f,0.8f);
            if (!stoppedSfx) 
            {
                startHolding.SFX.Stop();
                startEnd.Play();
                stoppedSfx = true;
                startedSfx = false;
            }
        }
        interacting = false;
    }
    public override void HeldInteract()
    {
        base.HeldInteract();
        if (health > 0 && Cache.health.health < Cache.health.maxHealth)
        {
            if (!startedSfx){
                startSfx.Play();
                startHolding.Play();
                stoppedSfx = false;
                startedSfx = true;
            }
            interacting = true;
            HealPlayer heal = new HealPlayer();
            float healthTaken = Time.deltaTime * healSpeed;
            if (healthTaken > health){
                healthTaken = health;
            }
            heal.amount = healthTaken;
            heal.type = Totalled.HealType.Consumable;
            heal.postFxTime = 0.2f;
            heal.postFxDisableSmoothness = 3f;
            postAmount = Mathf.Clamp(postAmount+(Time.deltaTime / 4f),0f,0.8f);
            heal.postFxAmount = postAmount;
            Cache.health.Heal(heal);
            health -= healthTaken;
            slider.value = health;
        }
    }
}
