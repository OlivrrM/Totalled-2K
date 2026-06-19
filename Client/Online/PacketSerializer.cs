using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Totalled;
using Totalled.Numerics;
using UnityEngine;

public class PacketSerializer
{
    public static byte[] SerializePacket(sv_Packet packet)
    {
        using (MemoryStream ms = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(ms))
        {
            // Write the packet type
            writer.Write((byte)packet.packetType);

            // Get all fields (including those in subclasses) via reflection
            var fields = packet.GetType().GetFields(
                                            BindingFlags.Public |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance)
                                            .Where(field => !field.GetCustomAttributes(typeof(NonSerializedPacketField), false).Any())
                                            .ToArray();

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute(typeof(NonSerializedPacketField)) == null)
                {
                    object value = field.GetValue(packet);
                    if (value is int intValue)
                        writer.Write(intValue);
                    else if (value is float floatValue)
                        writer.Write(floatValue);
                    else if (value is bool boolValue)
                        writer.Write(boolValue);
                    else if (value is string stringValue)
                        writer.Write(stringValue ?? string.Empty);
                    else if (value is byte byteValue)
                        writer.Write(byteValue);
                    else if (value is sbyte sbyteValue)
                        writer.Write(sbyteValue);
                    else if (value is Vector3si8 vector3si8Value){
                        writer.Write((sbyte)vector3si8Value.x);
                        writer.Write((sbyte)vector3si8Value.y);
                        writer.Write((sbyte)vector3si8Value.z);
                    }
                    else if (value is UnityEngine.Vector3 vector3Value){
                        writer.Write(vector3Value.x);
                        writer.Write(vector3Value.y);
                        writer.Write(vector3Value.z);
                    }
                    else if (value is Vector3f16 vector3f16Value){
                        writer.Write((short)vector3f16Value.x);
                        writer.Write((short)vector3f16Value.y);
                        writer.Write((short)vector3f16Value.z);
                    }
                    // Add handling for other types as needed
                }
                else
                {
                    Cache.terminal.Print("NonSerializedPacketField FOUND: "+field.FieldType.GetType().FullName);
                }
            }
            
            return ms.ToArray();
        }
    }
    
    public static sv_Packet DeserializePacket(byte[] data)
    {
        sv_PacketType packetType = sv_PacketType.sv_Unknown;
        try
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                // Read the packet type
                packetType = (sv_PacketType)reader.ReadByte();

                // Determine the class name based on the packet type
                string className = packetType.ToString(); // "ServerInfo" for PacketType.ServerInfo

                // Use reflection to get the Type of the class
                Type type = Type.GetType(className); /// Using Type.GetType() in a normal c# project requires you to include the 
                                                     /// namespace in the string, but in Unity it doens't. Very ambiguous.

                // If type is found, create an instance of it; otherwise, create a default Packet
                sv_Packet packet = type != null && typeof(sv_Packet).IsAssignableFrom(type)
                    ? (sv_Packet)Activator.CreateInstance(type)
                    : new sv_Packet { packetType = sv_PacketType.sv_Unknown };

                // Get all fields in the instance and populate them
                var fields = packet.GetType().GetFields(System.Reflection.BindingFlags.Public |
                                                        System.Reflection.BindingFlags.NonPublic |
                                                        System.Reflection.BindingFlags.Instance);

                foreach (var field in fields)
                {
                    if (field.FieldType == typeof(int))
                        field.SetValue(packet, reader.ReadInt32());
                    else if (field.FieldType == typeof(float))
                        field.SetValue(packet, reader.ReadSingle());
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(packet, reader.ReadBoolean());
                    else if (field.FieldType == typeof(string))
                        field.SetValue(packet, reader.ReadString());
                    else if (field.FieldType == typeof(byte))
                        field.SetValue(packet, reader.ReadByte());
                    else if (field.FieldType == typeof(Vector3si8)){
                        field.SetValue(packet, new Vector3si8(reader.ReadSByte(), reader.ReadSByte(), reader.ReadSByte()));
                    }
                    else if (field.FieldType == typeof(UnityEngine.Vector3))
                    {
                        UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        field.SetValue(packet, vector3);
                    }
                    else if (field.FieldType == typeof(Vector3f16))
                    {
                        // Read as short and convert back to Half
                        short x = reader.ReadInt16();
                        short y = reader.ReadInt16();
                        short z = reader.ReadInt16();
                        field.SetValue(packet, new Vector3f16(Half.FromShort(x), Half.FromShort(y), Half.FromShort(z)));
                    }
                    // Add handling for other types as needed
                }

                return packet;
            }
        }
        catch (Exception e) 
        {
            Debug.LogError($"Error deserializing packet of type {packetType.ToString()}: {e.Message}");
            return null;
        }
    }
    public static sv_Packet[] DeserializePacketBatch(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            int packetCount = reader.ReadByte();

            sv_Packet[] packetBatch = new sv_Packet[packetCount];

            for (int i = 0; i < packetCount; i++)
            {
                short packetLength = reader.ReadInt16();
                byte[] packetData = reader.ReadBytes(packetLength);

                packetBatch[i] = DeserializePacket(packetData);
            }

            return packetBatch;
        }
    }
}