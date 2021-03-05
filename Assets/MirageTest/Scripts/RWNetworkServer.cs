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
    public ServerGameLogic serverGameLogic;
    private void Awake()
    {
        serverGameLogic = GetComponent<ServerGameLogic>();
        Connected.AddListener(OnConnected);
    }

    private void OnConnected(INetworkConnection arg0)
    {
        arg0.RegisterHandler<PlayAnimationRelayMessage>(OnPlayAnimationRelay);
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

    private void OnPlayAnimationRelay(INetworkConnection arg1, PlayAnimationRelayMessage arg2)
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
