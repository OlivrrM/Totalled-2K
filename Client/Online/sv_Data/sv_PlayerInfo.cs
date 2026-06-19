using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using Totalled.Numerics;

[System.Serializable]
public class sv_PlayerInfo : sv_Packet
{
    public byte uuid;
    public Vector3f16 position;
    public Vector3si8 orientation;
    public sv_PlayerInfo()
    {
        packetType = sv_PacketType.sv_PlayerInfo;

        position = new Vector3f16(0f,0f,0f);
        orientation = new Vector3si8(0f, 0f, 0f);
    }
}
