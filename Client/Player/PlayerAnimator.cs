using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
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

    public ScaleManager playerGraphicsScaleManager;
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

    public struct animationVariables
    {
        public float hSpeed;
        public float xTurn;
        float xTurnAbs;
        public bool jumping;
        public bool grounded;
        public bool justGrounded;
        public float sideMove;
        public float forwardMove;
        public float yVelocity;
        public int justJumpedFrames;
        public bool jumpKey;
        public bool vaulted;
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
    private void Start()
    {
        Cache.playerAnimator = this;
        armRotationManager.AddValue("CameraLook", Vector3.zero);
        defaultHeadBonePos = headBone.localPosition;
        StartCoroutine(StartEndFrame());
        vaultingAnimationLength = vaultAnimation.length;
        playerGraphicsScaleManager.AddValue("VaultFlip", Vector3.one);
        slideEndClipLength = slideEndClip.length;
        legRotationManager.AddValue("MoveDirection", Vector3.zero);
        armRotationManager.AddValue("MoveDirection", Vector3.zero);
    }
    IEnumerator StartEndFrame()
    {
        yield return new WaitForEndOfFrame();
        Cache.cameraLocalPositionOffsetManager.AddOffset("AnimationHeadMovement", Vector3.zero);
    }
    private void Update()
    {
        transform.rotation = Cache.surfCharacter.transform.rotation;
        justJumpedFrames++;
        if (Cache.moveData.JustJumped||Cache.vaulter.vaulting) { justJumpedFrames = 0; }

        for (int i = 0; i < animators.Length; i++){
            animators[i].SetFloat("hSpeed", FragMovementListener.hSpeedPercentage);
            animators[i].SetFloat("xTurn", Cache.surfCharacter.turnSpeed.x);
            animators[i].SetFloat("xTurnAbs", Mathf.Abs(Cache.surfCharacter.turnSpeed.x));
            animators[i].SetBool("Jumping", Cache.moveData.JustJumped);
            animators[i].SetBool("Grounded", FragMovementListener.grounded);
            animators[i].SetBool("JustGrounded", FragMovementListener.justGrounded);
            animators[i].SetFloat("SideMove", Cache.moveData.SideMove);
            animators[i].SetFloat("ForwardMove", Cache.moveData.ForwardMove);
            animators[i].SetFloat("yVelocity", Cache.moveData.Velocity.y);
            animators[i].SetInteger("JustJumpedFrames", justJumpedFrames);
            animators[i].SetBool("JumpKey", Input.GetKey(Cache.surfCharacter.JumpButton));
            animators[i].SetBool("Vaulted", Cache.vaulter.vaultFrame);
            animators[i].SetBool("FinishedVaulting", finishedVaultingAnimation);
            animators[i].SetBool("Vaulting", Cache.vaulter.vaulting);
            animators[i].SetBool("VaultActive", vaultActive);
            animators[i].SetFloat("VaultSpeed", (Mathf.Clamp(Cache.vaulter.currentVaultSpeed,vaultSpeedClamp,Mathf.Infinity))* vaultSpeedMultiplier);
            animators[i].SetFloat("TimeSinceLastVault", timeSinceLastVault);
            animators[i].SetFloat("FallingSpeed", fallingSpeed);
            animators[i].SetBool("Wallrunning", Wallrunner.globalWallrunning);
            animators[i].SetFloat("WallrunningSpeed", wallrunningSpeed);
            animators[i].SetBool("Wallclimbing", Cache.wallclimber.wallclimbing);
            animators[i].SetBool("LongJumped", Cache.longJump.longJumped);
            animators[i].SetBool("LongJumping", Cache.longJump.longJumping);
            animators[i].SetFloat("TimeAfterLongJump", Cache.longJump.timeAfterLongJump);
            animators[i].SetBool("StartSlide", Cache.slide.slideStartFrame);
            animators[i].SetBool("EndSlide", Cache.slide.slideEndFrame);
            animators[i].SetBool("SlideEnding", slideEndClipPlaying);
            //animators[i].SetFloat("WallclimbSpeed", Cache.wallclimber.animationTime);
        }
        if (Cache.vaulter.vaultFrame) { StartCoroutine(SetVaultingTimer()); }
        if (Cache.slide.slideEndFrame) { StartCoroutine(SetSlideEndTimer()); }
        finishedVaultingAnimation = false;

        //Arm Y rotation
        if (Cache.vcam.transform.eulerAngles.x < camYArmFollowClamp.x) { targetArmRotation = new Vector3(camYArmFollowClamp.y, 0, 0); }
        else { targetArmRotation = new Vector3(Mathf.Clamp(Cache.vcam.transform.eulerAngles.x, camYArmFollowClamp.x, camYArmFollowClamp.y), 0, 0); }
        armRotationPointer = Vector3.Lerp(armRotationPointer, targetArmRotation, Time.deltaTime * armYRotationSpeed);
        armRotationManager.UpdateValue("CameraLook", armRotationPointer);

        //Leg rotation
        float sideMove = Mathf.Clamp(Cache.moveData.SideMove, -1f, 1f);
        //+(Cache.moveData.ForwardMove >= 0f ? 1.5f : 0f)
        float forwardMultipler = 0f;
        if (Cache.moveData.ForwardMove < 0f) { forwardMultipler = -1f; }
        else if (Cache.moveData.ForwardMove > 0f) { forwardMultipler = 1f; }
        else if (Cache.moveData.ForwardMove == 0f) { forwardMultipler = 1.5f; }
        currentLegRotationAmount = Mathf.Lerp(currentLegRotationAmount, legRotationAmount * ((sideMove*forwardMultipler) *(FragMovementListener.grounded ? 1f : 0.25f)), Time.deltaTime * legRotationSpeed);
        legRotationManager.UpdateValue("MoveDirection", new Vector3(0, currentLegRotationAmount, 0));
        armRotationManager.UpdateValue("MoveDirection", new Vector3(0, currentLegRotationAmount* legArmRotationSpeedMultiplier, 0));

        //Head Bob
        targetHeadBob = (headBone.localPosition - defaultHeadBonePos) * headCamBobAount;
        currentHeadBob = Vector3.Lerp(currentHeadBob, targetHeadBob, Time.deltaTime * headBobSmoothness);
        Cache.cameraLocalPositionOffsetManager.UpdateOffset("AnimationHeadMovement", currentHeadBob);

        timeSinceLastVault += Time.deltaTime;

        fallingSpeed = Mathf.Clamp(Mathf.Abs(Cache.moveData.Velocity.y) / 25f, 1f, 2f);

        wallrunningSpeed = Mathf.Clamp(1*(1+((FragMovementListener.hSpeedPercentage-1)* wallrunningSpeedMultiplier)),1,Mathf.Infinity);

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
