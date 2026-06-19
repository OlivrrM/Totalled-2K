using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    public enum PacketType : byte
    {
        Unknown,
        ServerInfo,
        MapLoadedResult,
        PlayerInfo,
        PlayerDisconnected,
        Movement,
        EnteredGameValidation,
        Orientation
    }

    [Serializable]
    class Packet
    {
        public PacketType packetType = PacketType.Unknown;
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NonDeserializedPacketField : Attribute { }
}
