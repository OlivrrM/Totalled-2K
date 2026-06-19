using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace t2kCore
{
    class Player
    {
        public Client client;
        public byte uuid;

        public Vector3f16 position;
        public Vector3si8 orientation;

        public Player()
        {
            position = new Vector3f16(0f,0f,0f);
            orientation = new Vector3si8(0f,0f,0f);
        }
    }
}
