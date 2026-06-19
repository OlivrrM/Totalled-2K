using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using Totalled.Numerics;
using static UnityEngine.Networking.UnityWebRequest;

[System.Serializable]
public class sv_Movement : sv_Packet
{
    [NonSerializedPacketField] public byte uuid;
    public Vector3f16 position;
    public byte meta;
    public sv_Movement()
    {
        packetType = sv_PacketType.sv_Movement;
    }
    public void SerializeMeta(bool jumping,bool vaulting, Vector2 moveDirection)
    {
        meta |= (byte)(Utilities.BoolToInt(jumping) << 0);
        meta |= (byte)(Utilities.BoolToInt(vaulting) << 1);

        meta |= (byte)(moveDirection.x > 0 ? (1 << 2) : moveDirection.x < 0 ? (1 << 3) : 0);
        meta |= (byte)(moveDirection.y > 0 ? (1 << 4) : moveDirection.y < 0 ? (1 << 5) : 0);
    }
}
