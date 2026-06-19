using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class WorldInfo : Packet //Unused for now
    {
        public WorldInfo() 
        {
            packetType = PacketType.Unknown;
        }
    }
}