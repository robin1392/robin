using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public void SummonActor(ActorProxy summoner, byte summonActorId, Vector2 position)
    {
        var actorProxy = Instantiate(serverGameLogic.actorProxyPrefab, position, summoner.transform.rotation);
        actorProxy.ownerTag = summoner.ownerTag;
        actorProxy.actorType = ActorType.SummonByMinion;
        actorProxy.dataId = summonActorId;
        actorProxy.team = summoner.team;
        actorProxy.spawnSlot = 0;
        actorProxy.power = summoner.power;
        actorProxy.maxHealth = summoner.maxHealth;
        actorProxy.currentHealth = summoner.maxHealth;
        actorProxy.effect = summoner.effect;
        actorProxy.attackSpeed = summoner.attackSpeed;
        actorProxy.diceScale = summoner.diceScale;
        actorProxy.ingameUpgradeLevel = summoner.ingameUpgradeLevel;
        actorProxy.spawnTime = (float) Time.Time;
        serverGameLogic.ServerObjectManager.Spawn(actorProxy.NetIdentity);
    }
}
