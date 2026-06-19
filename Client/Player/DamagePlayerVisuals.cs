using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Totalled;

public class DamagePlayerVisuals : MonoBehaviour
{
    public Health health;

    public float damagePostFxShowTime;
    public float damagePostFxDisableSpeed;
    public PostFxVolumeManager damagePostFxVolumeManager;
    public PostFxVolumeManager secondaryDamagePostFxVolumeManager;

    public Image damageScreen;
    RectTransform damageScreenRect;
    Color defaultDamageScreenColor;
    Color targetDamageScreenColor;
    float currentDamageScreenColorChangeSpeed;

    float currentCamBobSpeed;
    float currentReturnSpeed;
    [HideInInspector] public Vector3 targetBobDirection;
    [HideInInspector] public Vector3 currentBobDirection;

    public PostFxVolumeManager damagedPostFxVolumeManager;

    public PostFxVolumeManager enflamedPostFxVolumeManager;

    public PlayRandomSound flameDamageTickSfx;

    public AudioSource heartBeat;

    private void Start()
    {
        defaultDamageScreenColor = damageScreen.color;
        targetDamageScreenColor = Utilities.Invisible(defaultDamageScreenColor);
        damageScreen.color = Utilities.Invisible(defaultDamageScreenColor);
        damageScreenRect = damageScreen.rectTransform;
        damagedPostFxVolumeManager.Disable(99f);
        Cache.cameraWorldSpaceRotationOffsetManager.AddOffset("Damage", Vector3.zero);
    }
    public void Play(DamagePlayer damagePlayer) //Used for precise control over damage visuals
    {
        damagePostFxVolumeManager.Enable(20f,damagePlayer.postFxAmount+ damagePostFxVolumeManager.currentVisibility);
        secondaryDamagePostFxVolumeManager.Enable(20f, damagePlayer.postFxAmount + damagePostFxVolumeManager.currentVisibility);
        targetDamageScreenColor = defaultDamageScreenColor;
        targetDamageScreenColor = new Color(targetDamageScreenColor.r, targetDamageScreenColor.g, targetDamageScreenColor.b, damagePlayer.postFxAmount + damagePostFxVolumeManager.currentVisibility);
        damageScreenRect.Rotate(new Vector3(0, 0, Random.RandomRange(0, 360)));
        currentDamageScreenColorChangeSpeed = 20f;
        float disablePostFxSpeedMultiplier = 1;
        disablePostFxSpeedMultiplier = damagePlayer.postFxDisableTimeMultiplier;
        targetBobDirection = damagePlayer.direction * damagePlayer.directionAmount;
        targetBobDirection += new Vector3(Random.RandomRange(-360, 360) * damagePlayer.directionRandomnessAmount, Random.RandomRange(-360, 360) * damagePlayer.directionRandomnessAmount, Random.RandomRange(-360, 360) * damagePlayer.directionRandomnessAmount);
        currentCamBobSpeed = damagePlayer.directionSpeed;
        currentReturnSpeed = damagePlayer.directionReturnSpeed;
        StartCoroutine(DisableDamagePostFx(damagePostFxShowTime, disablePostFxSpeedMultiplier));
    }
    public void Play(Damage damage) //Used for damage taken without specific visuals data. This really should be the standard
    {
        switch (damage.type)
        {
            case DamageType.FireTick:
                enflamedPostFxVolumeManager.Enable(5f, 1f* Mathf.InverseLerp(0f, 8f, damage.amount));
                damagePostFxVolumeManager.Enable(20f, damage.amount/50f);
                targetBobDirection += Utilities.V3All(Random.RandomRange(-360, 360)*(0.1f*Mathf.InverseLerp(0f,10f,damage.amount)));
                currentCamBobSpeed = 12f;
                currentReturnSpeed = 4f;
                flameDamageTickSfx.Play();
                StartCoroutine(DisableEnflamedPostFx(0.2f));
                StartCoroutine(DisableDamagePostFx(1f, 2f));
                break;
        }
    }
    public void PlayExplosion(ExplosionDamage explosionDamage)
    {
        DamagePlayer damagePlayer = new DamagePlayer();
        damagePlayer.amount = explosionDamage.amount;
        damagePlayer.direction = (transform.position - explosionDamage.explosionPos).normalized + new Vector3(-2f, 0f, 0f);
        float distanceMultiplier = (1 - Mathf.InverseLerp(0f, explosionDamage.explosionRadius, Vector3.Distance(explosionDamage.explosionPos, transform.position)));
        damagePlayer.directionAmount = 25f * distanceMultiplier;
        damagePlayer.directionRandomnessAmount = 0.02f;
        damagePlayer.directionReturnSpeed = 1f;
        damagePlayer.directionSpeed = 10f;
        damagePlayer.postFxAmount = 2f * distanceMultiplier;
        damagePlayer.postFxDisableTimeMultiplier = 1f;
        damagePlayer.type = DamageType.Explosion;
        Play(damagePlayer);
    }
    IEnumerator DisableDamagePostFx(float time, float disableSpeedMultiplier)
    {
        yield return new WaitForSeconds(time);
        damagePostFxVolumeManager.Disable(damagePostFxDisableSpeed * disableSpeedMultiplier);
        secondaryDamagePostFxVolumeManager.Disable((damagePostFxDisableSpeed * disableSpeedMultiplier) * 10);
        currentDamageScreenColorChangeSpeed = damagePostFxDisableSpeed * disableSpeedMultiplier;
        targetDamageScreenColor = Utilities.Invisible(defaultDamageScreenColor);

    }
    IEnumerator DisableEnflamedPostFx(float time)
    {
        yield return new WaitForSeconds(time);
        enflamedPostFxVolumeManager.Disable(1f);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)&&Application.isEditor)
        {
            GameObject.Find("Health").GetComponent<Health>().Damage(new DamagePlayer { amount = 10f, postFxDisableTimeMultiplier = 3f, type = DamageType.Unknown, direction = Camera.main.transform.right, directionSpeed = 10, directionAmount = 20, directionReturnSpeed = 1, directionRandomnessAmount = 0.02f });
        }

        damageScreen.color = Color.Lerp(damageScreen.color, targetDamageScreenColor, Time.deltaTime * currentDamageScreenColorChangeSpeed);
        currentBobDirection = Vector3.Lerp(currentBobDirection, targetBobDirection, Time.deltaTime * currentCamBobSpeed);
        if (!Death.dead) { targetBobDirection = Vector3.Lerp(targetBobDirection, Vector3.zero, Time.deltaTime * currentReturnSpeed); }
        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("Damage", currentBobDirection);

        float hpPercentage = Mathf.InverseLerp(health.maxHealth*0.5f, 0, health.health);
        damagedPostFxVolumeManager.Enable(2, hpPercentage);

        heartBeat.volume = Mathf.InverseLerp(health.maxHealth * (health.dead ? 0f : 0.6f), 0, health.health);
    }
}
