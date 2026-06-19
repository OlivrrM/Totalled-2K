using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;

[System.Serializable]
public class sv_EnteredGameValidation : sv_Packet
{
    public sv_EnteredGameValidation()
    {
        packetType = sv_PacketType.sv_EnteredGameValidation;
    }
}
