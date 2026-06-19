using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelRobotGun : MonoBehaviour
{
    public Transform gun;
    Quaternion targetDirection;
    public float lookSpeed;

    // Limit settings for yaw (left/right) and pitch (up/down)
    public float minYaw = -45f;  // Minimum horizontal rotation (left)
    public float maxYaw = 45f;   // Maximum horizontal rotation (right)
    public float minPitch = -30f; // Minimum vertical rotation (down)
    public float maxPitch = 30f;  // Maximum vertical rotation (up)

    private void Update()
    {
        if (Robot.ghost) {gun.localRotation = Quaternion.identity; return; }
        // Calculate the target direction (relative to the gun's parent)
        Vector3 targetPosition = Cache.surfCharacter.transform.position + new Vector3(0f, 1f, 1f);
        targetDirection = Quaternion.LookRotation(gun.parent.InverseTransformPoint(targetPosition) - gun.localPosition);

        // Interpolate the gun's local rotation towards the target direction
        Quaternion newRotation = Quaternion.Lerp(gun.localRotation, targetDirection, Time.deltaTime * lookSpeed);

        // Convert the local rotation to Euler angles so we can clamp them
        Vector3 localEulerRotation = newRotation.eulerAngles;

        // Clamp the yaw (Y-axis rotation) between minYaw and maxYaw
        localEulerRotation.y = Utilities.ClampAngle(localEulerRotation.y, minYaw, maxYaw);

        // Clamp the pitch (X-axis rotation) between minPitch and maxPitch
        localEulerRotation.x = Utilities.ClampAngle(localEulerRotation.x, minPitch, maxPitch);

        // Optional: If you want to lock the roll (Z-axis rotation), you can set eulerRotation.z to 0
        localEulerRotation.z = 0f;

        // Convert the clamped Euler angles back to a Quaternion and apply the local rotation
        gun.localRotation = Quaternion.Euler(localEulerRotation);
    }
}
