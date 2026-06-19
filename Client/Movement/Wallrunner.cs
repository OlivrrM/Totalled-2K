using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallrunner : MonoBehaviour
{
    public bool right;
    float rightf;
    public LayerMask layerMask;
    public float wallrunCheckDistance;

    [HideInInspector] public bool wallrunning;

    public float gravityDivision;
    public float wallrunGravity;

    public float stopWallrunningJumpHeight;

    public float wallrunSpeedIncrease;
    public float wallrunOppositeDirectionSpeed;

    float timeStoppedWallrunning;

    Vector3 wallRunDirection;

    public float camTiltAmount;
    public float camTiltSpeed;
    float camTilt;

    //[HideInInspector] public bool available;

    public float runBobMultiplier;

    public float requiredMoveSpeedPercentage;

    public static bool globalWallrunning;
    public static int wallrunningDirection;

    public float runBobAmountMultiplier;

    public float cameraBobAmount;
    public CameraBobManager cameraBobManager;

    public float wallrunKickAmountMultiplier;

    public LocalRotationOffsetManager bodyRotationOffsetManager;
    public float bodyRotationAmount;
    public float bodyRotationSpeed;
    float currentBodyRotationAmount;

    public CameraLocalPositionOffsetManager cameraLocalPositionOffsetManager;
    public float camPositionOffsetAmount;
    public float camPositionOffsetSpeed;
    float currentCamPositionOffset;

    [HideInInspector] public bool backwardsWallrunning;

    public Wallrunner otherWallrunner;

    [HideInInspector] public int availabilityState;

    public enum move
    {
        FL,
        FR,
        BL,
        BR,
        None
    }
    public static move lastMove = move.None;
    private void Awake()
    {
        FixedUpdateQueue.functions.Add(new Function { function = "WallrunCheck", instance = this, orderIndex = Utilities.BoolToInt(right) });
        cameraBobManager.AddNewBob("WallrunBob", 15f, 2f);
        lastMove = move.None;
    }
    private void Start()
    {
        Cache.wallrunner = this;
        rightf = Utilities.BoolToIntn(right);
        camTiltAmount *= rightf;
        Cache.gravityManager.AddValue("Wallrunner"+ rightf.ToString(), 1);
        Cache.runBob.enablers.Add("Wallrunning" + rightf.ToString(), false);
        Cache.cameraWorldSpaceRotationOffsetManager.AddOffset("Wallrunning" + rightf.ToString(), Vector3.zero);
        Cache.runBob.speedMultipliers.Add("Wallrunner", 1f);
        Cache.runBob.amountMultipliers.Add("Wallrunner", 1f);
        if (bodyRotationOffsetManager != null) { bodyRotationOffsetManager.AddValue("Wallrunner" + rightf.ToString(), Vector3.zero); }
        if (cameraLocalPositionOffsetManager != null) { cameraLocalPositionOffsetManager.AddOffset("Wallrunner" + rightf.ToString(), Vector3.zero); } //null check for outdated character controllers
    }
    private void WallrunCheck()
    {
        if (FragMovementListener.grounded) {lastMove = move.None; availabilityState = 0; if (wallrunning) {wallrunning=false; OnWallRunEnd(); } }
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward* wallrunCheckDistance, Color.black);
        if (Physics.Raycast(transform.position, transform.forward,out hit, wallrunCheckDistance, layerMask))
        {
            Debug.DrawRay(Cache.surfCharacter.transform.position, wallRunDirection, Color.blue);
            if (Input.GetKeyDown(Cache.surfCharacter.JumpButton) && !(!wallrunning && FragMovementListener.grounded))
            {
                if (!wallrunning) { if (CanStartWallrun(hit) &&FragMovementListener.hSpeedPercentage >= requiredMoveSpeedPercentage) { OnWallRunStart(); } }
                else { wallrunning = false; OnWallRunEnd(true); Cache.moveData.Velocity += new Vector3(0, stopWallrunningJumpHeight, 0); }
            }
            timeStoppedWallrunning = 0;
        }
        else
        {
            timeStoppedWallrunning += Time.deltaTime;
            if (timeStoppedWallrunning > 0.2f)
            {
                if (wallrunning) { OnWallRunEnd(); }
                wallrunning = false;
            }
        }
    }
    private void Update()
    {
        if (wallrunning)
        {
            Cache.moveData.Velocity += wallRunDirection * wallrunSpeedIncrease * Time.deltaTime;
            if (right) { Cache.surfCharacter.directionalKeysMultipliers = new Vector4(1f, 0f, 1f, 1f); }
            else { Cache.surfCharacter.directionalKeysMultipliers = new Vector4(0f, 1f, 1f, 1f); }
            /*if (Utilities.Vector3Sum(Vector3.Scale(Cache.moveData.Velocity, wallRunDirection)) < 0){
                Cache.moveData.Velocity += wallRunDirection * wallrunOppositeDirectionSpeed * Time.deltaTime;
            }*/

            //print(Cache.moveData.Velocity + "        " + wallRunDirection);
            //Cache.surfCharacter.transform.rotation = Quaternion.Euler(wallRunDirection);
        }
        currentBodyRotationAmount = Mathf.Lerp(currentBodyRotationAmount, (bodyRotationAmount * Utilities.BoolToIntn(right)) * Utilities.BoolToInt(wallrunning), Time.deltaTime * bodyRotationSpeed);
        if (bodyRotationOffsetManager != null) { bodyRotationOffsetManager.UpdateValue("Wallrunner" + rightf.ToString(), new Vector3(0, 0, currentBodyRotationAmount)); }

        currentCamPositionOffset = Mathf.Lerp(currentCamPositionOffset, (camPositionOffsetAmount * -Utilities.BoolToIntn(right)) * Utilities.BoolToInt(wallrunning), Time.deltaTime * camPositionOffsetSpeed);
        if (cameraLocalPositionOffsetManager != null) { cameraLocalPositionOffsetManager.UpdateOffset("Wallrunner" + rightf.ToString(), new Vector3(currentCamPositionOffset, 0, 0)); }

        camTilt = Mathf.Lerp(camTilt, camTiltAmount * Utilities.BoolToInt(wallrunning), Time.deltaTime * camTiltSpeed);
        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("Wallrunning" + rightf.ToString(), -Cache.surfCharacter.transform.forward* -camTilt);
    }
    bool CanStartWallrun(RaycastHit hit)
    {
        bool backwards = false;
        wallRunDirection = (-Vector3.Cross(hit.normal, Vector3.up) * rightf).normalized;
        if (Utilities.Vector3Sum(Vector3.Scale(wallRunDirection, Cache.moveData.Velocity)) < 0) { wallRunDirection = -wallRunDirection; backwards = true; }
        else { backwards = false; }
        backwardsWallrunning = backwards;
        /*
        switch (availabilityState) //Fucked up troolean logic
        {
            case 0:
                return true;
            case 1:
                if (!backwards) { return true; }
                break;
            case -1:
                if (backwards) { return true; }
                break;
        }
        return false;
        */
        bool pass = false;
        move currentMove = move.None;
        if (backwards && right) { currentMove = move.BR; }
        else if (!backwards && right) { currentMove = move.FR; }
        else if (backwards && !right) { currentMove = move.BL; }
        else if (!backwards && !right) { currentMove = move.FL; }
        switch (lastMove)
        {
            case move.BR:
                if (currentMove == move.BL || currentMove == move.FR) { pass = true; }
                break;
            case move.FR:
                if (currentMove == move.FL || currentMove == move.BR) { pass = true; }
                break;
            case move.BL:
                if (currentMove == move.FL || currentMove == move.BR) { pass = true; }
                break;
            case move.FL:
                if (currentMove == move.BL || currentMove == move.FR) { pass = true; }
                break;
            case move.None:
                return true;
        }
        return pass;
    }
    void OnWallRunStart()
    {
        wallrunning = true;
        Cache.moveData.Velocity = new Vector3(Cache.moveData.Velocity.x, Cache.moveData.Velocity.y / gravityDivision, Cache.moveData.Velocity.z);
        Cache.gravityManager.UpdateValue("Wallrunner" + rightf.ToString(), wallrunGravity);
        Cache.runBob.enablers["Wallrunning" + rightf.ToString()] = true;
        float yVelocity = Cache.moveData.Velocity.y;
        Cache.moveData.Velocity = wallRunDirection.normalized * Cache.moveData.Velocity.magnitude;
        Cache.moveData.Velocity = new Vector3(Cache.moveData.Velocity.x, yVelocity, Cache.moveData.Velocity.z);
        Cache.runBob.speedMultipliers["Wallrunner"] = runBobMultiplier;
        Cache.runBob.amountMultipliers["Wallrunner"] = runBobAmountMultiplier;
        //available = false;
        globalWallrunning = true;
        wallrunningDirection = Utilities.BoolToIntn(right);
        cameraBobManager.AddBobForce("WallrunBob", cameraBobAmount);
        //Cache.fragMovementManager.SetFixedBodyRotation(true);
    }
    void OnWallRunEnd(bool premeditated = false)
    {
        Cache.gravityManager.UpdateValue("Wallrunner" + rightf.ToString(), 1f);
        Cache.runBob.enablers["Wallrunning" + rightf.ToString()] = false;
        Cache.runBob.speedMultipliers["Wallrunner"] = 1f;
        Cache.runBob.amountMultipliers["Wallrunner"] = 1f;
        Cache.surfCharacter.directionalKeysMultipliers = Vector4.one;
        Debug.DrawRay(transform.position, wallRunDirection+ (Cache.surfCharacter.transform.right*-Utilities.BoolToIntn(right)), Color.blue, 10);
        if (premeditated) { Cache.moveData.Velocity += (wallRunDirection + (Cache.surfCharacter.transform.right * -Utilities.BoolToIntn(right))) * (new Vector3(Cache.moveData.Velocity.x, 0, Cache.moveData.Velocity.z).magnitude * wallrunKickAmountMultiplier); }
        //otherWallrunner.availabilityState = availabilityState;
        /*
        bool firstWallrun = availabilityState == 0 && otherWallrunner.availabilityState == 0;
        if (((otherWallrunner.availabilityState == 0 && availabilityState == -1) || (otherWallrunner.availabilityState == -1 && availabilityState == 0)) && !firstWallrun)
        {
            int cachedState = availabilityState;
            availabilityState = otherWallrunner.availabilityState;
            otherWallrunner.availabilityState = cachedState;
        }
        else
        {
            if (availabilityState == 0)
            {
                if (backwardsWallrunning) { availabilityState = 1; }
                else { availabilityState = -1; }
            }
            else if (backwardsWallrunning) { availabilityState = 1; }
            else { availabilityState = -1; }
        }
        */
        if (backwardsWallrunning && right) { lastMove = move.BR; }
        else if (!backwardsWallrunning && right) { lastMove = move.FR; }
        else if (backwardsWallrunning && !right) { lastMove = move.BL; }
        else if (!backwardsWallrunning && !right) { lastMove = move.FL; }
        globalWallrunning = false;
        wallrunningDirection = 0;
        //Cache.fragMovementManager.SetFixedBodyRotation(false);
    }
}
