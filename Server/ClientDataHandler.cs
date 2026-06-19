using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger;
using System.Net;
using System.Net.Sockets;

namespace t2kCore
{
    class ClientDataHandler
    {
        public Server server;

        public ClientDataHandler(Server server)
        {
            this.server = server;
        }

        public void HandleData(byte[] data, TcpClient client)
        {
            Packet[] packets = PacketSerializer.DeserializePacketBatch(data);
            for (int i = 0; i < packets.Length; i++){
                //ConsoleLogger.LogInfo($"Received packet: {packets[i].packetType}");
                this.GetType().GetMethod(packets[i].packetType.ToString()).Invoke(this, new object[] { packets[i], client });
            }
        }

        public void MapLoadedResult(MapLoadedResult mapLoadedResult, TcpClient client)
        {
            PacketBatch packetBatch = new PacketBatch();
            foreach (KeyValuePair<byte,Player> player in server.world.players){
                packetBatch.packets.Add(PacketSerializer.SerializePacket(new PlayerInfo(player.Value)));
            }
            packetBatch.packets.Add(PacketSerializer.SerializePacket(new EnteredGameValidation()));
            server.SendDataToClient(packetBatch.Serialize(), client);
            server.world.AddPlayer(server.connectedClients[client]);


            //Here we gotta send World Info, shit like where all the players r at and stuff
        }
        public void Movement(Movement movement,TcpClient client){
            server.world.players[server.connectedClients[client].player.uuid].position = movement.position;
            server.world.tickManager.AddPacketsForAllBesides(
                new List<Packet>
                {
                    new Movement
                    {
                        uuid = server.connectedClients[client].player.uuid,
                        position = movement.position,
                        meta = movement.meta
                    }
                },
                server.connectedClients[client].player.uuid);
            //ConsoleLogger.LogInfo($"Player [{server.connectedClients[client].player.uuid}] moved to x:{movement.position.x},y:{movement.position.y},z:{movement.position.z}");
        }
        public void Orientation(Orientation orientation, TcpClient client)
        {
            server.world.players[server.connectedClients[client].player.uuid].orientation = orientation.euler;
            server.world.tickManager.AddPacketsForAllBesides(
                new List<Packet>
                {
                    new Orientation
                    {
                        uuid = server.connectedClients[client].player.uuid,
                        euler = orientation.euler
                    }
                },
                server.connectedClients[client].player.uuid);
        }
    }
}
