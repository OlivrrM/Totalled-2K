using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class ServerInfo : Packet
    {
        public string instance;
        public int tps;
        public ServerInfo()
        {
            packetType = PacketType.ServerInfo;
        }
    }
}
