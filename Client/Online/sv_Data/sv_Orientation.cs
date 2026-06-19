using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using Totalled.Numerics;

[Serializable]
public class sv_Orientation : sv_Packet
{
    [NonSerializedPacketField] public byte uuid;
    public Vector3si8 euler;
    public sv_Orientation()
    {
        packetType = sv_PacketType.sv_Orientation;

        euler = new Vector3si8(0f,0f,0f);
    }
}
