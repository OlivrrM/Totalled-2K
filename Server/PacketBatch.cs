using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace t2kCore
{
    [Serializable]
    class PacketBatch
    {
        public List<byte[]> packets = new List<byte[]>();
        public void AddPacket(byte[] packetData)
        {
            packets.Add(packetData);
        }
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write((byte)packets.Count);

                foreach (var packet in packets)
                {
                    writer.Write((short)packet.Length);
                    writer.Write(packet);
                }
                return ms.ToArray();
            }
        }
    }
}
