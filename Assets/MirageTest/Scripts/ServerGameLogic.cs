using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Mirage;
using Mirage.Logging;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;
using RandomWarsResource.Data;
using Service.Template;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    [RequireComponent(typeof(RWNetworkServer))]
    public class ServerGameLogic : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ServerGameLogic));
        
        private RWNetworkServer server;
        private ServerObjectManager _serverObjectManager;
        public ServerObjectManager ServerObjectManager => _serverObjectManager;
        
        private readonly int _gamePlayerCount = 2;

        //TODO: State를 Server가 가지고 있는 것이 좋을 듯 하다. 
        public GameState gameStatePrefab;
        public PlayerState playerStatePrefab;
        public ActorProxy actorProxyPrefab;
        
        public GameModeBase _gameMode;
        public bool isAIMode;
        
        public PLAY_TYPE modeType;

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
        }

        async UniTask CheckGameSession()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(60));
            if (NoPlayers)
            {
                EndGameSession();
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

            var prefabHolder = new PrefabHolder()
            {
                PlayerState = playerStatePrefab,
                GameState = gameStatePrefab,
                ActorProxy = actorProxyPrefab,
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
            }

            await _gameMode.OnBeforeGameStart();
            
            if (isAIMode)
            {
                await WaitForFirstPlayer();
            }
            else
            {
                await WaitForPlayers();    
            }

            if (NoPlayers)
            {
                EndGameSession();
                return;
            }

            _gameMode.GameState.state = EGameState.Playing;
            
            if (isAIMode)
            {
                var aiPlayer = new AIPlayer(_gameMode.PlayerState2);
                // var aiPlayer2 = new AIPlayer(_gameMode.PlayerState1);
                await UniTask.WhenAny(_gameMode.UpdateLogic(), aiPlayer.UpdataAI()); //, aiPlayer2.UpdataAI());   
            }
            else
            {
                await _gameMode.UpdateLogic();    
            }
        }
        
        private void EndGameSession()
        {
            logger.LogError($"EndGameSession");
            
            server.Disconnect();
            var objs = _serverObjectManager.SpawnedObjects.Values.ToArray();
            foreach (var obj in objs)
            {
                _serverObjectManager.Destroy(obj.gameObject);
            }

            _gameMode?.End();
        }
        
        private async UniTask WaitForPlayers()
        {
            while (true)
            {
                var readyPlayerProxies =
                    _serverObjectManager.SpawnedObjects.Where(p =>
                    {
                        var playerProxy = p.Value.GetComponent<PlayerProxy>();
                        return (playerProxy != null && playerProxy.ready);
                    });
                
                if (readyPlayerProxies.Count() >= _gamePlayerCount)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }
        
        private async UniTask WaitForFirstPlayer()
        {
            while (true)
            {
                var readyPlayerProxies =
                    _serverObjectManager.SpawnedObjects.Where(p =>
                    {
                        var playerProxy = p.Value.GetComponent<PlayerProxy>();
                        return (playerProxy != null && playerProxy.ready);
                    });
                
                if (readyPlayerProxies.Any())
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
            _gameMode.End();
        }
    }

    public class PrefabHolder
    {
        public GameState GameState;
        public PlayerState PlayerState;
        public ActorProxy ActorProxy;
    }
}