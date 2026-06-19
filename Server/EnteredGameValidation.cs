using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    class EnteredGameValidation : Packet
    {
        public EnteredGameValidation()
        {
            packetType = PacketType.EnteredGameValidation;
        }
    }
}
