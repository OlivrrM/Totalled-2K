using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class OtherPlayerAnimator : MonoBehaviour
{
    public Animator[] animators;

    public LocalRotationOffsetManager armRotationManager;
    public Vector2 camYArmFollowClamp;
    Vector3 targetArmRotation;
    Vector3 armRotationPointer;
    public float armYRotationSpeed;

    public LocalRotationOffsetManager legRotationManager;
    public float legRotationAmount;
    public float legRotationSpeed;
    public float legArmRotationSpeedMultiplier;
    float currentLegRotationAmount;

    public Transform headBone;
    public Vector3 defaultHeadBonePos;
    public float headCamBobAount;

    int justJumpedFrames;

    bool finishedVaultingAnimation;
    float vaultingAnimationLength;
    public float vaultingAnimationLengthMultiplier = 1f;
    public AnimationClip vaultAnimation;

    bool vaultCheck;
    bool vaultActive;

    //public ScaleManager playerGraphicsScaleManager;
    public float vaultSpeedMultiplier;
    public float vaultSpeedClamp;

    float timeSinceLastVault;

    float fallingSpeed;

    public float wallrunningSpeedMultiplier;
    float wallrunningSpeed;

    Vector3 targetHeadBob;
    Vector3 currentHeadBob;
    public float headBobSmoothness;

    public AnimationClip slideEndClip;
    float slideEndClipLength;
    bool slideEndClipPlaying;
    public struct AnimationVariables
    {
        public float hSpeed;
        public Vector2 xTurnSpeed;
        float xTurnAbs;
        public bool justJumped;
        public bool grounded;
        public bool justGrounded;
        public float sideMove;
        public float forwardMove;
        public float yVelocity;
        public int justJumpedFrames;
        public bool jumpKey;
        public bool vaultFrame;
        public bool finishedVaulting;
        public bool vaulting;
        public bool vaultActive;
        public float vaultSpeed;
        public float timeSinceLastVault;
        public float fallingSpeed;
        public bool wallrunning;
        public float wallrunningSpeed;
        public bool wallclimbing;
        public bool longJumped;
        public bool longJumping;
        public float timeAfterLongJump;
        public bool startSlide;
        public bool endSlide;
        public bool slideEnding;
    }
    public struct MovementVariables
    {
        public float sideMove; // s
        public float forwardMove; // s
        public Vector3 velocity;
        public bool grounded;
        public bool justGrounded;
        public float hSpeedPercentage;
        public bool justJumped;
        public bool vaulting; // s
        public bool vaultFrame;
        public bool jumping; // s
    }
    public AnimationVariables animationVariables;
    public MovementVariables movementVariables;
    private void Start()
    {
        armRotationManager.AddValue("CameraLook", Vector3.zero);
        defaultHeadBonePos = headBone.localPosition;
        vaultingAnimationLength = vaultAnimation.length;
        //playerGraphicsScaleManager.AddValue("VaultFlip", Vector3.one);
        slideEndClipLength = slideEndClip.length;
        legRotationManager.AddValue("MoveDirection", Vector3.zero);
        armRotationManager.AddValue("MoveDirection", Vector3.zero);
    }
    private void Update()
    {
        justJumpedFrames++;
        if (movementVariables.justJumped||movementVariables.vaulting) { justJumpedFrames = 0; }

        for (int i = 0; i < animators.Length; i++){
            animators[i].SetFloat("hSpeed", animationVariables.hSpeed);
            animators[i].SetFloat("xTurn", animationVariables.xTurnSpeed.x);
            animators[i].SetFloat("xTurnAbs", Mathf.Abs(animationVariables.xTurnSpeed.x));
            animators[i].SetBool("Jumping", movementVariables.jumping);
            animators[i].SetBool("Grounded", animationVariables.grounded);
            animators[i].SetBool("JustGrounded", animationVariables.justGrounded);
            animators[i].SetFloat("SideMove", animationVariables.sideMove);
            animators[i].SetFloat("ForwardMove", animationVariables.forwardMove);
            animators[i].SetFloat("yVelocity", animationVariables.yVelocity);
            animators[i].SetInteger("JustJumpedFrames", justJumpedFrames);
            animators[i].SetBool("JumpKey", movementVariables.jumping);
            animators[i].SetBool("Vaulted", animationVariables.vaultFrame);
            animators[i].SetBool("FinishedVaulting", finishedVaultingAnimation);
            animators[i].SetBool("Vaulting", animationVariables.vaulting);
            animators[i].SetBool("VaultActive", vaultActive);
            animators[i].SetFloat("VaultSpeed", (Mathf.Clamp(animationVariables.vaultSpeed, vaultSpeedClamp,Mathf.Infinity))* vaultSpeedMultiplier);
            animators[i].SetFloat("TimeSinceLastVault", timeSinceLastVault);
            animators[i].SetFloat("FallingSpeed", fallingSpeed);
            animators[i].SetBool("Wallrunning", Wallrunner.globalWallrunning);
            animators[i].SetFloat("WallrunningSpeed", wallrunningSpeed);
            animators[i].SetBool("Wallclimbing", animationVariables.wallclimbing);
            animators[i].SetBool("LongJumped", animationVariables.longJumped);
            animators[i].SetBool("LongJumping", animationVariables.longJumping);
            animators[i].SetFloat("TimeAfterLongJump", animationVariables.timeAfterLongJump);
            animators[i].SetBool("StartSlide", animationVariables.startSlide);
            animators[i].SetBool("EndSlide", animationVariables.endSlide);
            animators[i].SetBool("SlideEnding", slideEndClipPlaying);
            //animators[i].SetFloat("WallclimbSpeed", Cache.wallclimber.animationTime);
        }
        if (movementVariables.vaultFrame) { StartCoroutine(SetVaultingTimer()); }
        //if (Cache.slide.slideEndFrame) { StartCoroutine(SetSlideEndTimer()); }
        finishedVaultingAnimation = false;

        //Leg rotation
        float sideMove = Mathf.Clamp(movementVariables.sideMove, -1f, 1f);
        //+(Cache.moveData.ForwardMove >= 0f ? 1.5f : 0f)
        float forwardMultipler = 0f;
        if (movementVariables.forwardMove < 0f) { forwardMultipler = -1f; }
        else if (movementVariables.forwardMove > 0f) { forwardMultipler = 1f; }
        else if (movementVariables.forwardMove == 0f) { forwardMultipler = 1.5f; }
        currentLegRotationAmount = Mathf.Lerp(currentLegRotationAmount, legRotationAmount * ((sideMove*forwardMultipler) *(movementVariables.grounded ? 1f : 0.25f)), Time.deltaTime * legRotationSpeed);
        legRotationManager.UpdateValue("MoveDirection", new Vector3(0, currentLegRotationAmount, 0));
        armRotationManager.UpdateValue("MoveDirection", new Vector3(0, currentLegRotationAmount* legArmRotationSpeedMultiplier, 0));

        timeSinceLastVault += Time.deltaTime;

        fallingSpeed = Mathf.Clamp(Mathf.Abs(movementVariables.velocity.y) / 25f, 1f, 2f);

        wallrunningSpeed = Mathf.Clamp(1*(1+((movementVariables.hSpeedPercentage-1)* wallrunningSpeedMultiplier)),1,Mathf.Infinity);

        //if (!Cache.vaulter.vaulting && vaultCheck) { OnVaultEnd(); }
        //vaultCheck = Cache.vaulter.vaulting;
    }
    IEnumerator SetVaultingTimer()
    {
        vaultActive = true;
        //if (!Cache.vaultBob.vaultCamDirection) { Cache.cameraLocalPositionOffsetManager.UpdateOffset("VaultFlip", new Vector3(-1f, 1f, 1f)); print(Random.RandomRange(1, 99999)); }
        yield return new WaitForSeconds(Mathf.Clamp((vaultingAnimationLength / animators[0].GetFloat("VaultSpeed")) * vaultingAnimationLengthMultiplier, 0f, 5f));
        OnVaultEnd();
        //Cache.cameraLocalPositionOffsetManager.UpdateOffset("VaultFlip", Vector3.one);
    }
    IEnumerator SetSlideEndTimer()
    {
        slideEndClipPlaying = true;
        yield return new WaitForSeconds(slideEndClipLength);
        slideEndClipPlaying = false;
    }
    void OnVaultEnd()
    {
        finishedVaultingAnimation = true;
        vaultActive = false;
        timeSinceLastVault = 0f;
    }
}
