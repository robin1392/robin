using System;
using System.Collections.Generic;
using System.Linq;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.Messages;
using RandomWarsProtocol;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace MirageTest.Scripts
{
    public class RWNetworkClient : NetworkClient
    {
        public bool enableActor;
        public bool enableUI;

        public string localPlayerId;
        public byte localPlayerOwnerTag;

        public List<PlayerState> PlayerStates = new List<PlayerState>(); 
        public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
        public List<ActorProxy> ActorProxies = new List<ActorProxy>();
        public List<ActorProxy> Towers = new List<ActorProxy>();
        public GameState GameState;
        public bool IsPlayingAI => localPlayerOwnerTag == GameState.masterOwnerTag;

        private ClientObjectManager _clientObjectManager;

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

        private void OnConnectedRW(INetworkConnection arg0)
        {
            arg0.RegisterHandler<PlayAnimationRelayMessage>(OnPlayAnimationRelay);
            arg0.RegisterHandler<PositionRelayMessage>(OnPositionRelay);
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

        private void OnPlayAnimationRelay(PlayAnimationRelayMessage msg)
        {
            var actor = _clientObjectManager[msg.actorNetId];
            if (actor == null)
            {
                return;
            }
            
            var minion = actor.GetComponent<ActorProxy>().baseStat as Minion;
            minion?.SetAnimationTrigger(((E_AniTrigger)msg.aniId).ToString(), msg.targetNetId);
        }

        private void Start()
        {
            PoolManager.Get().MakePool();
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
                if (enableActor && ActorProxies.Any())
                {
                    var other = ActorProxies.First().baseStat as PlayerController;
                    var playerController = actorProxy.baseStat as PlayerController;
                    other.targetPlayer = playerController;
                    playerController.targetPlayer = other;
                }
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
            }
        }
        
        public bool IsLocalPlayer(string userId)
        {
            return userId == this.localPlayerId;
        }
        
        public PlayerProxy GetLocalPlayerProxy()
        {
            return PlayerProxies.Find(p => p.IsLocalPlayer);
        }
        
        public PlayerState GetLocalPlayerState()
        {
            return PlayerStates.Find(p => p.IsLocalPlayerState);
        }

        public bool IsLocalPlayerTag(byte ownerTag)
        {
            return ownerTag == this.localPlayerOwnerTag;
        }

        public PlayerController GetTower(byte ownerTag)
        {
            var tower = Towers.Find(p => p.ownerTag == ownerTag);
            return tower.baseStat as PlayerController;
        }
    }
}