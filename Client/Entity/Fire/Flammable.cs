using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour, IFlammable,IDestroy
{
    [HideInInspector] public bool lit;
    public ParticleSystem fireFx;
    ParticleSystem[] childFxs;
    public float tickDamage;
    public float hitsPerSecond;
    float currentHitTime;
    float currentLitTime;

    Damage fireTickDamage = new Damage();

    public bool clearFxOnDestroy;

    public PlaySound lightSfx;
    public AudioSource fireIdleSfx;
    public SmoothDisableSfx disableFireIdleSfx;

    public FireTrigger fireTrigger;

    IDamageable damageable;
    private void Start()
    {
        childFxs = GetComponentsInChildren<ParticleSystem>();
        fireTickDamage.amount = tickDamage;
        fireTickDamage.type = Totalled.DamageType.FireTick;
        if (fireFx != null) { fireFx.Stop(); }
        damageable = transform.GetComponent<IDamageable>();
        if (fireTrigger != null) { fireTrigger.gameObject.SetActive(false); }
    }
    public void Light(float lightTime = 999f)
    {
        if (!lit){
            lightSfx.Play();
            fireIdleSfx.Play();
            if (fireFx != null) { fireFx.Play(); }
        }
        lit = true;
        currentLitTime = lightTime;
        if (fireTrigger != null) { fireTrigger.gameObject.SetActive(true); }
    }
    public void Light(Kindle kindle)
    {
        if (!lit){
            lightSfx.Play();
            fireIdleSfx.Play();
            if (fireFx != null) { fireFx.Play(); }
        }
        lit = true;
        fireTickDamage.amount = kindle.tickDamage;
        hitsPerSecond = kindle.ticksPerSecond;
        currentLitTime = kindle.litTime;
        if (fireTrigger != null) { fireTrigger.gameObject.SetActive(true); }
    }
    public void Extinguish()
    {
        lit = false;
        if (fireFx != null) { fireFx.Stop(); }
        if (fireTrigger != null) { fireTrigger.gameObject.SetActive(false); }
    }
    private void Update()
    {
        if (lit){
            currentHitTime += Time.deltaTime;
            if (damageable != null) {
                if (currentHitTime > 1f / hitsPerSecond){
                    damageable.Damage(fireTickDamage);
                    currentHitTime = 0f;
                }
            }

            currentLitTime -= Time.deltaTime;
            if (currentLitTime < 0)
            {
                Extinguish();
            }

            fireIdleSfx.volume = Mathf.Lerp(fireIdleSfx.volume, 1f, Time.deltaTime*2f);
        }
        else
        {
            fireIdleSfx.volume = Mathf.Lerp(fireIdleSfx.volume, 0f, Time.deltaTime * 2f);
        }
    }
    public void Destroy()
    {
        if (!clearFxOnDestroy)
        {
            fireFx.transform.parent = null;
            fireFx.Stop();
            disableFireIdleSfx.Disable(1f);
            Destroy(fireFx.gameObject, 10f);
        }
    }
}
