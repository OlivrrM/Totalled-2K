using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallclimber : MonoBehaviour
{
    public LayerMask layerMask;
    public float checkHeight;
    public float checkFowardDistance;
    public GameObject topCheck;
    IsColliding topCheckCollider;
    public float requiredYDistance;

    Vector3 targetPos;
    Vector3 castPoint;
    [HideInInspector] public bool wallclimbing;

    public float climbingSpeed;
    float climbingTime;

    Vector3 startPos;

    public float endingSpeed;
    float endingTime;

    public float transitionSmoothness;
    float currentSpeed;
    float targetSpeed;

    public float finishPushAmount;

    public CameraBobManager cameraBobManager;
    public float bobAmount;

    Vector3 playerForwardOnStart;
    public float climbingNudgeBackAmount;
    public float climbingNudgeBackSmoothness;
    Vector3 targetNudgeBack;
    Vector3 currentNudgeBack;
    public CameraLocalPositionOffsetManager cameraLocalPositionOffsetManager;

    [HideInInspector] public float animationTime;
    enum wallclimbingStage
    {
        Climbing,
        Ending,
        None
    }
    wallclimbingStage wcStage;
    private void Start()
    {
        Cache.wallclimber = this;
        topCheck.transform.SetParent(null);
        topCheckCollider = topCheck.GetComponent<IsColliding>();
        wcStage = wallclimbingStage.None;
        animationTime = (1 * climbingSpeed) + (1 * endingSpeed);
        cameraBobManager.AddNewBob("WallclimbJoltBob", 5f, 1f,true);
        cameraLocalPositionOffsetManager.AddOffset("WallclimbNudge", Vector3.zero);
        currentSpeed = 3f;
    }
    private void Update()
    {
        float checkHeightAmount = checkHeight * 0.8f;
        float checkHeightIncrease = (checkHeight * 0.2f) * 1 - (Mathf.Clamp(Cache.longJump.timeSinceLastLongJump, 0, 1.5f) * 0.666f);
        topCheck.transform.position = Cache.surfCharacter.transform.position + new Vector3(0, checkHeightAmount+checkHeightIncrease, 0)+(Cache.surfCharacter.transform.forward* checkFowardDistance);
        if (Input.GetKeyDown(Cache.surfCharacter.JumpButton)&&!wallclimbing)//&& !topCheckCollider.colliding)
        {
            Debug.DrawRay(Cache.mainCam.position, Cache.surfCharacter.Forward, Color.cyan,3f);
            RaycastHit hit;
            if (Physics.Raycast(Cache.mainCam.position, Cache.surfCharacter.Forward,out hit, 1, layerMask))
            {
                playerForwardOnStart = -hit.transform.forward;
                RaycastHit topHit;
                Debug.DrawRay(topCheck.transform.position, Vector3.down * 10, Color.yellow,3f);
                if (Physics.Raycast(topCheck.transform.position, Vector3.down,out topHit, Mathf.Infinity, layerMask))
                {
                    if ((topHit.point.y- Cache.surfCharacter.transform.position.y) > requiredYDistance
                        && !FragMovementListener.grounded
                        && Cache.moveData.Velocity.y > -0.25f){
                        targetPos = topHit.point;
                        castPoint = topHit.point;
                        StartWallclimb();
                    }
                }
            }
        }
        if (wallclimbing)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * transitionSmoothness);
            switch (wcStage)
            {
                case wallclimbingStage.Climbing:
                    climbingTime += Time.deltaTime * currentSpeed;
                    Cache.surfCharacter.transform.position = Vector3.Lerp(startPos, targetPos, climbingTime);
                    targetNudgeBack = playerForwardOnStart * climbingNudgeBackAmount;
                    if (climbingTime > 1f){
                        //Cache.surfCharacter.transform.position = Vector3.Lerp(Cache.surfCharacter.transform.position, targetPos, 0.9f);
                        targetPos = new Vector3(castPoint.x, targetPos.y + 1.75f, castPoint.z);
                        startPos = Cache.surfCharacter.transform.position;
                        targetNudgeBack = Vector3.zero;
                        targetSpeed = endingSpeed;
                        wcStage = wallclimbingStage.Ending;
                    }
                    break;
                case wallclimbingStage.Ending:
                    endingTime += Time.deltaTime * currentSpeed;
                    Cache.surfCharacter.transform.position = Vector3.Lerp(startPos, targetPos, endingTime);
                    if (endingTime > 1f){
                        //Cache.surfCharacter.transform.position = Vector3.Lerp(Cache.surfCharacter.transform.position, targetPos, 1f);
                        wcStage = wallclimbingStage.None;
                    }
                    break;
                case wallclimbingStage.None:
                    EndWallclimb();
                    break;
            }
        }
        currentNudgeBack = Vector3.Lerp(currentNudgeBack, targetNudgeBack, Time.deltaTime * climbingNudgeBackSmoothness);
        cameraLocalPositionOffsetManager.UpdateOffset("WallclimbNudge", currentNudgeBack);
    }
    void StartWallclimb()
    {
        FragMovementManager.SetActive(false);
        targetPos = new Vector3(Cache.surfCharacter.transform.position.x, targetPos.y-1.25f, Cache.surfCharacter.transform.position.z);
        climbingTime = 0;
        endingTime = 0;
        startPos = Cache.surfCharacter.transform.position;
        targetSpeed = climbingSpeed;
        cameraBobManager.AddBobForce("WallclimbJoltBob", bobAmount);
        wallclimbing = true;
        wcStage = wallclimbingStage.Climbing;
    }
    public void EndWallclimb()
    {
        wallclimbing = false;
        wcStage = wallclimbingStage.None;
        FragMovementManager.SetActive(true);
        Cache.moveData.Velocity = Vector3.zero;
        Cache.moveData.Velocity += Cache.surfCharacter.Forward * finishPushAmount;
    }
}
