using System;
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
        
        public void SetWave(int oldValue, int newValue)
        {
            WorldUIManager.Get().SetWave(newValue);
            WorldUIManager.Get().RotateTimerIcon();
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