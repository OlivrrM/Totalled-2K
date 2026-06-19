using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

[System.Serializable]
public class sv_PlayerDisconnected : sv_Packet
{
    public byte uuid;
    public sv_PlayerDisconnected()
    {
        packetType = sv_PacketType.sv_PlayerDisconnected;
    }
}
