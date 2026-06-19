using Fragsurf.Movement;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TransformTurret : MonoBehaviour
{
    [Header("References")]
    public TurretHealth turretHealth;

    [Header("Movement")]
    public Transform head;
    public float rotationSpeed; // Degrees per second

    public float agroRadius;
    bool agrod;

    float timeSinceLastSeen;

    Quaternion directionFromLastSeen;

    [Header("Shooting")]
    public GameObject projectile;
    public GameObject muzzleFlash;
    public float shootCd;
    float currentShootCd;
    public Transform[] shootPoses;
    int currentShootPosIndex;

    [Header("Animation")]
    public Animator headAnimator;
    public Animator[] legAnimators;
    public Transform[] legPoints;

    [Header("Sfx")]
    public PlaySound agroSfx;

    [Header("Detect Graphic")]
    public MeshRenderer[] detectRenderes;
    Material[] detectRenderMats;
    public SpriteRenderer[] detectBloom;
    float currentDetectAlpha;
    public PlayRandomSound shootSfx;

    [Header("Debug")]
    public DecalProjector agroRadiusVisual;

    private void Start()
    {
        currentDetectAlpha = 1f;
        detectRenderMats = new Material[detectRenderes.Length];
        for (int i = 0; i < detectRenderes.Length; i++) {
            detectRenderMats[i] = detectRenderes[i].material;
        }
        UpdateShowDebugAgroRadius();
    }
    public void UpdateShowDebugAgroRadius()
    {
        agroRadiusVisual.gameObject.SetActive(Robot.debugShowAgroRadius);
    }

    private void Update()
    {
        timeSinceLastSeen += Time.deltaTime;
        if (timeSinceLastSeen > 3f)
        {
            Vector3 forward = directionFromLastSeen * Vector3.forward;
            forward.y = 0;
            forward.Normalize();
            Quaternion adjustedRotation = Quaternion.LookRotation(forward, Vector3.up);
            head.localRotation = Quaternion.RotateTowards(head.localRotation, adjustedRotation, rotationSpeed * Time.deltaTime);
        }

        bool agro = false;
        if (!Robot.ghost && !turretHealth.dead){
            Vector3 direction = PlayerTargetInfo.pos - head.position;
            float distanceFromTarget = Vector3.Distance(head.position, PlayerTargetInfo.pos);

            Vector3 directionLos = Cache.mainCam.position - head.position;
            float distanceFromTargetLos = Vector3.Distance(head.position, Cache.mainCam.position);

            if (distanceFromTargetLos < agroRadius){
                if (!Physics.Raycast(head.position, directionLos, distanceFromTargetLos, Cache.references.solidLayerMask)){
                    if (!agrod) { agroSfx.Play(); currentDetectAlpha = 1f; }
                    agrod = true;
                    agro = true;
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    directionFromLastSeen = targetRotation;
                    head.localRotation = Quaternion.RotateTowards(head.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
                    timeSinceLastSeen = 0f;
                }
            }
        }
        if (!agro) { agrod = false; }

        if (currentDetectAlpha > 0.01f)
        {
            currentDetectAlpha = Mathf.Lerp(currentDetectAlpha, 0f, Time.deltaTime * 3f);
            for (int i = 0; i < detectRenderes.Length; i++){
                detectRenderes[i].material.SetColor("_BaseColor", new Color(
                    detectRenderes[i].material.color.r,
                    detectRenderes[i].material.color.g,
                    detectRenderes[i].material.color.b,
                    currentDetectAlpha));
                detectBloom[i].color = new Color(detectBloom[i].color.r, detectBloom[i].color.g, detectBloom[i].color.b, currentDetectAlpha);
            }
        }

        if (timeSinceLastSeen < 0.2f && !turretHealth.dead){
            currentShootCd += Time.deltaTime;
            if (currentShootCd >= shootCd){
                Instantiate(projectile, shootPoses[currentShootPosIndex].position, shootPoses[currentShootPosIndex].rotation);
                Instantiate(muzzleFlash, shootPoses[currentShootPosIndex].position, shootPoses[currentShootPosIndex].rotation);
                currentShootPosIndex++;
                headAnimator.Play("Shoot", -1, 0);
                Animator closest = null;
                Vector3 forward = head.forward;
                for (int i = 0; i < legPoints.Length; i++){
                    Vector3 directionToObj = legPoints[i].position - head.position;
                    float dotProduct = Vector3.Dot(forward, directionToObj.normalized);
                    if (dotProduct < 0){
                        float distance = directionToObj.sqrMagnitude;
                        closest = legAnimators[i];
                        closest.Play("Shoot", -1, 0);
                    }
                }
                shootSfx.Play();

                if (currentShootPosIndex >= shootPoses.Length) { currentShootPosIndex = 0; }
                currentShootCd = 0f;
            }
        }

        agroRadiusVisual.size = new Vector3(agroRadius * 2, agroRadius * 2, 10f);
    }
}