using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalBob : MonoBehaviour
{
    public CameraWorldSpaceRotationOffsetManager cameraWorldSpaceRotationOffsetManager;
    Vector3 targetDirection;
    [HideInInspector] public Vector3 currentDirection;
    public float tiltMultiplier;
    public float tiltSpeed;
    public float maximumTilt;

    public float xMultiplier;
    public float zMultiplier;

    public Vector2 xVelocityClampAB;
    public Vector2 zVelocityClampAB;
    private void Start()
    {
        cameraWorldSpaceRotationOffsetManager.AddOffset("DirectionalBob", Vector3.zero);
    }
    private void Update()
    {
        float rightVel = Vector3.Dot(Cache.moveData.Velocity, Cache.surfCharacter.transform.right);
        float forwardVel = Vector3.Dot(Cache.moveData.Velocity, Cache.surfCharacter.transform.forward);
        Vector3 sideDirection = -Cache.mainCam.transform.forward * (Mathf.InverseLerp(zVelocityClampAB.x,zVelocityClampAB.y,Mathf.Abs(rightVel))) * tiltMultiplier * zMultiplier * Mathf.Clamp(rightVel,-1f,1f);
        Vector3 forwardDirection = Cache.mainCam.transform.right * (Mathf.InverseLerp(xVelocityClampAB.x, xVelocityClampAB.y, Mathf.Abs(forwardVel))) * tiltMultiplier * xMultiplier * Mathf.Clamp(forwardVel, -1f, 1f);
        targetDirection = sideDirection + forwardDirection;
        //targetDirection = new Vector3(Mathf.Clamp(Cache.moveData.Velocity.z * tiltMultiplier * zMultiplier, -maximumTilt, maximumTilt), 0, Mathf.Clamp(-(Cache.moveData.Velocity.x * tiltMultiplier * xMultiplier), -maximumTilt, maximumTilt));
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, Time.deltaTime * tiltSpeed);
        cameraWorldSpaceRotationOffsetManager.UpdateOffset("DirectionalBob", currentDirection);
    }
}


/// Old code that used world space velocity direction. Changed due to gameplay being effected by forward/backward movements
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalBob : MonoBehaviour
{
    public CameraWorldSpaceRotationOffsetManager cameraWorldSpaceRotationOffsetManager;
    Vector3 targetDirection;
    [HideInInspector] public Vector3 currentDirection;
    public float tiltMultiplier;
    public float tiltSpeed;
    public float maximumTilt;

    public float xMultiplier;
    public float zMultiplier;
    private void Start()
    {
        cameraWorldSpaceRotationOffsetManager.AddOffset("DirectionalBob", Vector3.zero);
    }
    private void Update()
    {
        targetDirection = new Vector3(Mathf.Clamp(Cache.moveData.Velocity.z * tiltMultiplier * zMultiplier, -maximumTilt, maximumTilt), 0, Mathf.Clamp(-(Cache.moveData.Velocity.x * tiltMultiplier * xMultiplier), -maximumTilt, maximumTilt));
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, Time.deltaTime * tiltSpeed);
        cameraWorldSpaceRotationOffsetManager.UpdateOffset("DirectionalBob", currentDirection);
    }
}
*/