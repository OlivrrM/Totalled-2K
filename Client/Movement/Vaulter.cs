using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaulter : MonoBehaviour
{
    public LayerMask layerMask;
    public Transform vaultCheckPos;
    public Transform vaultTopCheckPos;
    public float vaultCheckDistance;

    public float vaultPower;
    public float forwardPower;

    [HideInInspector] public bool vaulting;
    public float vaultSpeed;
    [HideInInspector] public float currentVaultSpeed;
    public float vaultDistance;
    public float vaultHeight;

    public float surfClipAmountSprintingClamp;
    public float surfClipAmountWalkingClamp;

    float vaultCd;
    float enableColCd;

    Vector3 targetVaultPos;
    Vector3 startedVaultPos;

    float targetVaultPosHeightIncrement;
    float targetVaultPosNormalHeightIncrement;

    [HideInInspector] public Vector3 targetVaultNormal;

    public float vaultSmoothness;

    GameObject vaultPosCache;

    float vaultTime;
    public float minVaultSpeed;

    Vector3 velocityOnVault;

    public float vaultYBoostMultiplier;

    [HideInInspector] public bool vaultFrame;
    [HideInInspector] public bool finishVaultFrame;

    public GameObject TEST;

    private void Start()
    {
        Cache.vaulter = this;
        Cache.playerColliderSizeManager.AddValue("Vaulter", Vector3.one);
        vaultCheckPos.position = Cache.surfCharacter.transform.position;
        vaultTopCheckPos.position = Cache.surfCharacter.transform.position + new Vector3(0, Cache.surfCharacter.ColliderSize.y, 0);
        vaultCd = -1f;
        vaultPosCache = new GameObject("VaultPosCache");
    }
    private void Update()
    {
        vaultFrame = false;
        finishVaultFrame = false;
        if (InputManager.GetVaultKeyDown() && vaultCd<0f && new Vector3(Cache.moveData.PreJumpGroundNormal.x, 0, Cache.moveData.PreJumpGroundNormal.z).magnitude < 0.025f)
        {
            RaycastHit hit = VaultCheckCast(5);
            if (hit.transform!=null)
            {
                vaultTopCheckPos.position = new Vector3(hit.point.x, vaultTopCheckPos.position.y, hit.point.z);
                RaycastHit hitTop;
                if (Physics.Raycast(vaultTopCheckPos.position, vaultTopCheckPos.forward, out hitTop, Cache.surfCharacter.ColliderSize.y, layerMask)){
                    if (!FragMovementListener.surfing||(FragMovementListener.surfing&& (new Vector3(hitTop.normal.x, 0, hitTop.normal.z).magnitude < Fragsurf.Movement.SurfPhysics.SurfSlope))) ///THIS NEEDS TO CHECK IF PLAYER IS SURFING INSTEAD    old --> (new Vector3(hitTop.normal.x, 0, hitTop.normal.z).magnitude < Fragsurf.Movement.SurfPhysics.SurfSlope)&&
                    {
                        startedVaultPos = Cache.surfCharacter.transform.position;
                        targetVaultPos = hitTop.point + (Cache.surfCharacter.transform.forward * vaultDistance) + (Vector3.up * vaultHeight);
                        //Instantiate(TEST, targetVaultPos, Quaternion.identity);
                        //if (!Physics.BoxCast(targetVaultPos+ new Vector3(0f,0.85f,0f) + new Vector3(0f,Cache.surfCharacter.ColliderSize.y/2f,0f),new Vector3(Cache.surfCharacter.ColliderSize.x/2f, Cache.surfCharacter.ColliderSize.y / 2f, Cache.surfCharacter.ColliderSize.z / 2f), Vector3.up, Cache.surfCharacter.transform.rotation)){
                        targetVaultNormal = hitTop.normal;
                        if (FragMovementListener.grounded) { targetVaultPosHeightIncrement = 0f; targetVaultPosNormalHeightIncrement = 0f; }
                        else { targetVaultPosHeightIncrement = 0.5f; targetVaultPosNormalHeightIncrement = new Vector2(targetVaultNormal.x, targetVaultNormal.z).magnitude; }
                        //Instantiate(TEST, new Vector3(targetVaultPos.x, targetVaultPos.y + Cache.surfCharacter.ViewOffset.y + targetVaultPosHeightIncrement + targetVaultPosNormalHeightIncrement, targetVaultPos.z), Quaternion.identity);
                        Vector3 targetViewCheckPos = new Vector3(targetVaultPos.x, targetVaultPos.y + Cache.surfCharacter.ViewOffset.y + targetVaultPosHeightIncrement + targetVaultPosNormalHeightIncrement, targetVaultPos.z);
                        Vector3 direction = (targetViewCheckPos-Cache.mainCam.position).normalized;
                        if (!Physics.Raycast(Cache.mainCam.position, direction, Vector3.Distance(Cache.mainCam.position, targetViewCheckPos), Cache.references.solidLayerMask)){
                            if (!Physics.Raycast(targetViewCheckPos, Vector3.up, Cache.surfCharacter.ViewOffset.y / 2f, Cache.references.solidLayerMask)) {
                                OnVault(Cache.surfCharacter.ColliderSize.y - Vector3.Distance(vaultTopCheckPos.position, hitTop.point));
                            }
                        }
                        //}
                    }
                }
                vaultTopCheckPos.position = Cache.surfCharacter.transform.position + new Vector3(0, Cache.surfCharacter.ColliderSize.y, 0);
            }
        }
        enableColCd -= Time.deltaTime;
        /*
        float multiplier = 1f;
        if (enableColCd > 0) { multiplier = 0.1f; }
        else { multiplier = 1 - Mathf.Clamp(vaultCd+0.5f, 0.1f, 1); print(multiplier); }
        Cache.playerColliderSizeManager.UpdateValue("Vaulter", new Vector3(multiplier, multiplier, multiplier));
        */

        /*
        if (vaultCd > 0) ///Dont 01 lerp to target pos, do smooth lerp but check for greater than target y pos
        {
            vaultCd -= Time.deltaTime* currentVaultSpeed;
            currentVaultSpeed += Time.deltaTime*vaultIncreaseSpeed;
            //Cache.surfCharacter.transform.position = Vector3.Lerp(Cache.surfCharacter.transform.position, targetVaultPos, Time.deltaTime * vaultSpeed);
            Cache.surfCharacter.transform.position = Vector3.Lerp(targetVaultPos, startedVaultPos, vaultCd);
            if (vaultCd < 0)
            {
                Cache.surfCharacter.transform.position = targetVaultPos;
                FragMovementManager.SetActive(true);
                Cache.moveData.Velocity = delayedCachedVel;
            }
        }
        */
        vaultCd -= Time.deltaTime;
        //print(Cache.colliderCornerCaster.SurfClippingAmount());

        /*
        if (Input.GetKeyDown(KeyCode.F)) { Cache.surfCharacter.Collider.enabled = false; collisionBoxResetFrame = -5; }
        if (collisionBoxResetFrame < 0){
            collisionBoxResetFrame++;
            if (collisionBoxResetFrame >= 0){
                Cache.surfCharacter.Collider.enabled = true;
            }
        }
        */

        if (vaulting)
        {
            float surfClipAmount = Mathf.Clamp(Cache.colliderCornerCaster.SurfClippingAmount().amount,0f,FragMovementListener.hSpeedPercentage > 1.2f ? surfClipAmountSprintingClamp : surfClipAmountWalkingClamp);
            vaultTime += Time.deltaTime * currentVaultSpeed;
            Cache.surfCharacter.transform.position = Vector3.Lerp(Cache.surfCharacter.transform.position, new Vector3(targetVaultPos.x, targetVaultPos.y + targetVaultPosHeightIncrement+0.1f + surfClipAmount+ targetVaultPosNormalHeightIncrement, targetVaultPos.z), Time.deltaTime * currentVaultSpeed);
            if ((Cache.surfCharacter.transform.position.y >= (targetVaultPos.y + surfClipAmount + targetVaultPosHeightIncrement + targetVaultPosNormalHeightIncrement) && Mathf.Abs(Cache.surfCharacter.transform.position.x) >= (Mathf.Abs(targetVaultPos.x) - 0.1f) && Mathf.Abs(Cache.surfCharacter.transform.position.z) >= (Mathf.Abs(targetVaultPos.z) - 0.1f)))
            {
                ///TO FIX INCORRECT SURF AFTER VAULT:
                ///Check if every raycasthit within SurfController (880) is shit (not just 1, they all have to be shit) AND the player is grounded
                ///Im not sure how to actually fix the inconsistancy just move the dude around
                
                ///If player is grounded lerp pos to normal pos, if !grounded lerp to pos + 0.5f(y)

                //Cache.surfCharacter.transform.position = targetVaultPos;
                FragMovementManager.SetActive(true);
                Cache.moveData.Velocity = Cache.velocityManager.ChangeDirection(new Vector3(velocityOnVault.x, (targetVaultPosHeightIncrement + 0.1f + surfClipAmount + targetVaultPosNormalHeightIncrement)* vaultYBoostMultiplier, velocityOnVault.z), Cache.surfCharacter.transform.forward);
                //Cache.moveData.Velocity = delayedCachedVel;
                vaulting = false;
                finishVaultFrame = true;
                FragMovementListener.justJumped = false;
            }
        }
    }
    RaycastHit VaultCheckCast(int checkQuality)
    {
        RaycastHit hit;
        for (int i = 0; i < checkQuality; i++){
            Debug.DrawRay(vaultCheckPos.position + new Vector3(0, ((Cache.surfCharacter.ColliderSize.y / (float)checkQuality) * i), 0), vaultCheckPos.forward,Color.red,4f);
            if (Physics.Raycast(vaultCheckPos.position +new Vector3(0,((Cache.surfCharacter.ColliderSize.y / (float)checkQuality)*i),0), vaultCheckPos.forward, out hit, vaultCheckDistance, layerMask)){
                return hit;
            }
        }
        return new RaycastHit();
    }
    void OnVault(float depth)
    {
        //print(depth);
        //Cache.moveData.Velocity.y = 0;
        if (Cache.wallclimber != null) { Cache.wallclimber.EndWallclimb(); }
        FragMovementManager.SetActive(false);
        velocityOnVault = Cache.velocityManager.GetVelocityFromPast(0.2f);
        currentVaultSpeed = Mathf.Clamp(new Vector3(velocityOnVault.x,0, velocityOnVault.z).magnitude, minVaultSpeed,Mathf.Infinity) *vaultSpeed;
        //Cache.playerColliderSizeManager.UpdateValue("Vaulter", new Vector3(1, 0.1f, 1));
        //Cache.moveData.Velocity += new Vector3(0, depth * vaultPower,0);
        //Cache.moveData.Velocity += Cache.surfCharacter.transform.forward * forwardPower;
        vaultCd = 1f;
        enableColCd = 0.5f;
        vaulting = true;
        Cache.moveData.Velocity = Vector3.zero;
        //if (FragMovementListener.grounded) { targetVaultPosHeightIncrement = 0f; targetVaultPosNormalHeightIncrement = 0f; }
        //else { targetVaultPosHeightIncrement = 0.5f; targetVaultPosNormalHeightIncrement = new Vector2(targetVaultNormal.x, targetVaultNormal.z).magnitude; }
        vaultPosCache.transform.SetPositionAndRotation(Cache.surfCharacter.transform.position, Cache.surfCharacter.transform.rotation);
        vaultTime = 0;
        vaultFrame = true;
        FragMovementListener.justJumped = false;
    }
}
