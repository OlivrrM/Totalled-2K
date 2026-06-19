using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappler : MonoBehaviour ///UNUSED SCRIPT, LOOK AT SWINGER INSTEAD
{
    public KeyCode bind;
    public float maxDistance;
    public float minDistance;
    public float grappleSpeed;

    public LineRenderer lineRenderer;
    float distance;

    Transform playerTransform;

    public LayerMask layerMask;

    public 

    bool shooting;
    bool attached;
    bool pulling;
    Vector3 attachedPoint;

    public Transform grappleTargetPoint;
    public Transform grappleDirection;

    Vector3 graphicPos;

    float timeShooting;

    public float maxDistancePush;

    float targetDistance;

    Vector3 up;

    public float ropeGraphicStartOffset = -0.25f;

    [Header("Rope Graphics")]
    public AnimationCurve effectCurve;
    public float waveHeight;
    float waveCount;
    public Vector2 waveCountAB;
    float ropeSwingDecay;
    public float ropeSwingDecaySpeed;
    public float ropeDetachSpeed;
    float ropeDetachSpeedMultiplier;

    [Header("Swinging")]
    public float pushForce;
    public float distancePushMultiplier;
    
    private void Start()
    {
        playerTransform = Cache.surfCharacter.transform;
        lineRenderer.useWorldSpace = true;

        grappleTargetPoint.SetParent(null);
    }
    private void Update()
    {
        if (Input.GetKeyDown(bind))
        {
            if (shooting) { OnDetach(); }
            else if (!shooting && !pulling) { shooting = true; }
            if (pulling) { pulling = false; }
            if (shooting)
            {
                ropeSwingDecay = 1f;
                waveCount = Utilities.RandomExclusiveListRange(waveCountAB.x, waveCountAB.y, new List<float> { -1f,0f,1f});
                RaycastHit hit;
                if (Physics.Raycast(transform.position+new Vector3(0,-0.25f,0),transform.forward, out hit, Mathf.Infinity, layerMask)){
                    grappleTargetPoint.position = hit.point;
                }
                else{
                    grappleTargetPoint.position = transform.forward * 999f;
                    //Debug.DrawRay(transform.position, transform.forward * 999f, Color.red,3f);
                }
            }
        }
        if (shooting){
            lineRenderer.gameObject.SetActive(true);
            timeShooting += Time.deltaTime;
            distance += Time.deltaTime * grappleSpeed;
            grappleDirection.LookAt(grappleTargetPoint);

            RaycastHit hit;
            if (!attached)
            {
                if (Physics.Raycast(Cache.mainCam.position + new Vector3(0, -0.25f, 0), Cache.mainCam.forward, out hit, distance, layerMask))
                {
                    attached = true;
                    attachedPoint = hit.point;
                    OnAttach();
                    pulling = true;
                }
            }
            graphicPos = (transform.position+(grappleDirection.forward*distance))+ new Vector3(0, -0.25f, 0);
            if (attached) { graphicPos = attachedPoint; }
            lineRenderer.SetPosition(0, transform.position + new Vector3(0, ropeGraphicStartOffset, 0));
            lineRenderer.SetPosition(lineRenderer.positionCount-1, graphicPos);

            if (Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos) >= maxDistance){
                OnDetach();
            }

            if (pulling){
                /*
                if (Vector3.Distance(transform.position + new Vector3(0, -0.25f, 0), graphicPos) < targetDistance){
                    Cache.moveData.BaseVelocity -= Cache.velocityManager.PushTowards(attachedPoint) * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos), 0, maxDistancePush) * distancePushMultiplier) * Time.deltaTime;
                }
                else{
                    Cache.moveData.BaseVelocity += Cache.velocityManager.PushTowards(attachedPoint) * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos), 0, maxDistancePush) * distancePushMultiplier) * Time.deltaTime;
                }
                */
                Cache.moveData.BaseVelocity += Cache.velocityManager.PushTowards(attachedPoint) * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos), 0, maxDistancePush) * distancePushMultiplier) * Time.deltaTime;
                //SwingPlayer();
                //ApplySwingingForce();
                if (Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos) <= minDistance){
                    OnDetach();
                }
            }

            if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0),grappleDirection.forward, Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos)*0.75f, layerMask))
            {
                OnDetach();
            }
        }
        else
        {
            timeShooting = 0;
            distance = 0;
            attached = false;
            if (Vector3.Distance(graphicPos, transform.position + new Vector3(0, -0.25f, 0)) > 0.5f){
                graphicPos = Vector3.Lerp(graphicPos, transform.position + new Vector3(0, -0.25f, 0), Time.deltaTime * (ropeDetachSpeed * ropeDetachSpeedMultiplier));
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, graphicPos);
                ropeDetachSpeedMultiplier += Time.deltaTime * 5;
            }
            else { 
                graphicPos = transform.position + new Vector3(0, -0.25f, 0);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, graphicPos);
            }
            lineRenderer.gameObject.SetActive(false);
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }
    /*
    void SwingPlayer() //Written by chatGPT
    {
        Vector3 grappleDirection = (attachedPoint - Cache.surfCharacter.transform.position).normalized;
        Vector3 perpendicularDirection = new Vector3(-grappleDirection.y, grappleDirection.x, 0f); // Perpendicular to grappleDirection in the XY plane

        // Calculate tangential direction to the grapple point
        Vector3 tangentialDirection = Vector3.Cross(Vector3.forward, grappleDirection);

        // Calculate the component of the player's velocity in the tangential direction
        float tangentialVelocity = Vector3.Dot(Cache.moveData.BaseVelocity, tangentialDirection);

        // Apply swinging force based on the tangential component of the player's velocity
        Vector3 swingForce = tangentialDirection * Mathf.Clamp(tangentialVelocity, -maxSwingVelocity, maxSwingVelocity) * swingForceMultiplier;
        Cache.moveData.BaseVelocity += swingForce;

        // Apply pulling force towards the grapple point
        Vector3 pullForce = grappleDirection * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, attachedPoint), 0, maxDistancePush) * distancePushMultiplier);
        Cache.moveData.BaseVelocity += pullForce;
    }
    */
    void OnDetach()
    {
        shooting = false; 
        pulling = false;
        ropeDetachSpeedMultiplier = 1f;
    }
    void OnAttach()
    {
        targetDistance = Vector3.Distance(transform.position + new Vector3(0, -0.25f, 0), graphicPos);
        //Cache.moveData.BaseVelocity += Cache.velocityManager.PushTowards(attachedPoint) * pushForce;
    }
    void DrawRope()
    {
        if (attached) { ropeSwingDecay = Mathf.Lerp(ropeSwingDecay, 0f, Time.deltaTime * ropeSwingDecaySpeed*3); }
        else { ropeSwingDecay = Mathf.Lerp(ropeSwingDecay, 0f, Time.deltaTime * ropeSwingDecaySpeed); }
        up = Quaternion.LookRotation((grappleTargetPoint.position - (transform.position + new Vector3(0, -0.25f, 0))).normalized) * Vector3.right;
        for (var i = 0; i < lineRenderer.positionCount - 1; i++){
            float delta = i / (float)(lineRenderer.positionCount - 1);
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * Vector3.Distance(transform.position + new Vector3(0, -0.25f, 0),graphicPos) * ropeSwingDecay * effectCurve.Evaluate(delta);
            lineRenderer.SetPosition(i, Vector3.Lerp(transform.position + new Vector3(0, -0.25f, 0), graphicPos, delta) + offset);
        }
    }
} ///UNUSED USE 'Swinger.cs'