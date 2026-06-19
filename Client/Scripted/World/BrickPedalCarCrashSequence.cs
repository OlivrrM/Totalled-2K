using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickPedalCarCrashSequence : MonoBehaviour
{
    public Material brickIndicatorMaterial;
    public SpriteRenderer brickIndicatorSpriteRenderer;
    float brickIndicatorIntensity;

    public Transform brickPlaceIndicator;
    BrickPedalTrigger brickPedalTrigger;
    public Transform placedBrick;

    public Animator carAnimator;
    public Animator door1Animator;

    bool placed;

    public ParticleSystem notMovingSmoke;
    public ParticleSystem movingSmoke;
    public ParticleSystem dirt0;
    public ParticleSystem dirt1;

    public GameObject silo;
    public GameObject brokenSilo;

    public Detonation detonation;

    Vector3 witheredJumpyCoupeDefaultPos;
    public GameObject witheredJumpyCoupe;
    public Rigidbody witheredJumpyCoupeRb;
    public Transform witheredJumpyCoupeXRequirement;
    bool boom;

    public GameObject[] enemiesToEnable;

    public PlaySound sequenceSfx;

    public CrossInstanceEventCleaner eventCleaner;
    private void Start()
    {
        brickPedalTrigger = brickPlaceIndicator.GetComponent<BrickPedalTrigger>();
        witheredJumpyCoupeDefaultPos = witheredJumpyCoupe.transform.position;
        for (int i = 0; i < enemiesToEnable.Length; i++){
            enemiesToEnable[i].SetActive(false);
        }
    }
    public void Trigger()
    {
        placed = true;
        placedBrick.position = PlayerTargetInfo.pos;
        placedBrick.localScale = Vector3.zero;
        brickPlaceIndicator.gameObject.SetActive(false);
        Cache.inventory.ClearHeldItem();
        door1Animator.SetBool("SlowClose", true);
        movingSmoke.Play();
        StartCoroutine(Sequence());
        
    }
    public void RestartSequence()
    {
        witheredJumpyCoupe.transform.position = witheredJumpyCoupeDefaultPos;
        carAnimator.gameObject.SetActive(true);
        witheredJumpyCoupe.SetActive(false);
        carAnimator.SetBool("Move", false);
        Trigger();
    }
    IEnumerator Sequence()
    {
        sequenceSfx.Play();
        yield return new WaitForSeconds(0.15f);
        notMovingSmoke.Stop();
        carAnimator.SetBool("Move",true);
        dirt0.Play();
        dirt1.Play();
        yield return new WaitForSeconds(1.5f);
        dirt0.Stop();
        dirt1.Stop();
        yield return new WaitForSeconds(1.166f);
        movingSmoke.Stop();
        yield return new WaitForSeconds(0.75f);
        silo.SetActive(false);
        brokenSilo.SetActive(true);
        carAnimator.gameObject.SetActive(false);
        witheredJumpyCoupe.SetActive(true);
        detonation.Explode();
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < enemiesToEnable.Length; i++){
            enemiesToEnable[i].SetActive(true);
        }
        boom = true;
        eventCleaner.MarkEventComplete();
    }
    private void Update()
    {
        if (!placed){
            if (Cache.inventory.equippedItem != null){
                if (Cache.inventory.equippedItem.identifier == "Brick"){
                    brickPedalTrigger.interactable = true;
                }
                else{
                    brickPedalTrigger.interactable = false;
                }
            }
            else{
                brickPedalTrigger.interactable = false;
            }
            brickIndicatorIntensity = Mathf.PingPong(Time.time * 1f, 1f);
            brickIndicatorMaterial.SetFloat("_FresnelPower", brickIndicatorIntensity + 0.15f);
            brickIndicatorSpriteRenderer.color = new Color(brickIndicatorSpriteRenderer.color.r, brickIndicatorSpriteRenderer.color.g, brickIndicatorSpriteRenderer.color.b, brickIndicatorIntensity);
        }
        else{
            placedBrick.position = Vector3.Lerp(placedBrick.position, brickPlaceIndicator.position, Time.deltaTime * 10f);
            placedBrick.localScale = Vector3.Lerp(placedBrick.localScale, brickPlaceIndicator.localScale, Time.deltaTime * 10f);
            placedBrick.rotation = Quaternion.Lerp(placedBrick.rotation, brickPlaceIndicator.rotation, Time.deltaTime * 10f);
        }
    }
    private void FixedUpdate()
    {
        if (boom){
            if (witheredJumpyCoupeRb.transform.position.x < witheredJumpyCoupeXRequirement.position.x){
                witheredJumpyCoupeRb.AddForce(new Vector3(50f,0f,0f));
            }
        }
    }
}