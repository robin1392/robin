using System;
using ED;
using Mirage;
using UnityEngine;

namespace MirageTest.Scripts.Entities
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class GameState : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetWave))] public int wave;
        private float _waveInterval;
        private float _waveRemainTime;

        [SyncVar(hook = nameof(SetMasterOwnerTag))] public byte masterOwnerTag;
        
        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStopClient.AddListener(StopClient);
        }

        private void StopClient()
        {
            var client = Client as RWNetworkClient;
            client.GameState = null;
        }

        private void StartClient()
        {
            var client = Client as RWNetworkClient;
            client.GameState = this;
        }

        public void SetWave(int oldValue, int newValue)
        {
            WorldUIManager.Get().SetWave(newValue);
            WorldUIManager.Get().RotateTimerIcon();
        }
        
        public void SetMasterOwnerTag(byte oldValue, byte newValue)
        {
            var client = Client as RWNetworkClient;
            var isPlayingAI = client.localPlayerOwnerTag == newValue;
                
            foreach (var actorProxy in client.ActorProxies)
            {
                actorProxy.EnableClientCombatLogic(isPlayingAI);
            }

            if (!client.enableActor)
            {
                return;
            }
            
            foreach (var tower in client.Towers)
            {
                var playerController = tower.baseStat as PlayerController;
                playerController.isPlayingAI = isPlayingAI;
            }
        }

        private void Update()
        {
            if (IsServer)
            {
                return;
            }

            if (_waveRemainTime <= 0)
            {
                return;
            }
            
            _waveRemainTime -= Time.deltaTime;
            WorldUIManager.Get().SetSpawnTime( 1 - (_waveRemainTime / _waveInterval));
            WorldUIManager.Get().SetTextSpawnTime(_waveRemainTime);
        }

        [ClientRpc]
        public void CountDownOnClient(int waveInterval)
        {
            _waveInterval = waveInterval;
            _waveRemainTime = waveInterval;
        }
    }
}