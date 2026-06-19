using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class Movement : Packet
    {
        [NonDeserializedPacketField] public byte uuid;
        public Vector3f16 position;
        public byte meta;
        public Movement()
        {
            packetType = PacketType.Movement;

            position = new Vector3f16(0f,0f,0f);
        }
    }
}
