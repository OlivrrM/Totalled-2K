using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    public KeyCode slideKey;

    public bool sliding;

    public float slidingColSize;
    float targetColSize;
    float currentColSize;
    public float slidingSmoothness;

    public float camYNudgeAmount;
    float targetCamYNudge;
    float currentCamYNudge;

    public float slidingBoost;
    float boostDecay;
    public float boostDecayMultiplier;
    public float boostDecaySmoothness;

    float defaultFriction;

    public float velocityIncreaseOnSlide;

    public float slideCooldown;
    float currentSlideCd;

    public float angleMultiplyFactor;

    public float angleJumpOutMultiplier;

    [HideInInspector] public bool slideStartFrame;
    [HideInInspector] public bool slideEndFrame;
    private void Start()
    {
        Cache.slide = this;
        currentColSize = 1f;
        currentCamYNudge = 0f;
        targetColSize = 1f;
        defaultFriction = Cache.surfCharacter.MoveConfig.Friction;
        Cache.playerColliderSizeManager.AddValue("Sliding", Vector3.one);
        Cache.cameraLocalPositionOffsetManager.AddOffset("Sliding", Vector3.zero);
    }
    private void Update()
    {
        slideStartFrame = false;
        slideEndFrame = false;

        if (FragMovementListener.grounded)
        {
            if (Input.GetKey(slideKey) &&
                !sliding &&
                FragMovementListener.hSpeedPercentage > 0.95f)
            {
                StartSliding();
            }
        }
        currentSlideCd -= Time.deltaTime;

        //float angle = Vector3.Angle(Cache.moveData.GroundNormal, Vector3.up) / 100f;
        float angle = Vector3.Dot(Cache.surfCharacter.Forward, Cache.moveData.GroundNormal) * angleMultiplyFactor;
        //print(angle);
        //print(angle+"    "+ angleOld);
        //print(angle + "   " + (Vector3.Angle(Cache.moveData.GroundNormal, Vector3.down - (Cache.surfCharacter.Forward/2f)) / 100f));

        //float slidingNormalAngleMultiplier = Mathf.Clamp((1-angle*2),0f,1f);
        //print(angle);
        //print(angle + "      " + slidingNormalAngleMultiplier);
        if (sliding)
        {
            if (FragMovementListener.localVelocity.z <= 0f ||
                (FragMovementListener.grounded && new Vector2(Cache.moveData.Velocity.x, Cache.moveData.Velocity.z).magnitude < 0.2f) ||
                FragMovementListener.movingBackwards ||
                Input.GetKey(Cache.surfCharacter.JumpButton) ||
                !FragMovementListener.grounded)
            {
                StopSliding(angle);
            }
            
            Cache.moveData.Velocity += ((Cache.surfCharacter.Forward * slidingBoost)* boostDecay) * Time.deltaTime;
            float multiplyAmount = 1f;
            if (angle > 0.01f && angle < 0.15f) { multiplyAmount = 1 - (angle * 6.666f); }
            else if (angle < -0.01f) { multiplyAmount = (Mathf.Abs(angle) + 1); }
            else if (angle >= 0.15f) { multiplyAmount = 0f; }
            boostDecay = Mathf.Lerp(boostDecay, -boostDecaySmoothness, (Time.deltaTime* boostDecayMultiplier) * multiplyAmount);
        }
        //print(1-(((1.2f-(1+(1-angle)))*5)-1));
        //Mathf.Clamp((1 - (((1.2f - (1 + (1 - angle))) * 5) - 1)), 0f, 1f)

        currentColSize = Mathf.Lerp(currentColSize, targetColSize, Time.deltaTime * slidingSmoothness);
        Cache.playerColliderSizeManager.UpdateValue("Sliding", new Vector3(1, currentColSize, 1));

        currentCamYNudge = Mathf.Lerp(currentCamYNudge, targetCamYNudge, Time.deltaTime * slidingSmoothness);
        Cache.cameraLocalPositionOffsetManager.UpdateOffset("Sliding", new Vector3(0, currentCamYNudge, 0));
    }
    void StartSliding()
    {
        sliding = true;
        targetColSize = slidingColSize;
        targetCamYNudge = camYNudgeAmount;
        Cache.surfCharacter.directionalKeysMultipliers = new Vector4(0f, 0f, 0f, 0f);
        Cache.surfCharacter.MoveConfig.Friction = 0;
        Cache.moveData.Velocity += Cache.surfCharacter.Forward * velocityIncreaseOnSlide;
        slideStartFrame = true;
        boostDecay = 1f;
    }
    void StopSliding(float normalAngle)
    {
        sliding = false;
        targetColSize = 1f;
        targetCamYNudge = 0f;
        Cache.surfCharacter.MoveConfig.Friction = defaultFriction;
        Cache.surfCharacter.directionalKeysMultipliers = Vector4.one;
        currentSlideCd = slideCooldown;
        if (Input.GetKey(Cache.surfCharacter.JumpButton) && FragMovementListener.grounded){
            Cache.moveData.Velocity *= 0.5f* (1+(normalAngle* angleJumpOutMultiplier));// + (((1.2f - (1 + (1 - normalAngle))) * 5) - 1);
            //print(0.5f * (1 + (normalAngle * angleJumpOutMultiplier)));
        }
        slideEndFrame = true;
    }
}
