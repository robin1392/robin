using System;
using System.Collections.Generic;
using System.Linq;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace MirageTest.Scripts
{
    public class RWNetworkClient : NetworkClient
    {
        private static RWNetworkClient _instance;
        public static RWNetworkClient instance => _instance;

        public bool enableActor;

        public string localPlayerId;
        public byte localPlayerOwnerTag;

        public List<PlayerState> PlayerStates = new List<PlayerState>(); 
        public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();
        public List<ActorProxy> ActorProxies = new List<ActorProxy>();
        public List<ActorProxy> Towers = new List<ActorProxy>();

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("씬에 RWNetworkClient는 하나만 존재해야 합니다.");
                return;
            }

            _instance = this;
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
                if (ActorProxies.Any())
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