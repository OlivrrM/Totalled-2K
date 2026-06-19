using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace t2kCore
{
    class TickManager
    {
        public World world;

        public Dictionary<byte,List<Packet>> packetsNextTick = new Dictionary<byte, List<Packet>>();

        public TickManager(World world) 
        {
            this.world = world;
        }

        public void AddPacketsForAll(List<Packet> packets)
        {
            foreach (KeyValuePair<byte, Player> player in world.players) {
                if (!packetsNextTick.ContainsKey(player.Key)){
                    packetsNextTick.Add(player.Key, packets);
                }
                else{
                    packetsNextTick[player.Key].AddRange(packets);
                }
            }
        }
        public void AddPacketsForAllBesides(List<Packet> packets, byte dontIncludePlayerUuid)
        {
            foreach (KeyValuePair<byte, Player> player in world.players){
                if (player.Value.uuid != dontIncludePlayerUuid){
                    if (!packetsNextTick.ContainsKey(player.Key)){
                        packetsNextTick.Add(player.Key, packets);
                    }
                    else{
                        packetsNextTick[player.Key].AddRange(packets);
                    }
                }
            }
        }
        public void AddPacketsForAllBesides(List<Packet> packets, List<byte> dontIncludePlayerUuids)
        {
            foreach (KeyValuePair<byte, Player> player in world.players){
                if (!dontIncludePlayerUuids.Contains(player.Value.uuid)){
                    if (!packetsNextTick.ContainsKey(player.Key)){
                        packetsNextTick.Add(player.Key, packets);
                    }
                    else{
                        packetsNextTick[player.Key].AddRange(packets);
                    }
                }
            }
        }
        public void AddPacketsForOnly(List<Packet> packets, byte onlyIncludePlayerUuid)
        {
            if (packetsNextTick.ContainsKey(onlyIncludePlayerUuid)){
                packetsNextTick.Add(onlyIncludePlayerUuid, packets);
            }
            else{
                packetsNextTick[onlyIncludePlayerUuid].AddRange(packets);
            }
        }
        public void AddPacketsForOnly(List<Packet> packets, List<byte> onlyIncludePlayerUuid)
        {
            for (int i = 0; i < onlyIncludePlayerUuid.Count; i++){
                if (packetsNextTick.ContainsKey(onlyIncludePlayerUuid[i])){
                    packetsNextTick.Add(onlyIncludePlayerUuid[i], packets);
                }
                else{
                    packetsNextTick[onlyIncludePlayerUuid[i]].AddRange(packets);
                }
            }
        }
    }
}
