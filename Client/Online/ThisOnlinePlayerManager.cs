using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled.Numerics;

public class ThisOnlinePlayerManager : MonoBehaviour
{
    Vector3 lastTickPos;
    Vector3 lastTickCamRot;
    // levi was here
    private void Start()
    {
        Cache.onlineProxyOut.OnPreTick += OnPreTick;
    }
    public void OnPreTick()
    {
        if (lastTickPos != Cache.surfCharacter.transform.position){
            sv_Movement movement = new sv_Movement { position = (Vector3f16)Cache.surfCharacter.transform.position };
            movement.SerializeMeta(InputManager.GetJumpKey(), Cache.vaulter.vaulting, new Vector2(Cache.moveData.SideMove, Cache.moveData.ForwardMove));
            Cache.onlineProxyOut.packetsNextTick.Add(movement);
        }
        if (lastTickCamRot != Cache.mainCam.transform.eulerAngles){
            sv_Orientation orientation = new sv_Orientation{
                euler = new Vector3si8(Cache.mainCam.transform.eulerAngles.x * 0.70555f, Cache.mainCam.transform.eulerAngles.y * 0.70555f, Cache.mainCam.transform.eulerAngles.z * 0.70555f)
            };
            Cache.onlineProxyOut.packetsNextTick.Add(orientation);
        }
        //Cache.terminal.Print(Cache.surfCharacter.transform.position.x.ToString() + "\n"+((Vector3f16)Cache.surfCharacter.transform.position).x.ToString());
        lastTickPos = Cache.surfCharacter.transform.position;
        lastTickCamRot = Cache.mainCam.transform.eulerAngles;
    }
}
