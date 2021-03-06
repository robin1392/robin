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
        
        [SerializeField] private float _waveInterval;
        [SerializeField] private float _waveRemainTime;
        private bool _enableUI;
        private int _lastRemainTime = int.MaxValue;

        [SyncVar(hook = nameof(SetMasterOwnerTag))] public byte masterOwnerTag;
        [SyncVar]public float factor = 1f;
        
        [SyncVar(hook = nameof(SetSuddenDeath))]public bool suddenDeath;
        [SyncVar]public float suddenDeathAttackSpeedFactor = 1f;
        [SyncVar]public float suddenDeathMoveSpeedFactor = 1f;

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

            if (newValue == EGameState.End)
            {
                client.ExtractModelBeforeGameSessionEnd();
            }
                
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

            if (newValue == 1)
            {
                UI_InGame.Get().startFight.SetActive(true);
                UI_InGame.Get().startAnimator.SetTrigger("Fight");
            }

            if (suddenDeath)
            {
                UI_InGame.Get().waveAnimator.SetTrigger("SuddenDeath");
            }
            else
            {
                UI_InGame.Get().waveAnimator.SetTrigger("Change");   
            }
        }
        
        public void SetSuddenDeath(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                UI_InGame.Get().EnableSuddenDeath();
                UI_InGame.Get().waveAnimator.SetTrigger("SuddenDeath");
            }
        }
        
        public void SetMasterOwnerTag(byte oldValue, byte newValue)
        {
            var client = Client as RWNetworkClient;
            
            var localPlayerState = client.GetLocalPlayerState();
            
            //?????? ?????? ?????? ????????? ??? GameState??? PlayerState?????? ?????? ?????? ??? ??? ??????.
            //?????? ????????? ?????? ?????? ?????? ???????????? ???????????? AI????????? ???????????? ?????? ????????????????????? ????????????.
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
            
            UI_InGame.Get().SetSpawnTime(1 - (_waveRemainTime / _waveInterval));
            var remainTimeInt = Mathf.CeilToInt(_waveRemainTime);
             
            string remainTimeStr = $"{remainTimeInt:F0}";
            WorldUIManager.Get().SetTextSpawnTime(remainTimeStr);

            if (_lastRemainTime == remainTimeInt || _lastRemainTime < remainTimeInt)
            {
                return;
            }

            if (wave > 0)
            {
                return;
            }

            UI_InGame.Get().startAnimator.gameObject.SetActive(true);
                
            if (remainTimeInt != 0)
            {
                UI_InGame.Get().startNumber.text = remainTimeStr;    
                UI_InGame.Get().startAnimator.SetTrigger("Number");    
            }
            
            _lastRemainTime = remainTimeInt;
        }

        private float CalcWaveRemainTime(float value)
        {
            var mod = (value % 0.25f);
            if (mod == 0 && value > 0)
            {
                return 1f;
            }
            return  mod / 0.25f;
        }

        public void CountDown(float waveInterval)
        {
            if (IsLocalClient)
            {
                CountDownInternal(waveInterval);
                return;
            }
            CountDownOnClient(waveInterval);
        }
        
        [ClientRpc]
        public void CountDownOnClient(float waveInterval)
        {
            CountDownInternal(waveInterval);
        }

        void CountDownInternal(float waveInterval)
        {
            _waveInterval = waveInterval;
            _waveRemainTime = waveInterval;

            if (_enableUI)
            {
                var defaultWaveTime = 20;
                UI_InGame.Get().waveFlow.factor = defaultWaveTime / _waveInterval;
            }
        }
    }
}