using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using RandomWarsResource;
using RandomWarsResource.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MirageTest.Scripts.GameMode
{
    public abstract class GameModeBase
    {
        protected TableData<int, TDataDiceInfo> DiceInfos;
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
            DiceInfos = TableManager.Get().DiceInfo;
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

        [ClientRpc]
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
        
        protected IEnumerable<ActorProxy> CreateMinionsByPlayerField(PlayerState playerState)
        {
            var actorProxies = new List<ActorProxy>();
            for (byte fieldIndex = 0; fieldIndex < playerState.Field.Count; ++fieldIndex)
            {
                var fieldDice = playerState.Field[fieldIndex];
                if (fieldDice.IsEmpty)
                {
                    continue;
                }

                if (DiceInfos.GetData(fieldDice.diceId, out var diceInfo) == false)
                {
                    ED.Debug.LogError(
                        $"다이스정보 {fieldDice.diceId}가 없습니다. UserId : {playerState.userId} 필드 슬롯 : {fieldIndex}");
                    return Enumerable.Empty<ActorProxy>();
                }

                var diceScale = fieldDice.diceScale;
                var spawnCount = diceInfo.spawnMultiply;
                if (diceInfo.castType == (int) DICE_CAST_TYPE.MINION)
                {
                    spawnCount *= (diceScale + 1);
                }

                var deckDice = playerState.GetDeckDice(fieldDice.diceId);

                var power = diceInfo.power
                            + (diceInfo.powerUpgrade * deckDice.outGameLevel)
                            + (diceInfo.powerInGameUp * deckDice.inGameLevel);

                var maxHealth = diceInfo.maxHealth + (diceInfo.maxHpUpgrade * deckDice.outGameLevel) +
                                (diceInfo.maxHpInGameUp * deckDice.inGameLevel);
                var effect = diceInfo.effect + (diceInfo.effectUpgrade * deckDice.outGameLevel) +
                             (diceInfo.effectInGameUp * deckDice.inGameLevel);
                var attackSpeed = diceInfo.attackSpeed;

                if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.HERO)
                {
                    power *= diceScale + 1;
                    maxHealth *= diceScale + 1;
                    effect *= diceScale + 1;
                }

                for (int i = 0; i < spawnCount; ++i)
                {
                    var actorProxy = Object.Instantiate(ActorProxyPrefab);
                    actorProxy.SetDiceInfo(diceInfo);
                    actorProxy.ownerTag = playerState.ownerTag;
                    actorProxy.actorType = ActorType.MinionFromDice;
                    actorProxy.team = playerState.camp;
                    actorProxy.ownerTag = playerState.ownerTag;
                    actorProxy.spawnSlot = fieldIndex;
                    actorProxy.power = power;
                    actorProxy.maxHealth = maxHealth;
                    actorProxy.health = maxHealth;
                    actorProxy.effect = effect;
                    actorProxy.attackSpeed = attackSpeed;
                    actorProxy.diceScale = diceScale;
                    actorProxy.ingameUpgradeLevel = deckDice.inGameLevel;
                    actorProxies.Add(actorProxy);
                }
            }

            return actorProxies;
        }

        public void End()
        {
            IsGameEnd = true;
        }
    }
}