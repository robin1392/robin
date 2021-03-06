using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR || UNITY_STANDALONE_LINUX
using Aws.GameLift.Server;
#endif
using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.KCP;
using Mirage.Logging;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;
using Service.Template;
using UnityEngine;

namespace MirageTest.Scripts
{
    [RequireComponent(typeof(RWNetworkServer))]
    public class ServerGameLogic : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ServerGameLogic));
        
        public RWNetworkServer server;
        private ServerObjectManager _serverObjectManager;
        public ServerObjectManager ServerObjectManager => _serverObjectManager;

        public GameState gameStatePrefab;
        public PlayerState playerStatePrefab;
        public TowerActorProxy towerActorProxyPrefab;
        public GuardianActorProxy guardianActorProxyPrefab;
        public BossActorProxy bossActorProxyPrefab;
        public DiceActorProxy diceActorProxyPrefab;
        
        public GameModeBase _gameMode;

        public PLAY_TYPE modeType;
        public bool hostMode;

        bool NoPlayers =>
            _serverObjectManager
                .SpawnedObjects
                .Any(kvp => kvp.Value.GetComponent<PlayerProxy>() != null) == false;

        public bool IsEnd => _gameMode?.IsGameEnd ?? false;

        private void Awake()
        {
            server = GetComponent<RWNetworkServer>();
            _serverObjectManager = GetComponent<ServerObjectManager>();
            server.Disconnected.AddListener(OnClientDisconnected);
        }

        private void OnClientDisconnected(INetworkPlayer arg0)
        {
            _gameMode?.OnClientDisconnected(arg0);
            CheckGameSession().Forget();
            
            if (arg0?.Identity != null)
            {
                logger.Log($"OnClientDisconnected {arg0.Identity.NetId}");    
            }
            else
            {
                logger.Log($"OnClientDisconnected identity is null.");
            }
        }

        async UniTask CheckGameSession()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(60));
            if (NoPlayers)
            {
                await EndGameSession();
            }
        }
        
        private async void Start()
        {
            await StartServerLogic();
        }

        async UniTask StartServerLogic()
        {
            logger.Log($"StartServer");

            while (!server.Active)
            {
                await UniTask.Yield();
            }

            while (server.MatchData.PlayerInfos.Count < 2)
            {
                await UniTask.Yield();
            }

            if (TableManager.Get().Loaded == false)
            {
                var port = GetComponent<KcpTransport>().Port;
                var tableDataPath = $"{Application.dataPath}/../../TableData_{port}/";
                TableManager.Get().Init(tableDataPath);    
            }

            var prefabHolder = new PrefabHolder()
            {
                PlayerState = playerStatePrefab,
                GameState = gameStatePrefab,
                TowerActorProxyPrefab = towerActorProxyPrefab,
                GuardianActorProxyPrefab = guardianActorProxyPrefab,
                BossActorProxyPrefab =  bossActorProxyPrefab,
                DiceActorProxyPrefab = diceActorProxyPrefab,
            };
            
            switch (modeType)
            {
                case PLAY_TYPE.BATTLE:
                    _gameMode = new BattleMode(prefabHolder, _serverObjectManager);  //TODO: 추후에 풀링에 문제가 없으면 프리팹이 아닌 풀러를 넘긴다.
                    break;
                case PLAY_TYPE.CO_OP:
                    _gameMode = new CoopMode(prefabHolder, _serverObjectManager);
                    break;
                case PLAY_TYPE.ActorDev:
                    _gameMode = new ActorDevMode(prefabHolder, _serverObjectManager);
                    break;
                case PLAY_TYPE.Tutorial:
                    _gameMode = new BattleModeTutorial(prefabHolder, _serverObjectManager);
                    break;
                
            }
            
            logger.Log($"OnBeforeGameStart");

            await _gameMode.OnBeforeGameStart();

            var playerCount = server.MatchData.PlayerInfos.Count(p => p.isPlayer);
            await WaitForPlayers(playerCount).TimeoutWithoutException(TimeSpan.FromSeconds(30));

            if (NoPlayers)
            {
                logger.Log($"NoPlayers");
                EndGameSession().Forget();
                return;
            }

            _gameMode.GameState.state = EGameState.Playing;
            
            var aiTask = CreateAITask();
            if (aiTask.Count > 0)
            {
                aiTask.Add(_gameMode.UpdateLogic());
                await UniTask.WhenAny(aiTask.ToArray());
            }
            else
            {
                await _gameMode.UpdateLogic();
            }
        }

        List<UniTask> CreateAITask()
        {
            var list = new List<UniTask>();
            foreach (var player in server.MatchData.PlayerInfos)
            {
                if (player.isPlayer && player.enableAI == false)
                {
                    continue;
                }

                var playerState = _gameMode.GetPlayerState(player.UserId);
                var ai = new AIPlayer(playerState);
                list.Add(ai.UpdataAI());
            }

            return list;
        }

        public async UniTask EndGameSession()
        {
            logger.Log($"EndGameSession");
            
            _gameMode?.End();
            
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            server.Disconnect();

            TerminateServer();
        }

        public void TerminateServer()
        {
            if (hostMode)
            {
                return;
            }
            
#if UNITY_EDITOR || UNITY_STANDALONE_LINUX
            if (server.LocalClientActive == false)
            {
                var outcome = GameLiftServerAPI.ProcessEnding();
                Debug.Log($"GameLiftServerAPI.ProcessEnding {outcome.Success} - {outcome.Error}");
                Application.Quit();
            }
#endif
        }

        private async UniTask WaitForPlayers(int playerCount)
        {
            while (true)
            {
                var readyPlayerProxies =
                    _serverObjectManager.SpawnedObjects.Where(p =>
                    {
                        var playerProxy = p.Value.GetComponent<PlayerProxy>();
                        return (playerProxy != null && playerProxy.ready);
                    });
                
                if (readyPlayerProxies.Count() >= playerCount)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        public PlayerState GetPlayerState(string userId)
        {
            return _gameMode.GetPlayerState(userId);
        }

        public void OnTowerDestroyed(ActorProxy destroyedTower)
        {
            if (_gameMode.IsGameEnd)
            {
                return;
            }
            
            _gameMode.OnTowerDestroyed(destroyedTower);
        }

        public void ForceEnd()
        {
            _gameMode?.End();
        }

        public void OnHitDamageTower(ActorProxy actorProxy)
        {
            _gameMode.OnHitDamageTower(actorProxy);
        }

        public void OnBossDestroyed(BossActorProxy bossActorProxy)
        {
            _gameMode.OnBossDestroyed(bossActorProxy);
        }
    }

    public class PrefabHolder
    {
        public GameState GameState;
        public PlayerState PlayerState;
        public TowerActorProxy TowerActorProxyPrefab;
        public DiceActorProxy DiceActorProxyPrefab;
        public GuardianActorProxy GuardianActorProxyPrefab;
        public BossActorProxy BossActorProxyPrefab;
    }
}