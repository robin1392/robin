using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.Messages;
using RandomWarsProtocol;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace MirageTest.Scripts
{
    public class RWNetworkClient : NetworkClient
    {
        public bool enableActor;
        public bool enableUI;

        public List<PlayerState> PlayerStates = new List<PlayerState>(); 
        public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
        public List<ActorProxy> ActorProxies = new List<ActorProxy>();
        public List<ActorProxy> Towers = new List<ActorProxy>();
        public GameState GameState;
        public bool IsPlayingAI => GetLocalPlayerState().ownerTag== GameState.masterOwnerTag;

        private ClientObjectManager _clientObjectManager;
        public string localPlayerId;

        public MatchPlayer Player1;
        public MatchPlayer Player2;

        public static RWNetworkClient Get()
        {
            return FindObjectOfType<RWNetworkClient>();
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
        }

        private void OnConnectedRW(INetworkPlayer arg0)
        {
            arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
            arg0.RegisterHandler<MatchDataMessage>(OnMatchData);
        }

        public void OnMatchData(MatchDataMessage obj)
        {
            Player1 = obj.Player1;
            Player2 = obj.Player2;
            
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
            return PlayerStates.Find(p => p.userId == localPlayerId);
        }
        
        public PlayerState GetEnemyPlayerState()
        {
            return PlayerStates.Find(p => p.userId != localPlayerId);
        }

        public bool IsLocalPlayerTag(byte ownerTag)
        {
            return GetLocalPlayerState().ownerTag == ownerTag;
        }

        public PlayerController GetTower(byte ownerTag)
        {
            var tower = Towers.Find(p => p.ownerTag == ownerTag);
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