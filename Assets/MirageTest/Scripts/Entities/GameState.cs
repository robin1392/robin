using System;
using Mirage;
using UnityEngine;

namespace MirageTest.Scripts.Entities
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class GameState : NetworkBehaviour
    {
        [SyncVar] public int wave;
        [SyncVar(hook = nameof(SetWaveInterval))] public int waveInterval;
        private float _waveRemainTime;
        
        public void SetWaveInterval(int oldValue, int newValue)
        {
            _waveRemainTime = newValue;
        }

        private void Update()
        {
            _waveRemainTime -= Time.deltaTime;
        }
    }
}