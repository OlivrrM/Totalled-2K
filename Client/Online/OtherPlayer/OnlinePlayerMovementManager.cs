using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class OnlinePlayerMovementManager : MonoBehaviour
{
    public OtherPlayerAnimator otherPlayerAnimator;
    public OtherPlayerGroundedChecker otherPlayerGroundedChecker;

    Vector2 moveDirection;
    bool vaulting;
    bool jumping;

    Vector3 previousPos;
    float hSpeed;
    public float hSpeedMultiplier = 1f;
    public float hSpeedPercentageMultiplier = 1f;
    public float hSpeedXxzxMultiplier = 1f;
    public float fallingSpeedMultiplier = 1f;
    public float vaultSpeed = 1f;
    public float hSpeedDecayMultiplier;

    bool jumpedThisGrounded;
    private void Start()
    {
        previousPos = transform.position;
    }
    public void UpdateMovementMeta(byte movementMeta)
    {
        jumping = Utilities.IntToBool((movementMeta >> 0) & 1);
        vaulting = Utilities.IntToBool((movementMeta >> 1) & 1);

        moveDirection = new Vector2(
            (movementMeta & (1 << 2)) != 0 ? 1 : (movementMeta & (1 << 3)) != 0 ? -1 : 0,
            (movementMeta & (1 << 4)) != 0 ? 1 : (movementMeta & (1 << 5)) != 0 ? -1 : 0
        );
    }
    private void Update()
    {
        //bool grounded = Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.down, 0.525f, Cache.references.solidLayerMask);
        //Debug.DrawRay(transform.position + new Vector3(0f, 0.5f, 0f), Vector3.down, Color.red, 0.525f);
        //print(grounded);

        Vector3 displacement = transform.position - previousPos;
        //hSpeed = Mathf.Abs((new Vector3(displacement.x,0f, displacement.z).magnitude / Time.deltaTime) * hSpeedMultiplier);
        hSpeed = (new Vector3(displacement.x, 0f, displacement.z).magnitude / Time.deltaTime) * hSpeedMultiplier;
        hSpeed = hSpeed > 1f ? 1f + ((hSpeed - 1) * hSpeedXxzxMultiplier) : hSpeed;
        //if (moveDirection == Vector2.zero) { hSpeed = hSpeed < 0.033f?0f:Mathf.Lerp(hSpeed,0f,Time.deltaTime*hSpeedDecayMultiplier); }
        //else { hSpeed = Mathf.Abs(Mathf.Log(new Vector3(displacement.x, 0f, displacement.z).magnitude / Time.deltaTime) * hSpeedMultiplier); }
        previousPos = transform.position;

        otherPlayerAnimator.movementVariables.hSpeedPercentage = (new Vector3(displacement.x, 0, displacement.z).magnitude / (5.76f * Cache.moveData.WalkFactor)) * hSpeedPercentageMultiplier;
        otherPlayerAnimator.animationVariables.hSpeed = hSpeed;
        otherPlayerAnimator.animationVariables.fallingSpeed = displacement.y * fallingSpeedMultiplier;
        otherPlayerAnimator.animationVariables.yVelocity = otherPlayerAnimator.animationVariables.fallingSpeed;

        otherPlayerAnimator.movementVariables.justJumped = false;
        otherPlayerAnimator.animationVariables.justJumped = false;
        otherPlayerAnimator.animationVariables.justJumpedFrames++;
        if (otherPlayerAnimator.movementVariables.grounded && otherPlayerAnimator.movementVariables.jumping && !jumpedThisGrounded){
            otherPlayerAnimator.movementVariables.justJumped = true;
            otherPlayerAnimator.animationVariables.justJumped = false;
            otherPlayerAnimator.animationVariables.justJumpedFrames = 0;
            jumpedThisGrounded = true;
        }
        otherPlayerAnimator.movementVariables.jumping = jumping;
        otherPlayerAnimator.animationVariables.jumpKey = jumping;

        otherPlayerAnimator.movementVariables.vaultFrame = false;
        otherPlayerAnimator.animationVariables.vaultFrame = false;
        otherPlayerAnimator.animationVariables.vaultSpeed = vaultSpeed;
        otherPlayerAnimator.animationVariables.finishedVaulting = false;
        otherPlayerAnimator.animationVariables.timeSinceLastVault += Time.deltaTime;
        if (!otherPlayerAnimator.movementVariables.vaulting){
            otherPlayerAnimator.movementVariables.vaulting = vaulting;
            if (otherPlayerAnimator.movementVariables.vaulting){ 
                otherPlayerAnimator.movementVariables.vaultFrame = true;
                otherPlayerAnimator.animationVariables.vaultFrame = true;
            }
        }
        else{
            if (!otherPlayerAnimator.movementVariables.vaulting){
                otherPlayerAnimator.animationVariables.finishedVaulting = true;
                otherPlayerAnimator.animationVariables.timeSinceLastVault = 0f;
            }
        }
        otherPlayerAnimator.movementVariables.vaulting = vaulting;
        otherPlayerAnimator.animationVariables.vaultActive = vaulting;
        otherPlayerAnimator.animationVariables.vaulting = vaulting;

        otherPlayerAnimator.movementVariables.forwardMove = moveDirection.y;
        otherPlayerAnimator.animationVariables.forwardMove = moveDirection.y;
        otherPlayerAnimator.movementVariables.sideMove = moveDirection.x;
        otherPlayerAnimator.animationVariables.sideMove = moveDirection.x;

        otherPlayerAnimator.movementVariables.justGrounded = false;
        otherPlayerAnimator.animationVariables.justGrounded = false;
        if (!otherPlayerAnimator.movementVariables.grounded){
            otherPlayerAnimator.movementVariables.grounded = otherPlayerGroundedChecker.grounded;
            otherPlayerAnimator.animationVariables.grounded = otherPlayerGroundedChecker.grounded;
            if (otherPlayerAnimator.movementVariables.grounded){
                otherPlayerAnimator.movementVariables.justGrounded = true;
                otherPlayerAnimator.animationVariables.justGrounded = true;
            }
        }
        else { jumpedThisGrounded =  false; }
        otherPlayerAnimator.movementVariables.grounded = otherPlayerGroundedChecker.grounded;
        otherPlayerAnimator.animationVariables.grounded = otherPlayerGroundedChecker.grounded;
    }
}
