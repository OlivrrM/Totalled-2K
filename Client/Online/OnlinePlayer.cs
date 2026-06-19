using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled.Numerics;
using UnityEngine.UI;
using System.Net.Sockets;

public class OnlinePlayer : MonoBehaviour
{
    public byte uuid;

    public float posLerpSpeed; //Move all of this to OnlinePlayerMovementManager
    Vector3 targetPos;
    public float headRotSpeed;
    Vector3 targetHeadRot;
    public float bodyRotSpeed;
    Vector3 targetBodyRot;

    public OnlinePlayerMovementManager onlinePlayerMovementManager;
    public Transform headTarget;
    public Transform head;
    public void UpdatePlayerInfo(sv_PlayerInfo playerInfo)
    {
        uuid = playerInfo.uuid;
        targetPos = (Vector3)playerInfo.position;
    }
    public void UpdateMovement(sv_Movement movement)
    {
        SetTargetPosition((Vector3)movement.position);
        onlinePlayerMovementManager.UpdateMovementMeta(movement.meta);
    }
    public void UpdateOrientation(sv_Orientation orientation)
    {
        SetTargetOrientation(new Vector3((float)(orientation.euler.x * 1.417f), (float)(orientation.euler.y * 1.417f), (float)(orientation.euler.z * 1.417f)));
    }
    public void SetTargetOrientation(Vector3 targetOrientation)
    {
        targetBodyRot = new Vector3(0f, targetOrientation.y, 0f);
        targetHeadRot = targetOrientation;
    }
    public void SetTargetPosition(Vector3 pos)
    {
        targetPos = pos;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * posLerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(targetBodyRot),Time.deltaTime*bodyRotSpeed);
        headTarget.rotation = Quaternion.Lerp(headTarget.rotation, Quaternion.Euler(targetHeadRot),Time.deltaTime*headRotSpeed);
        headTarget.position = head.position;
        headTarget.position += headTarget.forward * 5f;
    }
}
