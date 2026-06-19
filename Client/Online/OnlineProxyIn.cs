using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Totalled;
using UnityEngine.SceneManagement;

public class OnlineProxyIn : MonoBehaviour
{
    /// How to get actions player does:
    // For each action needed to be sent over the server, Create an 'event Action'
    // Then within a script called something like 'ProxyListener' you can subscribe to said event Action, and have a function that is called when Action goes off

    public bool OutputIncomingStream;

    Dictionary<Type, Action<sv_Packet>> packetProcessors;

    public sv_ServerInfo currentServerInfo;

    private void Awake()
    {
        Cache.onlineProxyIn = this;
        packetProcessors = new Dictionary<Type, Action<sv_Packet>>
        {
            { typeof(sv_ServerInfo), (packet) => ProcessServerInfo((sv_ServerInfo)packet) },
            { typeof(sv_PlayerInfo), (packet) => ProcessPlayerInfo((sv_PlayerInfo)packet) },
            { typeof(sv_PlayerDisconnected), (packet) => ProcessPlayerDisconnected((sv_PlayerDisconnected)packet) },
            { typeof(sv_Movement), (packet) => ProcessMovement((sv_Movement)packet) },
            { typeof(sv_EnteredGameValidation), (packet) => ProcessEnteredGameValidation() },
            { typeof(sv_Orientation), (packet) => ProcessOrientation((sv_Orientation)packet) }
        };
    }
    public void ProcessReceivedData(byte[] receivedBytes, int byteCount)
    {
        if (OutputIncomingStream) { Cache.terminal.Print($"Received stream of bytes from server: [{Utilities.ToReadableByteArray(receivedBytes)}]"); }
        sv_Packet[] packets = PacketSerializer.DeserializePacketBatch(receivedBytes);

        for (int i = 0; i < packets.Length; i++)
        {
            Type packetType = packets[i].GetType();
            //Cache.terminal.Print("Packet type: " + packetType.Name);
            try
            {
                if (packetProcessors.TryGetValue(packetType, out var processAction))
                {
                    processAction(packets[i]);
                }
                else
                {
                    Debug.LogError("Unknown packet type received: " + packetType);
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($"Error processing packet of type {packetType.Name}: {e.Message}");
            }
        }
    }
    public void ProcessServerInfo(sv_ServerInfo serverInfo)
    {
        if (Client.currentConnectionState == OnlineState.OutGame){
            currentServerInfo = serverInfo;
            Cache.onlineProxyOut.tps = serverInfo.tps;
            Cache.terminal.Print($"Loading server map '{serverInfo.instance}'");
            Cache.instanceManagement.GlobalThreadLoadSceneName = serverInfo.instance;
            Cache.instanceManagement.GlobalThreadStartLoadingScene = true;
        }
    }
    public void ProcessPlayerInfo(sv_PlayerInfo playerInfo)
    {
        Cache.onlineWorldManager.AddPlayer(playerInfo);
    }
    public void ProcessPlayerDisconnected(sv_PlayerDisconnected playerDisconnected)
    {
        Cache.onlineWorldManager.RemovePlayer(playerDisconnected.uuid);
    }
    public void ProcessMovement(sv_Movement movement)
    {
        Cache.onlineWorldManager.players[movement.uuid].UpdateMovement(movement);
    }
    public void ProcessOrientation(sv_Orientation orientation)
    {
        Cache.onlineWorldManager.players[orientation.uuid].UpdateOrientation(orientation);
    }
    public void ProcessEnteredGameValidation()
    {
        Client.currentConnectionState = OnlineState.InGame;
    }
}
