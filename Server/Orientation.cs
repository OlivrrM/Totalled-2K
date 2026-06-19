using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class Orientation : Packet
    {
        [NonDeserializedPacketField] public byte uuid;
        public Vector3si8 euler;
        public Orientation() 
        {
            packetType = PacketType.Orientation;

            euler = new Vector3si8(0f,0f,0f);
        }
    }
}
