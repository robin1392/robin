using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using Mirage.KCP;
using Mirage.Logging;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.Messages;
using RandomWarsProtocol;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class RWNetworkClient : NetworkClient
    {
        static readonly UnityEngine.ILogger logger = LogFactory.GetLogger(typeof(RWNetworkClient));
        
        public bool enableActor;
        public bool enableUI;

        public List<PlayerState> PlayerStates = new List<PlayerState>(); 
        public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
        public List<ActorProxy> ActorProxies = new List<ActorProxy>();
        public List<ActorProxy> Towers = new List<ActorProxy>();
        public GameState GameState;

        public bool IsPlayingAI
        {
            get
            {
                var playerState = GetLocalPlayerState();
                if (playerState == null)
                {
                    return false;
                }

                if (GameState == null)
                {
                    return false;
                }
                
                return playerState.ownerTag== GameState.masterOwnerTag;       
            }
        }

        private ClientObjectManager _clientObjectManager;
        
        public string LocalUserId;
        public string LocalNickName;
        public string PlayerSessionId;

        public MatchPlayer Player1;
        public MatchPlayer Player2;
        public MatchPlayer LocalMatchPlayer;
        public MatchPlayer OtherMatchPlayer;

        public static RWNetworkClient Get()
        {
            return FindObjectOfType<RWNetworkClient>();
        }

        public string lastConnectServerIp;
        public async UniTask RWConnectAsync(string serverIp)
        {
            await RWConnectAsync(serverIp, GetComponent<KcpTransport>().Port);
        }
        
        public async UniTask RWConnectAsync(string serverIp, ushort port)
        {
            lastConnectServerIp = serverIp;
            logger.LogError($"[RWConnectAsync] ip:{serverIp} port:{port}");
            
            var retryCount = 3;
            var count = 0;
            while (count < retryCount)
            {
                count++;
                try
                {
                    await ConnectAsync(serverIp, port);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    logger.Log($"Retry {count}");
                    continue;
                }
                
                break;
            }
        }
        
        [Button]
        public void ForceDisconnect()
        {
            Disconnect();
        }
        
        [Button]
        public void Reconnect()
        {
            RWConnectAsync(lastConnectServerIp).Forget();
        }

        private void Awake()
        {
            if (enableUI)
            {
                InGameManager.Get().InitClient(this);
                UI_InGame.Get().InitClient(this);
                UI_DiceField.Get().InitClient(this);
            }

            _clientObjectManager = GetComponent<ClientObjectManager>();
            Connected.AddListener(OnConnectedRW);
            Disconnected.AddListener(OnDisconnected);
        }

        private void OnConnectedRW(INetworkPlayer arg0)
        {
            arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
            arg0.RegisterHandler<MatchDataMessage>(OnMatchData);
            arg0.RegisterHandler<ServerExceptionMessage>(OnServerException);
        }

        private void OnServerException(ServerExceptionMessage arg2)
        {
            logger.LogError($"[ServerException] {arg2.message}");
        }

        private void OnDisconnected()
        {
            
        }

        public void OnMatchData(MatchDataMessage obj)
        {
            Player1 = obj.Player1;
            Player2 = obj.Player2;

            if (Player1.UserId == LocalUserId)
            {
                LocalMatchPlayer = Player1;
                OtherMatchPlayer = Player2;
            }
            else
            {
                LocalMatchPlayer = Player2;
                OtherMatchPlayer = Player1;
            }

            if (enableUI == false)
            {
                return;
            }
            
            ShowMatchPopup().Forget();
        }

        async UniTask ShowMatchPopup()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            UI_InGamePopup.Get().InitUIElement(Player1, Player2);
        }

        private void OnPositionRelay(PositionRelayMessage msg)
        {
            var identity = _clientObjectManager[msg.netId];
            if (identity == null)
            {
                return;
            }

            var actor = identity.GetComponent<ActorProxy>();
            actor.lastRecieved = new MsgVector2()
            {
                X = msg.positionX,
                Y = msg.positionY,
            };
        }

        public void AddPlayerState(PlayerState playerState)
        {
            PlayerStates.Add(playerState);
        }

        public void RemovePlayerState(PlayerState playerState)
        {
            PlayerStates.Remove(playerState);
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
            
            OnActorProxyChanged();
        }

        public void RemoveActorProxy(ActorProxy actorProxy)
        {
            ActorProxies.Remove(actorProxy);
            if (actorProxy.actorType == ActorType.Tower)
            {
                Towers.Remove(actorProxy);
            }

            OnActorProxyChanged();
        }

        void OnActorProxyChanged()
        {
            if (UI_InGame.Get() != null)
            {
                UI_InGame.Get().SetUnitCount(ActorProxies.Count - Towers.Count);
            }
        }

        public PlayerProxy GetLocalPlayerProxy()
        {
            if (IsLocalClient)
            { 
                return PlayerProxies.First();
            }
            
            return PlayerProxies.Find(p => p.IsLocalPlayer);
        }
        
        public PlayerState GetLocalPlayerState()
        {
            if (PlayerStates == null)
            {
                return null;
            }
            
            return PlayerStates.Find(p => p.userId == LocalUserId);
        }
        
        public PlayerState GetEnemyPlayerState()
        {
            return PlayerStates.Find(p => p.userId != LocalUserId);
        }

        public bool IsLocalPlayerTag(byte ownerTag)
        {
            return GetLocalPlayerState().ownerTag == ownerTag;
        }

        public PlayerController GetTower(byte ownerTag)
        {
            var tower = Towers.Find(p => p.ownerTag == ownerTag);
            if (tower == null)
            {
                return null;
            }
            return tower.baseStat as PlayerController;
        }

        public bool IsLocalPlayerAlly(byte team)
        {
            return GetLocalPlayerState().team == team;
        }

        public BaseStat GetHighestHealthEnemy(byte team)
        {
            //TODO: 팀별로 액터를 분리해놓는다.
            return ActorProxies.Where(actor => actor.team != team)
                .OrderByDescending(actor => actor.currentHealth)
                .First().baseStat;
        }
    }
}