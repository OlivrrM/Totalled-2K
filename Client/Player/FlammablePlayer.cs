using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammablePlayer : MonoBehaviour, IFlammable
{
    bool lit;
    float tickDamage;
    float hitsPerSecond;
    float currentLitTime;
    float currentHitTime;

    Damage fireTickDamage = new Damage();

    public ParticleSystem fireFx;

    public GameObject fireTrigger;
    private void Start()
    {
        fireTickDamage.type = Totalled.DamageType.FireTick;
        fireFx.Stop();
        fireTrigger.SetActive(false);
    }
    public void Light(Kindle kindle)
    {
        lit = true;
        fireTickDamage.amount = kindle.tickDamage;
        hitsPerSecond = kindle.ticksPerSecond;
        currentLitTime = kindle.litTime;
        fireFx.Play();
        fireTrigger.SetActive(true);
    }
    public void Extinguish()
    {
        lit = false;
        fireFx.Stop();
        fireTrigger.SetActive(false);
    }
    private void Update()
    {
        if (lit)
        {
            currentHitTime += Time.deltaTime;
            if (currentHitTime > 1f / hitsPerSecond)
            {
                Cache.health.Damage(fireTickDamage);
                currentHitTime = 0f;
            }

            currentLitTime -= Time.deltaTime;
            if (currentLitTime < 0)
            {
                Extinguish();
            }
        }
    }
}
