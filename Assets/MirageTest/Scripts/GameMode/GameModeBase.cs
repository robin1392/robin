using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Entities;
using MirageTest.Scripts.Messages;
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
            var server = ServerObjectManager.Server as RWNetworkServer;
            var playerInfos = server.MatchData.PlayerInfos;

            var getStartSp = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.GetStartSP].value;

            var playerStates = new PlayerState[2];
            var playerInfo1 = playerInfos[0];
            playerStates[0] = SpawnPlayerState(
                playerInfo1.UserId, playerInfo1.UserNickName, getStartSp,
                playerInfo1.Deck.GuadialId,
                playerInfo1.Deck.DiceInfos.Select(d => new DeckDice()
                {
                    diceId = d.DiceId,
                    outGameLevel = d.OutGameLevel,
                    inGameLevel = 0,
                }).ToArray(), GameConstants.Player1Tag);
        
            var playerInfo2 = playerInfos[1];
            playerStates[1] = SpawnPlayerState(
                playerInfo2.UserId, playerInfo2.UserNickName, getStartSp,
                playerInfo2.Deck.GuadialId,
                playerInfo2.Deck.DiceInfos.Select(d => new DeckDice()
                {
                    diceId = d.DiceId,
                    outGameLevel = d.OutGameLevel,
                    inGameLevel = 0,
                }).ToArray(), GameConstants.Player2Tag);

            return playerStates;
        }
         
         PlayerState SpawnPlayerState(string userId, string nickName, int sp, int gudianId, DeckDice[] deck, byte tag)
         {
             //원래 코드에서는 덱인덱스를 가지고 디비에서 긁어오는 중. 매칭서버에서 긁어서 넣어두는 방향을 제안
             var playerState = Object.Instantiate(_prefabHolder.PlayerState);
             playerState.Init(userId, nickName, sp, deck, tag, gudianId);
             return playerState;
         }

        public async UniTask UpdateLogic()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            await UniTask.WhenAny(UpdateWave());
        }

        private async UniTask UpdateWave()
        {
            var vsmode = TableManager.Get().Vsmode;
            var waveTime = vsmode.KeyValues[(int) EVsmodeKey.WaveTime].value;
            var addSp = vsmode.KeyValues[(int) EVsmodeKey.AddSP].value;
            var addSpCountPerWave = 4;
            var addSpInterval = waveTime / addSpCountPerWave;
            
            while (true)
            {
                OnWave(GameState.wave);
                CheckRobotFusion();
                GameState.CountDown(waveTime);
                
                var addSpCount = 0;
                while (addSpCount < addSpCountPerWave)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(addSpInterval));
                    if (IsGameEnd)
                    {
                        break;
                    }
                    
                    addSpCount++;
                    foreach (var playerState in PlayerStates)
                    {
                        var upgradeSp = 10 + ((playerState.spGrade - 1) * 5);
                        var sp = addSp + (GameState.wave * upgradeSp);
                        playerState.sp += sp;
                        playerState.AddSpByWave(sp);
                    }
                }
                
                if (IsGameEnd)
                {
                    break;
                }
                
                GameState.wave++;
            }
        }

        private void CheckRobotFusion()
        {
            var server = ServerObjectManager.Server as RWNetworkServer;
            var robotId = 4005;
            var robots = server.ActorProxies.Where(actor => actor.dataId == robotId);
            var groupBy = robots.GroupBy(r => r.team);
            foreach (var group in groupBy)
            {
                if (group.Count() == 4)
                {
                    ActorProxy actor = null;
                    foreach (var robot in group)
                    {
                        actor = robot;
                        robot.Fusion();
                    }
                    
                    server.CreateActor(
                        4012,
                        actor.ownerTag,
                        actor.team,
                        actor.ingameUpgradeLevel,
                        actor.outgameUpgradeLevel,
                        new Vector3[] { actor.transform.position },
                        2f);
                }
                else
                {
                    foreach (var robot in group)
                    {
                        robot.Destroy(1.6f);
                    }
                }
            }
        }

        protected abstract UniTask OnBeforeGameStart();

        protected abstract void OnWave(int wave);
        
        public PlayerState GetPlayerState(string userId)
        {
            return PlayerStates.First(ps => ps.userId == userId);
        }

        [ClientRpc]
        public void OnClientDisconnected(INetworkPlayer arg0)
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
                    spawnCount *= diceScale + 1;
                }

                var deckDice = playerState.GetDeckDice(fieldDice.diceId);
                
                Stat stat = new Stat();
                if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MINION)
                {
                    stat = CalcMinionStat(diceInfo, deckDice.inGameLevel, deckDice.outGameLevel);    
                }
                else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.HERO)
                {
                    stat = CalcHeroStat(diceInfo, deckDice.inGameLevel, deckDice.outGameLevel, diceScale);
                }
                else if((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.INSTALLATION ||
                        (DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MAGIC)
                {
                    stat = CalcMagicOrInstallationStat(diceInfo, deckDice.inGameLevel, deckDice.outGameLevel, diceScale);
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
                    actorProxy.actorType = ActorType.Actor;
                    actorProxy.team = playerState.team;
                    actorProxy.spawnSlot = fieldIndex;
                    actorProxy.power = stat.power;
                    actorProxy.maxHealth = stat.maxHealth;
                    actorProxy.currentHealth = stat.maxHealth;
                    actorProxy.effect = stat.effect;
                    actorProxy.attackSpeed = diceInfo.attackSpeed;
                    actorProxy.diceScale = diceScale;
                    actorProxy.moveSpeed = diceInfo.moveSpeed;
                    actorProxy.ingameUpgradeLevel = deckDice.inGameLevel;
                    actorProxy.outgameUpgradeLevel = deckDice.outGameLevel;
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

        public static Quaternion GetRotation(bool isBottomCamp)
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

        public static Stat CalcMinionStat(TDataDiceInfo diceInfo, byte inGameLevel, byte outGameLevel)
        {
            var power = diceInfo.power
                        + (diceInfo.powerUpgrade * outGameLevel)
                        + (diceInfo.powerInGameUp * inGameLevel);
            var maxHealth = diceInfo.maxHealth + (diceInfo.maxHpUpgrade * outGameLevel) +
                            (diceInfo.maxHpInGameUp * inGameLevel);
            var effect = diceInfo.effect + (diceInfo.effectUpgrade * outGameLevel) +
                         (diceInfo.effectInGameUp * inGameLevel);

            return new Stat()
            {
                power = power,
                maxHealth = maxHealth,
                effect = effect,
            };
        }
        
        public static Stat CalcHeroStat(TDataDiceInfo diceInfo,  byte inGameLevel, byte outGameLevel, byte diceScale)
        {
            var stat = CalcMinionStat(diceInfo, inGameLevel, outGameLevel);
            return new Stat()
            {
                power = stat.power * (diceScale + 1),
                maxHealth = stat.maxHealth * (diceScale + 1),
                effect = stat.effect * (diceScale + 1),
            };
        }
        
        public static Stat CalcMagicOrInstallationStat(TDataDiceInfo diceInfo,  byte inGameLevel, byte outGameLevel, byte diceScale)
        {
            var stat = CalcMinionStat(diceInfo, inGameLevel, outGameLevel);
            
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