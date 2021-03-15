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
using Random = UnityEngine.Random;

namespace MirageTest.Scripts.GameMode
{
    public abstract class GameModeBase
    {
        protected TableData<int, TDataDiceInfo> DiceInfos;
        protected GameState GameState;
        protected PlayerState[] PlayerStates;
        protected PrefabHolder _prefabHolder;
        protected ServerObjectManager ServerObjectManager;
        protected bool IsGameEnd;

        public PlayerState PlayerState1 => PlayerStates[0];
        public PlayerState PlayerState2 => PlayerStates[1];


        public GameModeBase(PrefabHolder prefabHolder,
            ServerObjectManager serverObjectManager)
        {
            _prefabHolder = prefabHolder;
            ServerObjectManager = serverObjectManager;
            DiceInfos = TableManager.Get().DiceInfo;
        }
        
         protected GameState CreateGameState()
        {
            var gameState = Object.Instantiate(_prefabHolder.GameState);
            return gameState;
        }

         protected PlayerState[] CreatePlayerStates()
        {
            var authData = ServerObjectManager
                .SpawnedObjects
                .Select(kvp => kvp.Value.GetComponent<PlayerProxy>())
                .Where(p => p != null)
                .Select(p => p.ConnectionToClient.AuthenticationData as AuthDataForConnection).ToList();

            authData.Sort((a, b) =>  String.Compare(a.PlayerId, b.PlayerId, StringComparison.Ordinal));
            if (authData.Count < 2)
            {
                authData.Add(new AuthDataForConnection(){ PlayerId = "auto_setted", PlayerNickName = "auto_setted"});
            }
            
            var getStartSp = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.GetStartSP].value;
            
            var playerStates = new PlayerState[2];
            playerStates[0] = SpawnPlayerState(
                authData[0].PlayerId, authData[0].PlayerNickName, getStartSp,
                new DeckDice[]
                {
                    new DeckDice(){ diceId = 1000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 1001, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2002, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2003, inGameLevel = 0, outGameLevel = 1 },
                }, GameConstants.Player1Tag);
        
            playerStates[1] = SpawnPlayerState(
                authData[1].PlayerId, authData[1].PlayerNickName, getStartSp,
                new DeckDice[]
                {
                    new DeckDice(){ diceId = 1000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 1001, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2000, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2002, inGameLevel = 0, outGameLevel = 1 },
                    new DeckDice(){ diceId = 2003, inGameLevel = 0, outGameLevel = 1 },
                }, GameConstants.Player2Tag);
            return playerStates;
        }
         
