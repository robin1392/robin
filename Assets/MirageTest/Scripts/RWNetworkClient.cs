using System;
using System.Collections.Generic;
using Mirage;
using MirageTest.Scripts.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class RWNetworkClient : NetworkClient
    {
        private static RWNetworkClient _instance;
        public static RWNetworkClient instance => _instance;

        public bool enableActor;

        public string localPlayerId;

        public List<PlayerState> PlayerStates = new List<PlayerState>(); 
        public List<PlayerProxy> PlayerProxies = new List<PlayerProxy>();

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
    }
}