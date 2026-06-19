using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectJumpBob : MonoBehaviour
{
    public LocalOffsetManager localOffsetManager;

    //public AnimationCurve bounceCurve;
    float curvePointer;
    float pushDownwardsSpeed;
    float oscillatingValue;
    bool isMovingToNegativeOne = false;

    public float bounceAmount;
    float currentBounceAmount;
    public float bounceSpeed;
    public float gravity;
    public float maxBounceVelocity;
    public float maxBounceAmount;
    public Vector2 hitVelocityRange;

    float pointer;
    float velocity;
    float velocityOnBounce;
    //float currentVelocity;
    //public float velocityLerpSpeed;

    float y;
    float currentY;
    public float lerpSpeed;

    public PlayRandomSound bounceSfx;

    CarAlarmTrigger carAlarmTrigger;
    private void Start()
    {
        localOffsetManager.AddValue("ObjectJumpBob", Vector3.zero);
        carAlarmTrigger = gameObject.GetComponent<CarAlarmTrigger>();
    }
    public void Bounce(float hitVelocity, Vector3 point)
    {
        float force = Mathf.InverseLerp(hitVelocityRange.x, hitVelocityRange.y, Mathf.Clamp(Mathf.Abs(hitVelocity), hitVelocityRange.x, hitVelocityRange.y));
        currentBounceAmount = bounceAmount * (1+ force);
        velocity += force * currentBounceAmount;
        velocityOnBounce = force * currentBounceAmount;
        pushDownwardsSpeed = Mathf.Clamp(force,0.33f,1f) * 5f;
        isMovingToNegativeOne = true;
        if (point != Vector3.zero) 
        {
            bounceSfx.transform.position = point;
            float pitch = 0.72f + ((1 - force) * 0.75f);
            bounceSfx.Play(pitch, pitch, force * 2);
        }

        if (carAlarmTrigger != null) { carAlarmTrigger.Trigger(); }
    }
    private void Update()
    {
        if (velocity > 0.001f)
        {
            curvePointer = CalculateCurvature();

            //currentVelocity = Mathf.Lerp(currentVelocity, velocity, Time.deltaTime * velocityLerpSpeed);
            //y = curvePointer * currentVelocity;
            y = curvePointer * velocity;
            y = Mathf.Clamp(y, 0f, maxBounceAmount);
            currentY = Mathf.Lerp(currentY, y, Time.deltaTime * lerpSpeed);
            if (!isMovingToNegativeOne)
            {
                velocity = Mathf.Lerp(velocity, 0f, Time.deltaTime * gravity);
                velocity = Mathf.Clamp(velocity, 0f, maxBounceVelocity);
            }
            localOffsetManager.UpdateValue("ObjectJumpBob", new Vector3(0f, -currentY, 0f));
        }
    }
    float CalculateCurvature()
    {
        if (isMovingToNegativeOne){
            oscillatingValue = Mathf.MoveTowards(oscillatingValue, -1,bounceSpeed* pushDownwardsSpeed * Time.deltaTime);
            if (Mathf.Approximately(oscillatingValue, -1)){
                isMovingToNegativeOne = false;
            }
        }
        else{
            oscillatingValue = Mathf.Sin(Time.time * bounceSpeed);
        }
        return (oscillatingValue + 1)/2f;
    }
}
