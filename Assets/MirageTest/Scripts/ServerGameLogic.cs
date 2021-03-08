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
        private readonly int _gamePlayerCount = 2;

        //TODO: State를 Server가 가지고 있는 것이 좋을 듯 하다. 
        public GameState gameStatePrefab;
        public PlayerState playerStatePrefab;
        public ActorProxy actorProxyPrefab;

        private bool _isGameStart;
        private GameModeBase _gameMode;
        
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
            _gameMode.OnClientDisconnected(arg0);
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

        async UniTask StartServerAndLogic()
        {
            _networkServer.ListenAsync().Forget();
            
            await StartServerLogic();
        }
            
        async UniTask StartServerLogic()
        {
            logger.LogError($"StartServer");

            while (!_networkServer.Active)
            {
                await UniTask.Yield();
            }

            await WaitForFirstPlayer();
            
            await WaitForPlayers();

            if (NoPlayers)
            {
                EndGameSession();
                return;
            }
            
            var gameState = SpawnGameState();
            var playerStates = SpawnPlayerStates();
            gameState.masterOwnerTag = playerStates[0].ownerTag;

            _isGameStart = true;
            
            switch (modeType)
            {
                case PLAY_TYPE.BATTLE:
                    _gameMode = new BattleMode(gameState, playerStates, actorProxyPrefab, _serverObjectManager);  //TODO: 추후에 풀링에 문제가 없으면 프리팹이 아닌 풀러를 넘긴다.
                    break;
                case PLAY_TYPE.CO_OP:
                    _gameMode = new CoopMode(gameState, playerStates, actorProxyPrefab, _serverObjectManager);
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

            _gameMode.End();
            _isGameStart = false;
            
            //TODO: GameLift 연동 시 서버 프로세스를 재사용하지 않고 종료시킨다.
            StartServerAndLogic().Forget();
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

        private GameState SpawnGameState()
        {
            var gameState = Instantiate(gameStatePrefab);
            _serverObjectManager.Spawn(gameState.NetIdentity);
            return gameState;
        }

        private PlayerState[] SpawnPlayerStates()
        {
            var authData = _serverObjectManager
                .SpawnedObjects
                .Select(kvp => kvp.Value.GetComponent<PlayerProxy>())
                .Where(p => p != null)
                .Select(p => p.ConnectionToClient.AuthenticationData as AuthDataForConnection).ToList();

            if (authData.Count < 2)
            {
                authData.Add(new AuthDataForConnection(){ PlayerId = "auto_setted", PlayerNickName = "auto_setted"});
            }
            
            var getStartSp = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.GetStartSP].value;
            
            var playerStates = new PlayerState[2];
            playerStates[0] = SpawnPlayerState(
                authData[0].PlayerId, authData[0].PlayerNickName, getStartSp,
                new DeckDice[]
                {
                    new DeckDice(){ diceId = 1000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 1001, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2002, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2003, inGameLevel = 0, outGameLevel = 1 },
                }, GameConstants.Player1Tag);
        
            playerStates[1] = SpawnPlayerState(
                authData[1].PlayerId, authData[1].PlayerNickName, getStartSp,
                new DeckDice[]
                {
                    new DeckDice(){ diceId = 1000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 1001, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2002, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2003, inGameLevel = 0, outGameLevel = 1 },
                }, GameConstants.Player2Tag);
            return playerStates;
        }

        PlayerState SpawnPlayerState(string userId, string nickName, int sp, DeckDice[] deck, byte tag)
        {
            //원래 코드에서는 덱인덱스를 가지고 디비에서 긁어오는 중. 매칭서버에서 긁어서 넣어두는 방향을 제안
            var playerState = Instantiate(playerStatePrefab);
            playerState.Init(userId, nickName, sp, deck, tag);
            _serverObjectManager.Spawn(playerState.NetIdentity);
            return playerState;
        }
        
        public PlayerState GetPlayerState(string userId)
        {
            return _gameMode.GetPlayerState(userId);
        }

        [Button]
        public void Spawn()
        {
            _gameMode?.Spawn();
        }
        
        [Button]
        public void GetDiceAll()
        {
            _gameMode.PlayerState1.GetDice();
            _gameMode.PlayerState2.GetDice();
        }

        // private async UniTask UpdateSpawn()
        // {
        //     SpawnActorsForPlayer();
        //
        //     await UniTask.Delay(TimeSpan.FromSeconds(1000));
        // }
        //
        // private void SpawnActorsForPlayer()
        // {
        //     for (int i = 0; i < 12; ++i)
        //     {
        //         SpawnActor(new Vector3(i % 4, 0, ((i / 4) + 1)), 1, 1);
        //         SpawnActor(new Vector3(i % 4, 0, -((i / 4) + 1)), 2, 2);
        //     }
        // }
        //
        //
        // void SpawnActor(Vector3 position, int owner, int team)
        // {
        //     var actor = Instantiate(actorProxyPrefab, position, Quaternion.identity);
        //     var actorProxy = actor.GetComponent<ActorProxy>();
        //     actorProxy.owner = owner;
        //     actorProxy.team = team;
        //     actorProxy.SetTeamInternal(team);
        //     _serverObjectManager.Spawn(actor);
        // }
    }
}