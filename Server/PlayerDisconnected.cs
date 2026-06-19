using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class PlayerDisconnected : Packet
    {
        public byte uuid;
        public PlayerDisconnected() 
        {
            packetType = PacketType.PlayerDisconnected;
        }
    }
}
