using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultBob : MonoBehaviour 
{
    public Vaulter vaulter;
    public float tiltBobAmount;
    public float tiltBobWalkSpeed;
    public float tiltBobSprintSpeed;
    public float minTiltSpeed;
    Vector3 tiltBobPointer;
    Vector3 tiltBobTarget;
    Vector3 tiltBobDirection;

    public CameraBobManager cameraBobManager;
    public float cameraBobAmount;

    [HideInInspector] public bool vaultCamDirection;
    private void Start()
    {
        Cache.vaultBob = this;
        cameraBobManager.AddNewBob("VaultJoltBob", 15f, 2f);
        Cache.cameraWorldSpaceRotationOffsetManager.AddOffset("VaultBob", Vector3.zero);
    }
    private void LateUpdate()
    {
        if (vaulter.vaultFrame)
        {
            OnVault();
        }
        if (vaulter.finishVaultFrame)
        {
            OnVaultFinish();
        }
    }
    void OnVault()
    {
        vaultCamDirection = IsVaultCamDirectionRight(vaulter.targetVaultNormal);
        tiltBobDirection = Cache.surfCharacter.transform.forward*Utilities.BoolToIntn(vaultCamDirection);
        tiltBobTarget = tiltBobDirection * (tiltBobAmount* (1+new Vector2(vaulter.targetVaultNormal.x, vaulter.targetVaultNormal.z).magnitude));
        cameraBobManager.AddBobForce("VaultJoltBob", cameraBobAmount);
    }
    bool IsVaultCamDirectionRight(Vector3 targetNormal)
    {
        if (targetNormal == new Vector3(0f, 1f, 0f)) { return Utilities.CoinFlip(); } //Face is perpendicular
        float dotProduct = Vector3.Dot(targetNormal, Cache.surfCharacter.transform.right);
        if (dotProduct > 0) { return true; }
        else if (dotProduct < 0) { return false; }
        else { return Utilities.CoinFlip(); } //Face is perpendicular
    }
    void OnVaultFinish()
    {
        tiltBobTarget = Vector3.zero;
    }
    private void Update()
    {
        tiltBobPointer = Vector3.Lerp(tiltBobPointer, tiltBobTarget, Time.deltaTime * (FragMovementListener.hSpeedPercentage > 1.2f ? tiltBobSprintSpeed : tiltBobWalkSpeed) * Mathf.Clamp(vaulter.currentVaultSpeed,minTiltSpeed,Mathf.Infinity));
        Cache.cameraWorldSpaceRotationOffsetManager.UpdateOffset("VaultBob", tiltBobPointer);
    }
}
