using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Logger;

namespace t2kCore
{
    class World
    {
        //List<Player> players;
        public Dictionary<byte, Player> players = new Dictionary<byte, Player>();

        public int tps = 1;

        public Server server;

        public TickManager tickManager;
        public World(Server server)
        {
            this.server = server;
            //this.server = server;
            //players = this.server.connectedClients.Values.Select(client => client.player).ToList();
            tps = server.serverInfo.tps;

            ConsoleLogger.LogInfo("World started");

            tickManager = new TickManager(this);
            _ = Ticker();
        }
        public async Task Ticker()
        {
            while (true)
            {
                await Task.Delay((int)((1f / tps) * 1000f));
                try { Tick(); }
                catch (Exception e) { ConsoleLogger.LogError($"Error running tick: {e.Message}"); }
            }
        }
        public void Tick()
        {
            if (tickManager.packetsNextTick.Count > 0){
                foreach (KeyValuePair<byte,List<Packet>> playerTickPackets in tickManager.packetsNextTick){
                    PacketBatch packetBatch = new PacketBatch();
                    for (int i = 0; i < playerTickPackets.Value.Count; i++){
                        packetBatch.AddPacket(PacketSerializer.SerializePacket(playerTickPackets.Value[i]));
                    }
                    server.SendDataToClient(packetBatch.Serialize(), server.world.players[playerTickPackets.Key].client.tcpClient);
                }
                tickManager.packetsNextTick.Clear();
            }
        }
        public void AddPlayer(Client client)
        {
            client.player = new Player();
            client.player.client = client;
            Random random = new Random();
            int uuid = random.Next(0,255);
            byte timesChecked = 0;
            while (true)
            {
                bool valid = false;
                if (players.Count > 0)
                {
                    foreach (KeyValuePair<byte, Player> player in players){
                        if (player.Value.uuid == uuid){
                            uuid = random.Next(0, 255);
                        }
                        else{
                            valid = true;
                            break;
                        }
                    }
                }
                else
                {
                    valid = true;
                }
                timesChecked++;
                if (timesChecked == 0){
                    ConsoleLogger.LogWarning("Server full!");
                    return;
                }
                if (valid) { break; }
            }
            client.player.uuid = (byte)uuid;
            players.Add(client.player.uuid, client.player);

            //ConsoleLogger.LogInfo("Added new player");

            tickManager.AddPacketsForAllBesides(
                new List<Packet>{
                    new PlayerInfo(client.player)
            },client.player.uuid);
        }
        public void RemovePlayer(Client client)
        {
            if (client.player != null)
            {
                if (players.ContainsKey(client.player.uuid))
                {
                    tickManager.AddPacketsForAllBesides(
                        new List<Packet>{
                        new PlayerDisconnected(){ uuid = client.player.uuid }
                    }, client.player.uuid); 
                    players.Remove(client.player.uuid);
                    //ConsoleLogger.LogInfo("Removed player");
                }
                else
                {
                    ConsoleLogger.LogWarning($"Attempted to remove player with uuid {client.player.uuid} but player doesn't exist");
                }
            }
            else
            {
                ConsoleLogger.LogWarning($"Attempted to remove player attatched to client {client.tcpClient.Client.RemoteEndPoint.ToString()} but no player is attatched");
            }
        }
    }
}
