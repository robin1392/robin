using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts.Entities;
using RandomWarsResource.Data;
using UnityEngine;

namespace MirageTest.Scripts.GameMode
{
    public abstract class GameModeBase
    {
        protected GameState GameState;
        protected PlayerState[] PlayerStates;
        protected ActorProxy ActorProxyPrefab;
        protected ServerObjectManager ServerObjectManager;
        
        public PlayerState PlayerState1 => PlayerStates[0];
        public PlayerState PlayerState2 => PlayerStates[1];
        
        public GameModeBase(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab, ServerObjectManager serverObjectManager)
        {
            GameState = gameState;
            GameState.wave = 0;
            PlayerStates = playerStates;
            ActorProxyPrefab = actorProxyPrefab;
            ServerObjectManager = serverObjectManager;
        }
        
        protected bool IsGameEnd; 
        public async UniTask UpdateLogic()
        {
            await UniTask.WhenAll(UpdateWave(), UpdateSp());
        }
        
        private async UniTask UpdateWave()
        {
            OnBeforeGameStart();
            
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            var waveInterval = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.WaveTime].value;
            while (IsGameEnd == false)
            {
                GameState.wave++;
                OnWave(GameState.wave);
                
                GameState.waveInterval = waveInterval;
                await UniTask.Delay(TimeSpan.FromSeconds(waveInterval));
            }
        }
        
        protected abstract void OnBeforeGameStart();
        
        protected abstract void OnWave(int wave);

        private async UniTask UpdateSp()
        {
            var vsmode = TableManager.Get().Vsmode;
            var waveTime = vsmode.KeyValues[(int)EVsmodeKey.WaveTime].value;
            var addSp = vsmode.KeyValues[(int)EVsmodeKey.AddSP].value;
            var addSpInterval = waveTime / 4;
            
            while (IsGameEnd == false)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(addSpInterval));

                foreach (var playerState in PlayerStates)
                {
                    var upgradeSp = 10 + ((playerState.spGrade - 1) * 5);
                    playerState.sp += (addSp + (GameState.wave * upgradeSp));    
                }
            }
        }
    }
}