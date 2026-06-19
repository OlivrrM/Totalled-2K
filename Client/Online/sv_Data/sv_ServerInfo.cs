using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

[System.Serializable]
public class sv_ServerInfo : sv_Packet
{
    public string instance;
    public int tps;
    public sv_ServerInfo()
    {
        packetType = sv_PacketType.sv_ServerInfo;
    }
}
