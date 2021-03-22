using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using Mirage.Logging;
using MirageTest.Aws;
using MirageTest.Scripts;
using MirageTest.Scripts.GameMode;
using MirageTest.Scripts.Messages;
using RandomWarsProtocol;
using Service.Core;
using UnityEngine;
using Debug = ED.Debug;

public class RWNetworkServer : NetworkServer
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(RWNetworkServer));
    
    public List<ActorProxy> ActorProxies = new List<ActorProxy>();
    public List<ActorProxy> Towers = new List<ActorProxy>();
    public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
    
    public ServerGameLogic serverGameLogic;

    public MatchData MatchData = new MatchData();

    public bool downLoadGameData = false;

    public static RWNetworkServer Get()
    {
        return FindObjectOfType<RWNetworkServer>();
    }

    private void Awake()
    {
        serverGameLogic = GetComponent<ServerGameLogic>();
        Connected.AddListener(OnConnected);
        Authenticated.AddListener(OnAuthed);

        if (downLoadGameData)
        {
            TableManager.Get().Init(Application.persistentDataPath + "/Resources/");
        }
    }

    private void OnAuthed(INetworkPlayer arg0)
    {
        if (LocalClientActive)
        {
            return;
        }

        SendMatchData(arg0);
    }

    void SendMatchData(INetworkPlayer arg0)
    {
        arg0.Send(new MatchDataMessage()
        {
            PlayType = serverGameLogic.modeType,
            Player1 = MatchData.PlayerInfos[0],
            Player2 = MatchData.PlayerInfos[1],
        });
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
            serverGameLogic.OnTowerDestroyed(actorProxy);
        }
    }

    private void OnConnected(INetworkPlayer arg0)
    {
        arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
        arg0.RegisterHandler<CreateActorMessage>(OnCreateActor);
        
        if (LocalClientActive)
        {
           SendMatchData(arg0);
        }
    }

    private void OnCreateActor(CreateActorMessage msg)
    {
        CreateActorWithDiceId(msg.diceId, msg.ownerTag, msg.team, msg.inGameLevel, msg.outGameLevel, msg.positions, msg.delay);
    }

    private void OnPositionRelay(INetworkPlayer arg1, PositionRelayMessage arg2)
    {
        foreach (var player in Players)
        {
            if (player == null)
            {
                continue;
            }

            if (player.Identity == null)
            {
                continue;
            }
            
            if (player == arg1)
            {
                continue;
            }
            
            player.Send(arg2);
        }
    }

    public void CreateActorWithDiceId(int diceId, byte ownerTag, byte team, byte inGameLevel, byte outGameLevel, Vector3[] positions, float delay)
    {
        StartCoroutine(CreateActorCoroutine(diceId, ownerTag, team, inGameLevel, outGameLevel, positions, delay));
    }

    private IEnumerator CreateActorCoroutine(int diceId, byte ownerTag, byte team, byte inGameLevel, byte outGameLevel, Vector3[] positions, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        
        if (TableManager.Get().DiceInfo.GetData(diceId, out var diceInfo) == false)
        {
            logger.LogError($"다이스정보 {diceId}가 없습니다.");
            yield break;
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
    
    public void CreateActorWithGuardianId(int guadianId, byte ownerTag, byte team, Vector3 position)
    {
        if (TableManager.Get().GuardianInfo.GetData(guadianId, out var tGuardianInfo) == false)
        {
            logger.LogError($"존재하지 않는 수호자 아이디 입니다. {guadianId}");
            return;
        }
            
        var isBottomCamp = team == GameConstants.BottomCamp;
        var actorProxyPrefab = serverGameLogic.actorProxyPrefab;
        var actorProxy = Instantiate(actorProxyPrefab, position, GameModeBase.GetRotation(isBottomCamp));
        actorProxy.actorType = ActorType.Guardian;
        actorProxy.dataId = guadianId;
        actorProxy.ownerTag = ownerTag;
        actorProxy.team = team;
        actorProxy.spawnSlot = 0;
        actorProxy.power = tGuardianInfo.power;
        actorProxy.maxHealth = tGuardianInfo.maxHealth;
        actorProxy.currentHealth = tGuardianInfo.maxHealth * 100;
        actorProxy.effect = tGuardianInfo.effect;
        actorProxy.attackSpeed = tGuardianInfo.attackSpeed;
        actorProxy.spawnTime = (float) Time.Time;
        //충분히 긴 시간 버프를 준다.
        actorProxy.BuffList.Add(new ActorProxy.Buff()
        {
            id = BuffInfos.HalfDamage,
            endTime = float.MaxValue,
        });

        serverGameLogic.ServerObjectManager.Spawn(actorProxy.NetIdentity);
    }

    public void OnGameEnd(List<MatchReport> matchReports)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_LINUX
        FindObjectOfType<SQSService>()?.SendMessage(ToMatchResults(matchReports)).Forget();
#endif
        foreach (var report in matchReports)
        {
            var playerProxy = PlayerProxies.FirstOrDefault(p => report.UserId == p.userId);
            if (playerProxy == null)
            {
                continue;
            }
            
            playerProxy.EndGame(playerProxy.ConnectionToClient, report);
        }
    }

    List<UserMatchResult> ToMatchResults(List<MatchReport> matchReport)
    {
        return matchReport.Select(report =>
        {
            var isVictory = (report.GameResult == GAME_RESULT.VICTORY ||
                             report.GameResult == GAME_RESULT.VICTORY_BY_DEFAULT);

            return new UserMatchResult()
            {
                MatchResult = isVictory ? 1 : 2,
                UserId = report.UserId,
                listReward = report.NormalRewards.Concat(report.PerfectRewards).Concat(report.StreakRewards).ToList(),
                AdReward = report.LoseReward,
            };
        }).ToList();
    }

    public void Finalize()
    {
        serverGameLogic.ForceEnd();
    }

    public void SetGameMode(string gameMode)
    {
        if (gameMode == "deathmatch")
        {
            serverGameLogic.modeType = PLAY_TYPE.BATTLE;
        }
        else if (gameMode == "coop")
        {
            serverGameLogic.modeType = PLAY_TYPE.CO_OP;
        }
    }

    public void OnClientPause(string userId)
    {
        var playerState = serverGameLogic.GetPlayerState(userId);
        serverGameLogic._gameMode.OnClientPause(playerState);
    }
}
