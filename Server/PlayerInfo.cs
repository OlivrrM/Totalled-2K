using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    [Serializable]
    class PlayerInfo :  Packet
    {
        public byte uuid;
        public Vector3f16 position;
        public Vector3si8 orientation;

        public PlayerInfo(Player player)
        {
            packetType = PacketType.PlayerInfo;

            uuid = player.uuid;
            position = player.position;
            orientation = player.orientation;
        }
    }
}
