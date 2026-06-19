using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
class sv_PacketBatch
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
            //Debug.Log($"Serialized: {Utilities.ToReadableByteArray(ms.ToArray())}");
            return ms.ToArray();
        }
    }
}