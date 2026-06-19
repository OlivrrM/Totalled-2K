using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class MapLoadedResult : Packet
    {
        public bool success;
        public MapLoadedResult()
        {
            packetType = PacketType.MapLoadedResult;
        }
    }
}
