using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour
{
    public LayerMask layerMask;
    public KeyCode shift;
    public float jumpHeightIncrease;

    float shiftTime;
    public float shiftDecayTime;

    public float velocityIncrease;

    public CameraBobManager cameraBobManager;
    public float bobAmount;

    public float timeSinceLastLongJump;

    public bool longJumped;
    public bool longJumping;
    public float longJumpingTime; //Used for animator
    public float timeAfterLongJump;

    public float ledgePowerIncrease;
    private void Awake()
    {
        //FixedUpdateQueue.functions.Add(new Function { function = "Listen", instance = this, orderIndex = 4 });
    }
    private void Start()
    {
        Cache.longJump = this;
        Cache.jumpHeightManagerScript.AddValue("LongJump", 1f);
        cameraBobManager.AddNewBob("LongJumpBob", 10f, 2f,true);
    }
    private void Update()
    {
        longJumped = false;
        timeSinceLastLongJump += Time.deltaTime;
        timeAfterLongJump += Time.deltaTime;
        if (Input.GetKeyDown(shift)&& shiftTime<=0f&&FragMovementListener.grounded){
            shiftTime = 1f;
        }
        if (shiftTime > 0f){
            shiftTime -= Time.deltaTime / shiftDecayTime;
            shiftTime = Mathf.Clamp(shiftTime, 0f, 1f);
        }
        if (shiftTime >= 0.8f) { Cache.jumpHeightManagerScript.UpdateValue("LongJump", jumpHeightIncrease); }
        else { Cache.jumpHeightManagerScript.UpdateValue("LongJump", 1+((jumpHeightIncrease-1) * shiftTime)); }
        if (FragMovementListener.grounded && Input.GetKeyDown(Cache.surfCharacter.JumpButton)){
            if (shiftTime >= 0.8f) 
            {
                Debug.DrawRay(Cache.surfCharacter.transform.position + (Cache.surfCharacter.transform.forward)+new Vector3(0,0.1f,0), Vector3.down, Color.green, 3f);
                float forwardVelIncrease = 1f;
                if (!Physics.Raycast(Cache.surfCharacter.transform.position + (Cache.surfCharacter.transform.forward) + new Vector3(0, 0.1f, 0), Vector3.down, 1f, layerMask)){
                    forwardVelIncrease = ledgePowerIncrease;
                }
                Cache.moveData.Velocity += (Cache.surfCharacter.Forward * velocityIncrease * forwardVelIncrease) * Mathf.Clamp(FragMovementListener.hSpeedPercentage, 0f, 1f);
                cameraBobManager.AddBobForce("JumpBob", bobAmount); 
                timeSinceLastLongJump = 0f; 
                longJumped = true; 
                StartCoroutine(LongJumpingSequence()); 
                timeAfterLongJump = 0f; 
            }
            else if (shiftTime > 0f) 
            {
                Debug.DrawRay(Cache.surfCharacter.transform.position + (Cache.surfCharacter.transform.forward) + new Vector3(0, 0.1f, 0), Vector3.down, Color.green, 3f);
                float forwardVelIncrease = 1f;
                if (!Physics.Raycast(Cache.surfCharacter.transform.position + (Cache.surfCharacter.transform.forward) + new Vector3(0, 0.1f, 0), Vector3.down, 1f, layerMask)){
                    forwardVelIncrease = ledgePowerIncrease;
                }
                Cache.moveData.Velocity += (Cache.surfCharacter.Forward * velocityIncrease * forwardVelIncrease) * Mathf.Clamp(FragMovementListener.hSpeedPercentage, 0f, 1f);
                cameraBobManager.AddBobForce("JumpBob", bobAmount * 1.5f); 
                timeSinceLastLongJump = 0f;
                longJumped = true;
                StartCoroutine(LongJumpingSequence());
                timeAfterLongJump = 0f; }
        }
        /*
        if (FragMovementListener.justJumped) {
            if (shiftTime >= 0.8f) { Cache.moveData.Velocity += (Cache.surfCharacter.Forward * velocityIncrease) * Mathf.Clamp(FragMovementListener.hSpeedPercentage, 0f, 1f); cameraBobManager.AddBobForce("JumpBob", bobAmount); timeSinceLastLongJump = 0f; print(Random.RandomRange(0, 99999)); }
            else if (shiftTime > 0f) { Cache.moveData.Velocity += (Cache.surfCharacter.Forward * (1 + ((velocityIncrease - 1) * shiftTime) * Mathf.Clamp(FragMovementListener.hSpeedPercentage, 0f, 1f))); cameraBobManager.AddBobForce("JumpBob", bobAmount / 2f); timeSinceLastLongJump = 0f; print(Random.RandomRange(0, 99999)); }
        }*/
    }
    IEnumerator LongJumpingSequence()
    {
        longJumping = true;
        yield return new WaitForSeconds(longJumpingTime);
        longJumping = false;
    }
}
