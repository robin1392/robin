using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.GameMode;
using MirageTest.Scripts.Messages;
using UnityEngine;
using Object = UnityEngine.Object;

public class RWNetworkServer : NetworkServer
{
    public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
    
    public ServerGameLogic serverGameLogic;

    public MatchData MatchData = new MatchData();

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

    private void OnConnected(INetworkConnection arg0)
    {
        arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
        arg0.RegisterHandler<CreateActorMessage>(OnCreateActor);
    }

    private void OnCreateActor(CreateActorMessage msg)
    {
        CreateActor(msg.diceId, msg.ownerTag, msg.team, msg.inGameLevel, msg.outGameLevel, msg.positions, msg.delay);
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

    public void CreateActor(int diceId, byte ownerTag, byte team, byte inGameLevel, byte outGameLevel, Vector3[] positions, float delay)
    {
        StartCoroutine(CreateActorCoroutine(diceId, ownerTag, team, inGameLevel, outGameLevel, positions, delay));
    }

    private IEnumerator CreateActorCoroutine(int diceId, byte ownerTag, byte team, byte inGameLevel, byte outGameLevel, Vector3[] positions, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        
        if (TableManager.Get().DiceInfo.GetData(diceId, out var diceInfo) == false)
        {
            ED.Debug.LogError(
                $"다이스정보 {diceId}가 없습니다.");
        }

        GameModeBase.Stat stat = new GameModeBase.Stat();
        if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MINION)
        {
            stat = GameModeBase.CalcMinionStat(diceInfo, inGameLevel, outGameLevel);
        }
        else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.HERO)
        {
            stat = GameModeBase.CalcHeroStat(diceInfo, inGameLevel, outGameLevel, 1);
        }
        else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.INSTALLATION ||
                 (DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MAGIC)
        {
            stat = GameModeBase.CalcMagicOrInstallationStat(diceInfo, inGameLevel, outGameLevel, 1);
        }

        var isBottomCamp = team == GameConstants.BottomCamp;
        
        
        var actorProxyPrefab = serverGameLogic.actorProxyPrefab;

        for (byte index = 0; index < positions.Length; ++index)
        {
            var position = positions[index];
            var spawnPosition = position;

            var actorProxy = Instantiate(actorProxyPrefab, spawnPosition, GameModeBase.GetRotation(isBottomCamp));
            actorProxy.SetDiceInfo(diceInfo);
            actorProxy.ownerTag = ownerTag;
            actorProxy.actorType = ActorType.Actor;
            actorProxy.team = team;
            actorProxy.spawnSlot = 0;
            actorProxy.power = stat.power;
            actorProxy.maxHealth = stat.maxHealth;
            actorProxy.currentHealth = stat.maxHealth;
            actorProxy.effect = stat.effect;
            actorProxy.attackSpeed = diceInfo.attackSpeed;
            actorProxy.diceScale = 1;
            actorProxy.ingameUpgradeLevel = inGameLevel;
            actorProxy.outgameUpgradeLevel = outGameLevel;
            actorProxy.spawnTime = (float) Time.Time;
            serverGameLogic.ServerObjectManager.Spawn(actorProxy.NetIdentity);
        }
    }
}
