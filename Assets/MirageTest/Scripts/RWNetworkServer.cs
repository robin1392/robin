using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;
using MirageTest.Scripts.Messages;
using UnityEngine;
using Object = UnityEngine.Object;

public class RWNetworkServer : NetworkServer
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(RWNetworkServer));
    
    public List<ActorProxy> ActorProxies = new List<ActorProxy>();
    public List<ActorProxy> Towers = new List<ActorProxy>();
    public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
    
    public ServerGameLogic serverGameLogic;

    public MatchData MatchData = new MatchData();

    public static RWNetworkServer Get()
    {
        return FindObjectOfType<RWNetworkServer>();
    }

    private void Awake()
    {
        serverGameLogic = GetComponent<ServerGameLogic>();
        Connected.AddListener(OnConnected);
    }

    private void Start()
    {
        string targetPath = System.IO.Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "DEV");
        TableManager.Get().LoadFromFile(targetPath);
    }

    public void AddPlayerProxy(PlayerProxy playerProxy)
    {
        PlayerProxies.Add(playerProxy);
    }
    
    public void RemovePlayerProxy(PlayerProxy playerProxy)
    {
        PlayerProxies.Remove(playerProxy);
    }
    
    public void AddActorProxy(ActorProxy actorProxy)
    {
        if (actorProxy.actorType == ActorType.Tower)
        {
            Towers.Add(actorProxy);
        }
            
        ActorProxies.Add(actorProxy);
    }

    public void RemoveActorProxy(ActorProxy actorProxy)
    {
        ActorProxies.Remove(actorProxy);
        if (actorProxy.actorType == ActorType.Tower)
        {
            Towers.Remove(actorProxy);
        }
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

    public void SummonActor(ActorProxy summoner, byte summonActorId, Vector3 position)
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
    
    public ActorProxy CreateGuadian(PlayerState playerState, Vector3 position, Quaternion rotation)
    {
        if (TableManager.Get().GuardianInfo.GetData(playerState.guadianId, out var dataGuardianInfo) == false)
        {
            logger.LogError($"가디언아이디가 존재하지 않습니다. id: {playerState.guadianId} player:{playerState.userId}");
            return null;
        }
        
        var actorProxy = Instantiate(serverGameLogic.actorProxyPrefab, position, rotation);
        actorProxy.ownerTag = playerState.ownerTag;
        actorProxy.actorType = ActorType.Actor;
        actorProxy.dataId = playerState.guadianId;
        actorProxy.team = playerState.team;
        actorProxy.spawnSlot = 0;

        actorProxy.power = dataGuardianInfo.power;
        actorProxy.maxHealth = dataGuardianInfo.maxHealth;
        actorProxy.currentHealth = dataGuardianInfo.maxHealth;
        actorProxy.effect = dataGuardianInfo.effect;
        actorProxy.attackSpeed = dataGuardianInfo.attackSpeed;
        actorProxy.spawnTime = (float) Time.Time;
        return actorProxy;
        
    }

    public void SpawnGuadian(PlayerState playerState, Vector3 position, Quaternion rotation)
    {
        var actorProxy = CreateGuadian(playerState, position, rotation);
        serverGameLogic.ServerObjectManager.Spawn(actorProxy.NetIdentity);
    }
}
