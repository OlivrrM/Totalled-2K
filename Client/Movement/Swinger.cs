using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swinger : MonoBehaviour
{
    public KeyCode bind;
    public float maxDistance;
    public float minDistance;
    public float grappleSpeed;

    public LineRenderer lineRenderer;
    float distance;

    public LayerMask layerMask;

    public 

    bool shooting;
    [HideInInspector] public bool attached;
    [HideInInspector] public bool pulling;
    Vector3 attachedPoint;

    public Transform grappleTargetPoint;
    public Transform grappleDirection;

    Vector3 graphicPos;

    float timeShooting;

    public float maxDistancePush;

    float targetDistance;

    Vector3 up;

    public float wallbangFactor = 0.75f;

    bool grappleTargetPointLocked;

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
    public float spring;
    public float damper;
    public float massScale;

    SpringJoint joint;
    Vector3 playerPosOnAttach;

    [Header("Camera Work")]
    [HideInInspector] public Vector3 swingingDirection;
    Vector3 targetCamDirection;
    Vector3 currentCamDirection;
    public float camAttachDirectionSpeed;
    public float camDetachDirectionSpeed;

    [Header("Cursor")]
    bool cursorEnabled;
    public RectTransform cursorUiGraphic;
    Vector3 cursorPos;
    Vector3 cursorGraphicPos;
    public Transform nullScreenPos;
    public Image[] cursorImages;
    public Color cursorLockedColor;
    public Color cursorUnlockedColor;
    Color currentCursorColor;

    public RectTransform[] cursorRects;
    public float attatchedCursorWideness;
    public float unattachedCursorWideness;
    float currentCursorWideness;
    float targetCursorWideness;
    public float cursorWidenessSpeed;

    public float shootingCursorSize;
    public float notShootingCursorSize;
    float currentCursorSize;
    float targetCursorSize;
    public float cursorSizeSpeed;

    public float cursorChangeColorSpeed;
    bool cursorLocked;

    public float cursorSmoothness;

    [Header("Sound Effects")]
    public PlaySound shootSfx;
    public GameObject LandSfx;
    public PlaySound shootingSfx;
    float targetShootSfxVolume;

    public bool cantBeGrounded;

    bool castHit;
    RaycastHit hit;
    private void OnDisable()
    {
        for (int i = 0; i < cursorRects.Length; i++){
            cursorRects[i].localScale = Vector3.zero;
        }
    }
    private void Start()
    {
        Cache.swinger = this;

        //grappleCursor.transform.SetParent(null);
        //grappleCursor.transform.position = new Vector3(0f, -999f, 0f);

        lineRenderer.useWorldSpace = true;

        grappleTargetPoint.SetParent(null);

        Cache.cameraWorldSpaceRotationOffsetManager.AddOffset("Swinger",Vector3.zero);
    }
    private void Update()
    {
        if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0), transform.forward, out hit, Mathf.Infinity, layerMask)){
            castHit = true;
        }
        else{
            castHit = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!shooting && !pulling) 
            {
                if (!cantBeGrounded) { OnStartShooting(); }
                else if (cantBeGrounded && !FragMovementListener.grounded) { OnStartShooting(); }
            }
            if (shooting)
            {
                ropeSwingDecay = 1f;
                waveCount = Utilities.RandomExclusiveListRange(waveCountAB.x, waveCountAB.y, new List<float> { -2f,-1f, 0f, 1f,2f });
                if (castHit)
                {
                    grappleTargetPoint.position = hit.point;
                    grappleTargetPointLocked = true;
                }
                else
                {
                    grappleTargetPoint.position = transform.forward * 9999999f;
                    grappleTargetPointLocked = false;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnDetach();
            if (pulling) { pulling = false; }
        }



        /*
        if (!FragMovementListener.grounded && !attached) 
        {
            grappleCursor.transform.localScale = Vector3.Lerp(grappleCursor.transform.localScale, Vector3.one, Time.deltaTime * 10);
            EnableCursor();
        }
        else { 
            grappleCursor.transform.localScale = Vector3.Lerp(grappleCursor.transform.localScale, Vector3.zero, Time.deltaTime * 10);
            DisableCursor();
        }
        RaycastHit cursorHit;
        if (!shooting)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0), transform.forward, out cursorHit, maxDistance, layerMask)){
                grappleCursor.transform.position = cursorHit.point; EnableCursor();
            }
            else { grappleCursor.transform.localScale = Vector3.Lerp(grappleCursor.transform.localScale, Vector3.zero, Time.deltaTime * 10); DisableCursor(); }
        }
        else
        {
            if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0), grappleDirection.forward, out cursorHit, maxDistance, layerMask)){
                grappleCursor.transform.position = cursorHit.point; EnableCursor();
            }
            else { grappleCursor.transform.localScale = Vector3.Lerp(grappleCursor.transform.localScale, Vector3.zero, Time.deltaTime * 10); DisableCursor(); }
        }
        */

        if (shooting){
            timeShooting += Time.deltaTime;
            distance += Time.deltaTime * grappleSpeed;
            grappleDirection.LookAt(grappleTargetPoint);

            RaycastHit hit;
            if (!attached)
            {
                if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0), grappleDirection.forward, out hit, distance, layerMask))
                {
                    attached = true;
                    attachedPoint = hit.point;
                    OnAttach();
                    pulling = true;
                }
            }
            graphicPos = (transform.position+(grappleDirection.forward*distance))+ new Vector3(0, -0.25f, 0);
            if (attached) { graphicPos = attachedPoint; }
            lineRenderer.SetPosition(0, transform.position + new Vector3(0, -0.25f, 0));
            lineRenderer.SetPosition(lineRenderer.positionCount-1, graphicPos);
            shootingSfx.transform.position = graphicPos;

            if (Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos) >= maxDistance){
                OnDetach();
            }
            if (cantBeGrounded && FragMovementListener.grounded)
            {
                OnDetach();
            }

            if (pulling){

                //Get direction towards the where the grapple is attatched to
                Vector3 playerPosition = Cache.surfCharacter.gameObject.transform.position;
                swingingDirection = graphicPos - playerPosition;
                //swingingDirection.Normalize(); // Normalize the vector to get a unit vector
                targetCamDirection = new Vector3(swingingDirection.z, 0,-swingingDirection.x);
                currentCamDirection = Vector3.Lerp(currentCamDirection, targetCamDirection, Time.deltaTime * camAttachDirectionSpeed);

                Vector3 direction = Cache.rb.velocity.normalized;
                //Cache.rb.AddForce(Cache.velocityManager.PushTowards(direction) * pushForce * Time.deltaTime);
                Debug.DrawRay(Cache.surfCharacter.transform.position, direction * 3f, Color.red);

                //Cache.moveData.BaseVelocity += Cache.velocityManager.PushTowards(attachedPoint) * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos), 0, maxDistancePush) * distancePushMultiplier) * Time.deltaTime;

                if (Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos) <= minDistance){
                    OnDetach();
                }
            }
            else
            {
                currentCamDirection = Vector3.Lerp(currentCamDirection, targetCamDirection, Time.deltaTime * camDetachDirectionSpeed);
                targetCamDirection = Vector3.zero;
            }

            if (Physics.Raycast(transform.position + new Vector3(0, -0.25f, 0),grappleDirection.forward, Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos)* wallbangFactor, layerMask))
            {
                OnDetach();
            }
        }
        else
        {
            targetCamDirection = Vector3.zero;
            currentCamDirection = Vector3.Lerp(currentCamDirection, targetCamDirection, Time.deltaTime * camDetachDirectionSpeed);
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
        }
        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("Swinger", currentCamDirection);
        shootSfx.SFX.volume = Mathf.Lerp(shootSfx.SFX.volume, targetShootSfxVolume, Time.deltaTime * 5f);

        ///Cursor
        //grappleCursor.transform.position = Vector3.Lerp(grappleCursor.transform.position, cursorPos, Time.deltaTime * cursorGraphicSpeed);
        //Vector3 cursorScreenPosition = Camera.main.WorldToScreenPoint(cursorGraphicPos);
        //cursorUiGraphic.position = cursorScreenPosition;
        //cursorGraphicPos = Vector3.Lerp(cursorGraphicPos, cursorPos, Time.deltaTime * cursorGraphicSpeed);
        float tWideness = 0f;
        if (!shooting)
        {
            if (castHit)
            {
                cursorPos = hit.point;
                if (hit.distance < maxDistance) { cursorLocked = true; }
                else { cursorLocked = false; }
            }
            else
            {
                cursorPos = nullScreenPos.position;
                cursorLocked = false;
            }
            targetCursorSize = notShootingCursorSize;
        }
        else
        {
            if (attached)
            {
                cursorPos = attachedPoint;
                cursorLocked = true;
                targetCursorWideness = attatchedCursorWideness;
                tWideness = 1f;
            }
            else
            {
                if (!grappleTargetPointLocked)
                {
                    cursorPos = grappleTargetPoint.position;
                    cursorPos = transform.position + Vector3.ClampMagnitude(cursorPos, maxDistance);
                }
                else
                {
                    tWideness = Utilities.NormalizedDistance(transform.position, cursorPos, graphicPos);
                }
            }
            targetCursorSize = shootingCursorSize;
        }
        currentCursorSize = Mathf.Lerp(currentCursorSize, targetCursorSize, cursorSizeSpeed);
        currentCursorWideness = Mathf.Lerp(unattachedCursorWideness, attatchedCursorWideness, tWideness);
        for (int i = 0; i < cursorRects.Length; i++)
        {
            cursorRects[i].localScale = Utilities.V3All(currentCursorSize);
            cursorRects[i].localPosition = Vector3.Lerp(cursorRects[i].localPosition, new Vector3(cursorRects[i].localPosition.x, currentCursorWideness, cursorRects[i].localPosition.z), cursorWidenessSpeed);
        }
        //currentCursorWideness = Mathf.Lerp(currentCursorWideness, targetCursorWideness, Time.deltaTime * cursorWidenessSpeed);
        cursorGraphicPos = Vector3.Lerp(cursorGraphicPos, cursorPos, Time.deltaTime * cursorSmoothness);
        if (cursorLocked) { currentCursorColor = Color.Lerp(currentCursorColor, cursorLockedColor, cursorChangeColorSpeed); }
        else { currentCursorColor = Color.Lerp(currentCursorColor, cursorUnlockedColor, cursorChangeColorSpeed); }
        for (int i = 0; i < cursorImages.Length; i++)
        {
            cursorImages[i].color = currentCursorColor;
        }
        Vector3 cursorScreenPosition = Camera.main.WorldToScreenPoint(cursorGraphicPos);
        cursorUiGraphic.position = cursorScreenPosition;
        //cursorGraphicPos = cursorPos;
        //grappleCursor.transform.position = cursorGraphicPos;
        //grappleCursor.transform.position = cursorPos;
    }
    /*
    void EnableCursor()
    {
        if (!cursorEnabled){
            cursorEnabled = true;
            cursorRotator.AddForce(360f);
        }
    }
    void DisableCursor()
    {
        if (cursorEnabled){
            cursorEnabled = false;
            cursorRotator.AddForce(-360f);
        }
    }
    */
    private void LateUpdate()
    {
        DrawRope();
    }
    private void FixedUpdate()
    {
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
    void OnStartShooting()
    {
        shooting = true;
        shootSfx.Play();
        targetShootSfxVolume = 0.75f;
        shootSfx.SFX.volume = 0.75f;
        shootingSfx.SFX.volume = 1f;
        shootingSfx.RefreshPitch();
    }
    public void OnDetach()
    {
        shooting = false; 
        pulling = false;
        ropeDetachSpeedMultiplier = 1f;

        if (joint != null){
            Destroy(joint);
            Cache.fragMovementManager.SetRigidbody(false);
        }

        shootingSfx.SFX.volume = 0f;
        targetShootSfxVolume = 0f;
    }
    void OnAttach()
    {
        playerPosOnAttach = Cache.surfCharacter.transform.position;
        targetDistance = Vector3.Distance(transform.position + new Vector3(0, -0.25f, 0), graphicPos);

        //Cache.moveData.Velocity += Cache.velocityManager.PushTowards(attachedPoint) * pushForce * (Mathf.Clamp(Vector3.Distance(Cache.surfCharacter.transform.position, graphicPos), 0, maxDistancePush) * distancePushMultiplier);

        Cache.fragMovementManager.SetRigidbody(true);

        joint = Cache.surfCharacter.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = graphicPos;

        //joint.maxDistance = targetDistance * 0.8f;
        //joint.minDistance = targetDistance * 0.25f;

        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        Instantiate(LandSfx, graphicPos, Quaternion.identity);
        shootingSfx.SFX.volume = 0f;
        shootSfx.SFX.Stop();

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
}