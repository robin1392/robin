using System;
using ED;
using Mirage;
using MirageTest.Scripts.Messages;
using Service.Template;
using UnityEngine;
using UnityEngine.Events;

namespace MirageTest.Scripts.Entities
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class GameState : NetworkBehaviour
    {
        [SyncVar(hook = nameof(SetState))] public EGameState state;
        [SyncVar(hook = nameof(SetWave))] public int wave;
        public UnityEvent<int> WaveEvent;
        
        private float _waveInterval;
        private float _waveRemainTime;
        private bool _enableUI;

        [SyncVar(hook = nameof(SetMasterOwnerTag))] public byte masterOwnerTag;
        
        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }
            
            NetIdentity.OnStartServer.AddListener(StartServer);
            NetIdentity.OnStopServer.AddListener(StopServer);
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStopClient.AddListener(StopClient);
        }

        private void StartServer()
        {
            if (Server.LocalClientActive)
            {
                StartClient();
            }
        }
        
        private void StopServer()
        {
            if (Server.LocalClientActive)
            {
                StopClient();
            }
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
            _enableUI = client.enableUI;
            
            SetWave(wave, wave);
            SetState(state, state);
        }
        
        public void SetState(EGameState oldValue, EGameState newValue)
        {
            var client = Client as RWNetworkClient;
            client.playing = newValue == EGameState.Playing;
                
            if (_enableUI == false)
            {
                return;
            }
        }

        public void SetWave(int oldValue, int newValue)
        {
            if (_enableUI == false)
            {
                return;
            }
            
            WaveEvent.Invoke(newValue);
            
            WorldUIManager.Get().SetWave(newValue);
            WorldUIManager.Get().RotateTimerIcon();
        }
        
        public void SetMasterOwnerTag(byte oldValue, byte newValue)
        {
            var client = Client as RWNetworkClient;
            
            var localPlayerState = client.GetLocalPlayerState();
            
            //게임 시작 시나 재입장 시 GameState가 PlayerState보다 늦게 생성 될 수 있다.
            //아래 루틴은 게임 진행 중에 상대방이 나갔을때 AI조작을 넘겨주기 위한 동작이기때문에 무시한다.
            if (localPlayerState == null)
            {
                return;
            }

            var isPlayingAI = localPlayerState.ownerTag == newValue;
            foreach (var actorProxy in client.ActorProxies)
            {
                actorProxy.EnableAI(isPlayingAI);
            }
        }

        private void Update()
        {
            if (IsLocalClient == false && IsServer)
            {
                return;
            }

            if (_waveRemainTime <= 0)
            {
                return;
            }
            
            _waveRemainTime -= Time.deltaTime;
            
            if (_enableUI == false)
            {
                return;
            }
            
            WorldUIManager.Get().SetSpawnTime( 1 - (_waveRemainTime / _waveInterval));
            WorldUIManager.Get().SetTextSpawnTime(_waveRemainTime);
        }

        public void CountDown(int waveInterval)
        {
            if (IsLocalClient)
            {
                CountDownInternal(waveInterval);
                return;
            }
            CountDownOnClient(waveInterval);
        }
        
        [ClientRpc]
        public void CountDownOnClient(int waveInterval)
        {
            CountDownInternal(waveInterval);
        }

        void CountDownInternal(int waveInterval)
        {
            _waveInterval = waveInterval;
            _waveRemainTime = waveInterval;
        }
    }
}