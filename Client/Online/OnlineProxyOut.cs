using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totalled;
using System;
using System.Linq;

public class OnlineProxyOut : MonoBehaviour
{
    public float tps;
    [HideInInspector] public List<sv_Packet> packetsNextTick = new List<sv_Packet>();

    float currentTickTime;

    public event Action OnTicked;
    public event Action OnPreTick;
    private void Awake()
    {
        Cache.onlineProxyOut = this;
    }
    private void OnEnable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent += OnInstanceLoad;
    }
    private void OnDisable()
    {
        Cache.instanceManagement.OnInstanceLoadEvent -= OnInstanceLoad;
    }
    private void Update()
    {
        switch (Client.currentConnectionState)
        {
            case OnlineState.InGame:
                currentTickTime += Time.deltaTime;
                if (currentTickTime > (1f / tps))
                {
                    PreTick();
                    SendTickInfo();
                    currentTickTime = 0f;
                }
                break;
            case OnlineState.OutGame:

                break;
        }
    }
    public void OnInstanceLoad(bool success,string instanceName)
    {
        switch (Client.currentConnectionState)
        {
            case OnlineState.OutGame:
                if (instanceName == Cache.onlineProxyIn.currentServerInfo.instance)
                {
                    sv_MapLoadedResult mapLoadedResult = new sv_MapLoadedResult { success = success };
                    Cache.client.SendDataToServer(
                    new sv_PacketBatch
                    {
                        packets = new List<byte[]>{
                            PacketSerializer.SerializePacket(mapLoadedResult)
                        }
                    }.Serialize());
                }
                break;
        }
    }
    public void PreTick()
    {
        OnPreTick?.Invoke();
    }
    public void SendTickInfo()
    {
        if (packetsNextTick.Count > 0)
        {
            sv_PacketBatch packetBatch = new sv_PacketBatch();
            for (int i = 0; i < packetsNextTick.Count; i++){
                packetBatch.AddPacket(PacketSerializer.SerializePacket(packetsNextTick[i]));
            }
            Cache.client.SendDataToServer(packetBatch.Serialize());
            packetsNextTick.Clear();
        }
        OnTicked?.Invoke();
    }
}
