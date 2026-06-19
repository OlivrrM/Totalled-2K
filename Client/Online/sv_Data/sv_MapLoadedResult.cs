using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
[System.Serializable]
public class sv_MapLoadedResult : sv_Packet
{
    public bool success;
    public sv_MapLoadedResult()
    {
        packetType = sv_PacketType.sv_MapLoadedResult;
    }
}
