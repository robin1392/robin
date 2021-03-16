using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.GameMode;
using RandomWarsResource.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    [RequireComponent(typeof(RWNetworkServer))]
    public class ServerGameLogic : MonoBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(ServerGameLogic));
        
        private RWNetworkServer _networkServer;
        private ServerObjectManager _serverObjectManager;
        public ServerObjectManager ServerObjectManager => _serverObjectManager;
        
        private readonly int _gamePlayerCount = 2;

        //TODO: State를 Server가 가지고 있는 것이 좋을 듯 하다. 
        public GameState gameStatePrefab;
        public PlayerState playerStatePrefab;
        public ActorProxy actorProxyPrefab;

        private bool _isGameStart;
        public GameModeBase _gameMode;
        public bool isAIMode;
        
        public PLAY_TYPE modeType;

        bool NoPlayers =>
            _serverObjectManager
                .SpawnedObjects
                .Any(kvp => kvp.Value.GetComponent<PlayerProxy>() != null) == false;

        private void Awake()
        {
            _networkServer = GetComponent<RWNetworkServer>();
            _serverObjectManager = GetComponent<ServerObjectManager>();
            _networkServer.Disconnected.AddListener(OnClientDisconnected);
        }

        private void OnClientDisconnected(INetworkConnection arg0)
        {
            _gameMode?.OnClientDisconnected(arg0);
            CheckGameSession().Forget();
        }

        async UniTask CheckGameSession()
        {
            await UniTask.Yield();
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
            logger.LogError($"StartServer");

            while (!_networkServer.Active)
            {
                await UniTask.Yield();
            }
            
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

            _isGameStart = true;

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

            await _gameMode.UpdateLogic();
        }
        
        private void EndGameSession()
        {
            logger.LogError($"EndGameSession");
            
            _networkServer.Disconnect();
            var objs = _serverObjectManager.SpawnedObjects.Values.ToArray();
            foreach (var obj in objs)
            {
                _serverObjectManager.Destroy(obj.gameObject);
            }

            _gameMode?.End();
            _isGameStart = false;
            
            //프로세스 종료
        }
        
        private async UniTask WaitForPlayers()
        {
            while (true)
            {
                var playerProxies =
                    _serverObjectManager.SpawnedObjects.Where(kvp => kvp.Value.GetComponent<PlayerProxy>() != null);
                if (playerProxies.Count() >= _gamePlayerCount)
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
                var playerProxies =
                    _serverObjectManager.SpawnedObjects.Where(kvp => kvp.Value.GetComponent<PlayerProxy>() != null);
                if (playerProxies.Any())
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
    }

    public class PrefabHolder
    {
        public GameState GameState;
        public PlayerState PlayerState;
        public ActorProxy ActorProxy;
    }
}