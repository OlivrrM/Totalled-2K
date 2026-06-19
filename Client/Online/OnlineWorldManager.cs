using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineWorldManager : MonoBehaviour
{
    public Dictionary<byte,OnlinePlayer> players = new Dictionary<byte,OnlinePlayer>();

    public GameObject playerPrefab;

    List<sv_PlayerInfo> playersToAddOnUnityThread = new List<sv_PlayerInfo>();
    private void Awake()
    {
        Cache.onlineWorldManager = this;
    }
    /*
    private void Update()
    {
        if (playersToAddOnUnityThread.Count> 0)
        {
            for (int i = 0; i < playersToAddOnUnityThread.Count; i++)
            {
                if (!players.ContainsKey(playersToAddOnUnityThread[i].uuid)){
                    GameObject player = Instantiate(playerPrefab);
                    players.Add(playersToAddOnUnityThread[i].uuid, player.GetComponent<OnlinePlayer>());
                    print("New player added");
                }
                else{
                    print("huh");
                }
                players[playersToAddOnUnityThread[i].uuid].UpdatePlayerInfo(playersToAddOnUnityThread[i]);
            }
            playersToAddOnUnityThread.Clear();
        }
    }
    */
    public void AddPlayer(sv_PlayerInfo playerInfo)
    {
        if (!players.ContainsKey(playerInfo.uuid))
        {
            GameObject player = Instantiate(playerPrefab);
            players.Add(playerInfo.uuid, player.GetComponent<OnlinePlayer>());
        }
        players[playerInfo.uuid].UpdatePlayerInfo(playerInfo);
    }
    public void RemovePlayer(byte uuid)
    {
        if (players.ContainsKey(uuid)){
            Destroy(players[uuid].gameObject);
            players.Remove(uuid);
        }
        else{
            Debug.LogError($"Attempted to remove player with uuid {uuid.ToString()}, however no such player exists");
        }
    }
}