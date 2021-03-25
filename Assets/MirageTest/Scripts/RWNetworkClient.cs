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
using Service.Template;
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
        public List<TowerActorProxy> Towers = new List<TowerActorProxy>();
        public List<BossActorProxy> Bosses = new List<BossActorProxy>();
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
        public Global.PLAY_TYPE PlayType;

        public static RWNetworkClient Get()
        {
            return FindObjectOfType<RWNetworkClient>();
        }

        public string lastConnectServerIp;
        public bool playing;                

        public async UniTask RWConnectAsync(string serverIp)
        {
            await RWConnectAsync(serverIp, GetComponent<KcpTransport>().Port);
        }
        
        public async UniTask RWConnectAsync(string serverIp, ushort port)
        {
            lastConnectServerIp = serverIp;
            GetComponent<KcpTransport>().Port = port;
            logger.Log($"[RWConnectAsync] ip:{serverIp} port:{port}");
            
            var retryCount = 1;
            var count = 0;
            bool success = false;
            while (count < retryCount)
            {
                count++;
                try
                {
                    await ConnectAsync(serverIp);
                    success = true;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                    logger.Log($"Retry {count}");
                    continue;
                }
                
                break;
            }

            if (success == false)
            {
                InGameManager.Get().LeaveRoom();
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


        [Button]
        public void GiveUp()
        {
            GetLocalPlayerProxy()?.GiveUp();
        }
        
        [Button]
        public void Pause()
        {
            GetLocalPlayerProxy()?.ClientPause();
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
            //TODO: 크래시리틱스로 보내고 유저에게 문제 발생 팝업 띄워주고 메인으로 뱉는다.
            logger.LogError($"[ServerException] {arg2.message}");
        }

        private void OnDisconnected()
        {
            if (playing)
            {
                Reconnect();
            }
        }

        public void OnMatchData(MatchDataMessage msg)
        {
            Player1 = msg.Player1;
            Player2 = msg.Player2;

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

            PlayType = msg.PlayType == PLAY_TYPE.CO_OP ? Global.PLAY_TYPE.COOP : Global.PLAY_TYPE.BATTLE;
                
            ShowMatchPopup().Forget();
        }

        async UniTask ShowMatchPopup()
        {
            await UniTask.Yield();
            
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            
            UI_InGamePopup.Get().InitUIElement(Player1, Player2);
            
            await UniTask.Delay(TimeSpan.FromSeconds(2.0f));

            while (GameState != null && GameState.state != EGameState.Playing)
            {
                await UniTask.Yield();
            }
            
            UI_InGamePopup.Get().DisableStartPopup();
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
            if (actorProxy is TowerActorProxy towerActorProxy)
            {
                Towers.Add(towerActorProxy);
            }
            else if (actorProxy is BossActorProxy bossActorProxy)
            {
                Bosses.Add(bossActorProxy);
            }
            
            ActorProxies.Add(actorProxy);
            
            OnActorProxyChanged();
        }

        public void RemoveActorProxy(ActorProxy actorProxy)
        {
            ActorProxies.Remove(actorProxy);
            if (actorProxy is TowerActorProxy towerActorProxy)
            {
                Towers.Remove(towerActorProxy);
            }
            else if (actorProxy is BossActorProxy bossActorProxy)
            {
                Bosses.Remove(bossActorProxy);
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