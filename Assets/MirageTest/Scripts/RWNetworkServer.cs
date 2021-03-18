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
using UnityEngine;

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
        Authenticated.AddListener(OnAuthed);
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
            Player1 = MatchData.PlayerInfos[0],
            Player2 = MatchData.PlayerInfos[1],
        });
    }

    private void Start()
    {
        if (TableManager.Get().Loaded == false)
        {
            string targetPath = Path.Combine(Application.persistentDataPath + "/Resources/", "Table", "DEV");
            TableManager.Get().LoadFromFile(targetPath);
        }
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
        CreateActor(msg.diceId, msg.ownerTag, msg.team, msg.inGameLevel, msg.outGameLevel, msg.positions, msg.delay);
    }

    private void OnPositionRelay(INetworkPlayer arg1, PositionRelayMessage arg2)
    {
        foreach (var player in Players)
        {
            if (player == arg1)
            {
                continue;
            }
            
            player.Send(arg2);
        }
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

    public void OnGameEnd(List<UserMatchResult> listMatchResult)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        FindObjectOfType<SQSService>()?.SendMessage(listMatchResult).Forget();
#endif
        foreach (var result in listMatchResult)
        {
            var playerProxy = PlayerProxies.FirstOrDefault(p => result.UserId == p.userId);
            if (playerProxy == null)
            {
                continue;
            }
            playerProxy.EndGame(result);
        }
    }

    public void Finalize()
    {
        serverGameLogic.ForceEnd();
    }
}