         PlayerState SpawnPlayerState(string userId, string nickName, int sp, DeckDice[] deck, byte tag)
         {
             //원래 코드에서는 덱인덱스를 가지고 디비에서 긁어오는 중. 매칭서버에서 긁어서 넣어두는 방향을 제안
             var playerState = Object.Instantiate(_prefabHolder.PlayerState);
             playerState.Init(userId, nickName, sp, deck, tag);
             return playerState;
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
        
        protected IEnumerable<ActorProxy> CreateActorByPlayerFieldDice(PlayerState playerState)
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
                
                Stat stat = new Stat();
                if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MINION)
                {
                    stat = CalcMinionStat(diceInfo, deckDice);    
                }
                else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.HERO)
                {
                    stat = CalcHeroStat(diceInfo, deckDice, diceScale);
                }
                else if((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.INSTALLATION ||
                        (DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MAGIC)
                {
                    stat = CalcMagicOrInstallationStat(diceInfo, deckDice, diceScale);
                }

                var isBottomCamp = playerState.team == GameConstants.BottomCamp;
                var spawnTransform = isBottomCamp
                    ? FieldManager.Get().GetBottomListTs(fieldIndex)
                    : FieldManager.Get().GetTopListTs(fieldIndex);

                for (int i = 0; i < spawnCount; ++i)
                {
                    var spawnPosition = spawnTransform.position;
                    //스폰 카운트에 따라 균일한 포지션을 같는 방법을 고려해본다. 예) 1일때는 스폰트랜스폼 포지션, 2개일때는 스폰트랜스폼 포지션을 기준으로 좌우 10센티미터
                    if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MINION)
                    {
                        spawnPosition.x += Random.Range(-0.2f, 0.2f);
                        spawnPosition.z += Random.Range(-0.2f, 0.2f);    
                    }
                    //지뢰
                    else if (diceInfo.id == 1005)
                    {
                        spawnPosition = GetRandomPlayerFieldPosition();
                    }
                    
                    var actorProxy = Object.Instantiate(_prefabHolder.ActorProxy, spawnPosition, GetRotation(isBottomCamp));
                    actorProxy.SetDiceInfo(diceInfo);
                    actorProxy.ownerTag = playerState.ownerTag;
                    actorProxy.actorType = ActorType.MinionFromDice;
                    actorProxy.team = playerState.team;
                    actorProxy.spawnSlot = fieldIndex;
                    actorProxy.power = stat.power;
                    actorProxy.maxHealth = stat.maxHealth;
                    actorProxy.currentHealth = stat.maxHealth;
                    actorProxy.effect = stat.effect;
                    actorProxy.attackSpeed = diceInfo.attackSpeed;
                    actorProxy.diceScale = diceScale;
                    actorProxy.ingameUpgradeLevel = deckDice.inGameLevel;
                    actorProxy.spawnTime = (float)ServerObjectManager.Server.Time.Time;
                    //HACK:닌자
                    if (diceInfo.id == 2000)
                    {
                        actorProxy.BuffList.Add(new ActorProxy.Buff()
                        {
                            id = BuffInfos.NinjaClocking,
                            endTime = (float)ServerObjectManager.Server.Time.Time + diceInfo.effectDuration,
                        });
                    }
                    
                    actorProxies.Add(actorProxy);
                }
            }

            return actorProxies;
        }

        Quaternion GetRotation(bool isBottomCamp)
        {
            if (isBottomCamp)
            {
                return Quaternion.identity;
            }
 
            return Quaternion.Euler(0, 180f, 0);
        }
        
        public Vector3 GetRandomPlayerFieldPosition()
        {
            var x = Random.Range(-3f, 3f);
            var z = Random.Range(-2f, 2f);
            return new Vector3(x, 0, z);
        }

        public void End()
        {
            IsGameEnd = true;
        }

        Stat CalcMinionStat(TDataDiceInfo diceInfo, DeckDice deckDice)
        {
            var power = diceInfo.power
                        + (diceInfo.powerUpgrade * deckDice.outGameLevel)
                        + (diceInfo.powerInGameUp * deckDice.inGameLevel);
            var maxHealth = diceInfo.maxHealth + (diceInfo.maxHpUpgrade * deckDice.outGameLevel) +
                            (diceInfo.maxHpInGameUp * deckDice.inGameLevel);
            var effect = diceInfo.effect + (diceInfo.effectUpgrade * deckDice.outGameLevel) +
                         (diceInfo.effectInGameUp * deckDice.inGameLevel);

            return new Stat()
            {
                power = power,
                maxHealth = maxHealth,
                effect = effect,
            };
        }
        
        Stat CalcHeroStat(TDataDiceInfo diceInfo, DeckDice deckDice, byte diceScale)
        {
            var stat = CalcMinionStat(diceInfo, deckDice);
            return new Stat()
            {
                power = stat.power * (diceScale + 1),
                maxHealth = stat.maxHealth * (diceScale + 1),
                effect = stat.effect * (diceScale + 1),
            };
        }
        
        Stat CalcMagicOrInstallationStat(TDataDiceInfo diceInfo, DeckDice deckDice, byte diceScale)
        {
            var stat = CalcMinionStat(diceInfo, deckDice);
            
            return new Stat()
            {
                power = stat.power  * Mathf.Pow(1.5f, diceScale - 1),
                maxHealth = stat.maxHealth * Mathf.Pow(2f, diceScale - 1),
                effect = stat.effect * Mathf.Pow(1.5f, diceScale - 1)
            };
        }
        
        public struct Stat
        {
            public float power;
            public float maxHealth;
            public float effect;
        }
    }
}