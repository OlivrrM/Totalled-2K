using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Totalled;
using TMPro;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float health;

    public DamagePlayerVisuals damagePlayerVisuals;
    public HealPlayerVisuals healPlayerVisuals;

    public bool dead;

    public Vector3 respawnLocation;

    public Death death;

    public CenterOutwardsSlider hpSlider;
    float hpSliderPointer;
    public float hpPointerSmoothness;

    public Color sliderMaxHpColor;
    public Color sliderLowHpColor;
    Color sliderHpColorPointer;
    float defaultSliderAlpha;

    public TextMeshProUGUI hpText;
    float hpTextPointer;
    public Color textMaxHpColor;
    public Color textLowHpColor;
    Color textHpColorPointer;

    public TextMeshProUGUI hpTextBack;
    public Color textBackMaxHpColor;
    public Color textBackLowHpColor;
    Color textBackHpColorPointer;
    float hpAlpha;

    float timeAfterTakingDamage;

    public bool god;

    public HealthSplatterDecalManager healthSplatterDecalManager;
    private void Start()
    {
        health = maxHealth;
        hpSlider.SetMaxValue(maxHealth);
        defaultSliderAlpha = hpSlider.leftSliderGraphic.color.a;
        timeAfterTakingDamage = 999f;
        Cache.health = this;
    }
    public void Damage(Damage damage)
    {
        if (!dead && !god)
        {
            health -= damage.amount;
            if (damage is DamagePlayer) { damagePlayerVisuals.Play((DamagePlayer)damage); }
            else if (damage is ExplosionDamage){ damagePlayerVisuals.PlayExplosion((ExplosionDamage)damage); }
            else { damagePlayerVisuals.Play(damage); }
            if (health <= 0f) { Die(); }
            timeAfterTakingDamage = 0f;
            healthSplatterDecalManager.Damage(damage);
        }
    }
    public void Heal(Heal heal)
    {
        if (!dead)
        {
            health += heal.amount;
            health = Mathf.Clamp(health, 0f, maxHealth);
            if (heal is HealPlayer) { healPlayerVisuals.Play((HealPlayer)heal); }
        }
    }
    void Die()
    {
        dead = true;
        death.Die();
    }
    void Respawn()
    {
        dead = false;
        death.Respawn();
        health = maxHealth*0.65f;
        FragMovementManager.Teleport(respawnLocation);
        hpTextPointer = maxHealth*0.65f;

        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("Damage", Vector3.zero);
        damagePlayerVisuals.currentBobDirection = Vector3.zero;
        damagePlayerVisuals.targetBobDirection = Vector3.zero;

        if (Cache.ammo.amount < 20) { Cache.ammo.ChangeAmmo(20); }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)&&dead)
        {
            Respawn();
        }

        timeAfterTakingDamage += Time.deltaTime;
        hpAlpha = Mathf.Lerp(hpAlpha, ((timeAfterTakingDamage > 5f) && (health > maxHealth * 0.25f)) ? 0.5f : 1f, Time.deltaTime*2);

        hpSliderPointer = Mathf.Lerp(hpSliderPointer, health, Time.deltaTime * hpPointerSmoothness);
        hpSlider.SetValue(hpSliderPointer);

        sliderHpColorPointer = Color.Lerp(sliderHpColorPointer, Color.Lerp(sliderLowHpColor, sliderMaxHpColor, Mathf.InverseLerp(0f, maxHealth, health)), Time.deltaTime * hpPointerSmoothness);
        hpSlider.SetColor((sliderHpColorPointer * new Color(1f,1f,1f,0f))+new Color(0f,0f,0f,defaultSliderAlpha* hpAlpha));

        hpTextPointer = health > 1f ? (health > 99f ? 100f : (Mathf.Lerp(hpTextPointer, health, Time.deltaTime * hpPointerSmoothness * 2))) : 1f;
        hpText.text = (dead ? 0f : (int)hpTextPointer).ToString();

        textHpColorPointer = Color.Lerp(textHpColorPointer, Color.Lerp(textLowHpColor, textMaxHpColor, Mathf.InverseLerp(0f, maxHealth, health)), Time.deltaTime * hpPointerSmoothness);
        hpText.color = textHpColorPointer * new Color(1f,1f,1f, hpAlpha);

        textBackHpColorPointer = Color.Lerp(textBackHpColorPointer, Color.Lerp(textBackLowHpColor, textBackMaxHpColor, Mathf.InverseLerp(0f, maxHealth, health)), Time.deltaTime * hpPointerSmoothness);
        hpTextBack.color = textBackHpColorPointer * new Color(1f, 1f, 1f, hpAlpha);
    }
}
