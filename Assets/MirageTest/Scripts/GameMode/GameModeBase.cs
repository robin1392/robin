using System;
using System.Linq;
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
        protected bool IsGameEnd;

        public PlayerState PlayerState1 => PlayerStates[0];
        public PlayerState PlayerState2 => PlayerStates[1];

        public GameModeBase(GameState gameState, PlayerState[] playerStates, ActorProxy actorProxyPrefab,
            ServerObjectManager serverObjectManager)
        {
            GameState = gameState;
            GameState.wave = 0;
            PlayerStates = playerStates;
            ActorProxyPrefab = actorProxyPrefab;
            ServerObjectManager = serverObjectManager;
        }

        public async UniTask UpdateLogic()
        {
            OnBeforeGameStart();

            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            await UniTask.WhenAll(UpdateWave(), UpdateSp());
        }

        private async UniTask UpdateWave()
        {
            var waveInterval = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.WaveTime].value;
            while (true)
            {
                GameState.wave++;
                OnWave(GameState.wave);

                GameState.CountDownOnClient(waveInterval);
                await UniTask.Delay(TimeSpan.FromSeconds(waveInterval));

                if (IsGameEnd)
                {
                    break;
                }
            }
        }

        protected abstract void OnBeforeGameStart();

        protected abstract void OnWave(int wave);

        private async UniTask UpdateSp()
        {
            var vsmode = TableManager.Get().Vsmode;
            var waveTime = vsmode.KeyValues[(int) EVsmodeKey.WaveTime].value;
            var addSp = vsmode.KeyValues[(int) EVsmodeKey.AddSP].value;
            var addSpInterval = waveTime / 5;

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(addSpInterval));

                if (IsGameEnd)
                {
                    break;
                }
                
                foreach (var playerState in PlayerStates)
                {
                    var upgradeSp = 10 + ((playerState.spGrade - 1) * 5);
                    var sp = addSp + (GameState.wave * upgradeSp);
                    playerState.sp += sp;
                    playerState.AddSpByWave(sp);
                }
            }
        }

        public PlayerState GetPlayerState(string userId)
        {
            return PlayerStates.First(ps => ps.userId == userId);
        }

        public void OnClientDisconnected(INetworkConnection arg0)
        {
            var auth = arg0.AuthenticationData as AuthDataForConnection;
            var playerState = GetPlayerState(auth.PlayerId);
            if (GameState.masterOwnerTag != playerState.ownerTag)
            {
                return;
            }

            var newMaster = PlayerStates.FirstOrDefault(p => p.ownerTag != playerState.ownerTag);
            if (newMaster == null)
            {
                return;
            }

            GameState.masterOwnerTag = newMaster.ownerTag;
        }

        public abstract void Spawn();

        public void End()
        {
            IsGameEnd = true;
        }
    }
}