using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;

public class RWNetworkServer : NetworkServer
{
    public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
    
    public ServerGameLogic serverGameLogic;
    private void Awake()
    {
        serverGameLogic = GetComponent<ServerGameLogic>();
        Connected.AddListener(OnConnected);
    }
    
    public void AddPlayerProxy(PlayerProxy playerProxy)
    {
        PlayerProxies.Add(playerProxy);
    }
    
    public void RemovePlayerProxy(PlayerProxy playerProxy)
    {
        PlayerProxies.Remove(playerProxy);
    }

    private void OnConnected(INetworkConnection arg0)
    {
        arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
    }

    private void OnPositionRelay(INetworkConnection arg1, PositionRelayMessage arg2)
    {
        foreach (var con in connections)
        {
            if (con == arg1)
            {
                continue;
            }
            
            con.SendAsync(arg2).Forget();
        }
    }
}
