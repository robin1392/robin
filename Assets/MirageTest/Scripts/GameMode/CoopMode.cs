using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.Logging;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Core;
using Service.Template;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace MirageTest.Scripts.GameMode
{
    public class CoopMode : GameModeBase
    {
        private int _bossIndex;
        private readonly int _maxWave;
        private readonly int _bossCount;
        private readonly int _towerHpForGuardianSpawn1St;
        private readonly int _towerHpForGuardianSpawn2nd;
        private bool _guardian1stSpawned;
        private bool _guardian2ndSpawned;
        protected int _basicRewardWave;
        private List<int> killedBossIndices = new List<int>();

        protected List<BossActorProxy> _bosses = new List<BossActorProxy>();

        public CoopMode(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder,
            serverObjectManager)
        {
            var tableManager = TableManager.Get();
            _maxWave = tableManager.CoopMode.KeyValues[(int) ECoopModeKey.maxWave].value;
            _towerHpForGuardianSpawn1St =
                tableManager.CoopMode.KeyValues[(int) ECoopModeKey.callGuardianTowerHp_1st].value;
            _towerHpForGuardianSpawn2nd =
                tableManager.CoopMode.KeyValues[(int) ECoopModeKey.callGuardianTowerHp_2nd].value;
            _basicRewardWave = tableManager.CoopMode.KeyValues[(int) ECoopModeKey.basicrewardWave].value;

            //플레이 최대횟수라고 설명이 되어있는데 보스 카운트로 추측.
            if (tableManager.CoopMode.GetData((int) ECoopModeKey.coopmodePlayCntMax, out var bossCount))
            {
                _bossCount = bossCount.value;
            }
        }

        public override async UniTask OnBeforeGameStart()
        {
            var gameState = CreateGameState();
            var playerStates = CreatePlayerStates();
            gameState.masterOwnerTag = playerStates[0].ownerTag;
            GameState = gameState;
            PlayerStates = playerStates;

            PlayerState1.team = GameConstants.BottomCamp;
            PlayerState2.team = GameConstants.BottomCamp;

            await UniTask.Yield();

            ServerObjectManager.Spawn(gameState.NetIdentity);
            foreach (var playerState in playerStates)
            {
                ServerObjectManager.Spawn(playerState.NetIdentity);
            }

            SpawnAllyTower();
        }

        void SpawnAllyTower()
        {
            var playerTowerPosition = FieldManager.Get().GetPlayerPos(true);
            var tower = Object.Instantiate(_prefabHolder.TowerActorProxyPrefab, playerTowerPosition,
                Quaternion.identity);
            tower.team = GameConstants.BottomCamp;
            tower.ownerTag = GameConstants.ServerTag;

            var tableManager = TableManager.Get();
            var hp = tableManager.Vsmode.KeyValues[(int) EVsmodeKey.TowerHp].value;

            foreach (var playerState in PlayerStates)
            {
                foreach (var deckDice in playerState.Deck)
                {
                    if (tableManager.DiceInfo.GetData(deckDice.diceId, out var diceInfo) == false)
                    {
                        Debug.LogError($"존재하지않는 다이스 아이디입니다. {deckDice.diceId}");
                        continue;
                    }

                    if (tableManager.DiceUpgrade.GetData(
                        x => x.diceLv == deckDice.outGameLevel && x.diceGrade == diceInfo.grade,
                        out TDataDiceUpgrade diceLevelUpInfo))
                    {
                        hp += diceLevelUpInfo.getTowerHp;
                    }
                }
            }

            tower.currentHealth = hp;
            tower.maxHealth = hp;

            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        public override void OnHitDamageTower(ActorProxy actorProxy)
        {
            if (_guardian1stSpawned == false)
            {
                if (actorProxy.currentHealth <= _towerHpForGuardianSpawn1St)
                {
                    _guardian1stSpawned = true;
                    var playerState = PlayerState1;
                    Server.CreateActorWithGuardianId(playerState.guardianId, actorProxy.ownerTag, actorProxy.team,
                        actorProxy.transform.position);
                }
            }

            if (_guardian2ndSpawned == false)
            {
                if (actorProxy.currentHealth <= _towerHpForGuardianSpawn2nd)
                {
                    _guardian2ndSpawned = true;
                    var playerState = PlayerState2;
                    Server.CreateActorWithGuardianId(playerState.guardianId, actorProxy.ownerTag, actorProxy.team,
                        actorProxy.transform.position);
                }
            }
        }

        protected override void OnWave(int wave)
        {
            if (_maxWave < wave)
            {
                EndGame(false, false);
                return;
            }

            UpdateBoss(wave);
            // SpawnEnemyMinions(wave);
            // SpawnAllyMinions(wave);
        }

        public override void OnBossDestroyed(BossActorProxy bossActorProxy)
        {
            _bosses.Remove(bossActorProxy);
            killedBossIndices.Add(bossActorProxy.bossIndex);
            if (killedBossIndices.Count >= _bossCount)
            {
                //TODO: 탈주자 보상지급에 대한 논의가 필요하다.
                var perfect = false;
                var tower = Server.Towers.FirstOrDefault();
                if (tower != null)
                {
                    var getDefenderTowerHp =
                        TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.GetDefenderTowerHp].value;
                    perfect = tower.currentHealth > getDefenderTowerHp * 100;
                }

                EndGame(true, perfect);
            }
        }

        private void EndGame(bool winLose, bool isPerfect)
        {
            GameState.state = EGameState.End;

            var tableManager = TableManager.Get();

            List<ItemBaseInfo> listBasicReward = null;

            //기본 보상
            if (GameState.wave >= _basicRewardWave)
            {
                int basicRewardId = tableManager.CoopMode.KeyValues[(int) ECoopModeKey.basicrewardID].value;
                int basicRewardValue = tableManager.CoopMode.KeyValues[(int) ECoopModeKey.basicrewardValue].value;

                if (basicRewardId > 0)
                {
                    CreateBoxReward(basicRewardId, basicRewardValue, out listBasicReward);
                }
            }

            var bossRewards = new List<ItemBaseInfo>();
            //보스 보상
            foreach (var killedBossIndex in killedBossIndices)
            {
                if (tableManager.CoopMode.GetData((int) ECoopModeKey.bossRewardID01 + killedBossIndex,
                    out var bossRewardId) == false)
                {
                    continue;
                }

                bossRewards.Add(new ItemBaseInfo
                {
                    ItemId = bossRewardId.value,
                    Value = 1
                });
            }

            var rewards = new List<ItemBaseInfo>(bossRewards);
            if (listBasicReward != null)
            {
                rewards.AddRange(listBasicReward);
            }

            var matchReports = PlayerStates.Select(p =>
            {
                var report = new MatchReport();
                report.GameResult = winLose ? GAME_RESULT.VICTORY : GAME_RESULT.DEFEAT;
                report.IsPerfect = isPerfect;
                report.NormalRewards.AddRange(rewards);
                report.UserId = p.userId;
                return report;
            }).ToList();

            Server.OnGameEnd(matchReports);
        }

        public bool CreateBoxReward(int itemId, int value, out List<ItemBaseInfo> listRewardInfo)
        {
            listRewardInfo = new List<ItemBaseInfo>();
            TDataItemList tDataItemList;
            if (TableManager.Get().ItemList.GetData(itemId, out tDataItemList) == false)
            {
                return false;
            }

            if (tDataItemList.boxOpenType == 1)
            {
                listRewardInfo.Add(new ItemBaseInfo
                {
                    ItemId = itemId,
                    Value = value,
                });

                return true;
            }


            int totalGold = 0;
            int totalDiamond = 0;
            var rand = new Random((int) DateTime.UtcNow.Ticks % 1000000000);
            List<ItemBaseInfo> diceRewardList = new List<ItemBaseInfo>();

            foreach (var productId in tDataItemList.productId)
            {
                TDataBoxOpenInfo productInfoData = null;
                if (TableManager.Get().BoxProductInfo.GetData(productId, out productInfoData) == false)
                {
                    return false;
                }

                if (productInfoData.itemId01 == (int) EItemListKey.gold && productInfoData.itemValue01[0] != -1)
                {
                    totalGold += rand.Next(productInfoData.itemValue01[0], productInfoData.itemValue01[1]);
                }

                if (productInfoData.itemId02 == (int) EItemListKey.dia && productInfoData.itemValue02[0] != -1)
                {
                    totalDiamond += rand.Next(productInfoData.itemValue02[0], productInfoData.itemValue02[1]);
                }

                if (productInfoData.rewardIsProbability1 == false
                    || (productInfoData.rewardIsProbability1 == true &&
                        productInfoData.rewardProbability1 < rand.Next(100)))
                {
                    var tempRewarList = CreateRewardUserDice(rand, productInfoData.rewardCardGradeType1,
                        productInfoData.rewardCardGradeCnt1,
                        productInfoData.rewardCardValue1[0],
                        productInfoData.rewardCardValue1.Length == 1
                            ? productInfoData.rewardCardValue1[0]
                            : productInfoData.rewardCardValue1[1]);

                    if (tempRewarList.Count > 0)
                    {
                        foreach (var elem in tempRewarList)
                        {
                            var findReward = diceRewardList.Find(x => x.ItemId == elem.ItemId);
                            if (findReward == null)
                            {
                                diceRewardList.Add(elem);
                            }
                            else
                            {
                                findReward.Value += elem.Value;
                            }
                        }
                    }
                }

                if (productInfoData.rewardIsProbability2 == false
                    || (productInfoData.rewardIsProbability2 == true &&
                        productInfoData.rewardProbability2 < rand.Next(100)))
                {
                    var tempRewarList = CreateRewardUserDice(rand, productInfoData.rewardCardGradeType2,
                        productInfoData.rewardCardGradeCnt2,
                        productInfoData.rewardCardValue2[0],
                        productInfoData.rewardCardValue2.Length == 1
                            ? productInfoData.rewardCardValue2[0]
                            : productInfoData.rewardCardValue2[1]);

                    if (tempRewarList.Count > 0)
                    {
                        foreach (var elem in tempRewarList)
                        {
                            var findReward = diceRewardList.Find(x => x.ItemId == elem.ItemId);
                            if (findReward == null)
                            {
                                diceRewardList.Add(elem);
                            }
                            else
                            {
                                findReward.Value += elem.Value;
                            }
                        }
                    }
                }

                if (productInfoData.rewardIsProbability3 == false
                    || (productInfoData.rewardIsProbability3 == true &&
                        productInfoData.rewardProbability3 < rand.Next(100)))
                {
                    var tempRewarList = CreateRewardUserDice(rand, productInfoData.rewardCardGradeType3,
                        productInfoData.rewardCardGradeCnt3,
                        productInfoData.rewardCardValue3[0],
                        productInfoData.rewardCardValue3.Length == 1
                            ? productInfoData.rewardCardValue3[0]
                            : productInfoData.rewardCardValue3[1]);

                    if (tempRewarList.Count > 0)
                    {
                        foreach (var elem in tempRewarList)
                        {
                            var findReward = diceRewardList.Find(x => x.ItemId == elem.ItemId);
                            if (findReward == null)
                            {
                                diceRewardList.Add(elem);
                            }
                            else
                            {
                                findReward.Value += elem.Value;
                            }
                        }
                    }
                }

                if (productInfoData.rewardIsProbability4 == false
                    || (productInfoData.rewardIsProbability4 == true &&
                        productInfoData.rewardProbability4 < rand.Next(100)))
                {
                    var tempRewarList = CreateRewardUserDice(rand, productInfoData.rewardCardGradeType4,
                        productInfoData.rewardCardGradeCnt4,
                        productInfoData.rewardCardValue4[0],
                        productInfoData.rewardCardValue4.Length == 1
                            ? productInfoData.rewardCardValue4[0]
                            : productInfoData.rewardCardValue4[1]);

                    if (tempRewarList.Count > 0)
                    {
                        foreach (var elem in tempRewarList)
                        {
                            var findReward = diceRewardList.Find(x => x.ItemId == elem.ItemId);
                            if (findReward == null)
                            {
                                diceRewardList.Add(elem);
                            }
                            else
                            {
                                findReward.Value += elem.Value;
                            }
                        }
                    }
                }

                if (productInfoData.rewardIsProbability5 == false
                    || (productInfoData.rewardIsProbability5 == true &&
                        productInfoData.rewardProbability5 < rand.Next(100)))
                {
                    var tempRewarList = CreateRewardUserDice(rand, productInfoData.rewardCardGradeType5,
                        productInfoData.rewardCardGradeCnt5,
                        productInfoData.rewardCardValue5[0],
                        productInfoData.rewardCardValue5.Length == 1
                            ? productInfoData.rewardCardValue5[0]
                            : productInfoData.rewardCardValue5[1]);

                    if (tempRewarList.Count > 0)
                    {
                        foreach (var elem in tempRewarList)
                        {
                            var findReward = diceRewardList.Find(x => x.ItemId == elem.ItemId);
                            if (findReward == null)
                            {
                                diceRewardList.Add(elem);
                            }
                            else
                            {
                                findReward.Value += elem.Value;
                            }
                        }
                    }
                }
            }

            if (totalGold > 0)
            {
                listRewardInfo.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.gold,
                    Value = totalGold
                });
            }


            if (diceRewardList.Count > 0)
            {
                listRewardInfo.AddRange(diceRewardList);
            }


            if (totalDiamond > 0)
            {
                listRewardInfo.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.dia,
                    Value = totalDiamond
                });
            }

            return true;
        }

        List<ItemBaseInfo> CreateRewardUserDice(Random rand, int grade, int gradeCount, int min, int max)
        {
            List<ItemBaseInfo> rewardList = new List<ItemBaseInfo>();
            TDataDiceLevelInfo tableDiceLevelInfo;
            if (TableManager.Get().DiceLevelInfo.GetData(grade, out tableDiceLevelInfo) == false)
            {
                return rewardList;
            }

            // 대상 등급 주사위를 무작위로 섞는다.
            TDataDiceInfo[] tableDiceInfo;
            TableManager.Get().DiceInfo.GetData(x => x.grade == grade && x.enableDice == true, out tableDiceInfo);
            ShuffleArray(tableDiceInfo);

            // 주사위 데이터를 생성/갱신한다.
            for (int i = 0; i < gradeCount; i++)
            {
                int count = Math.Max(1, rand.Next(min, max));
                ItemBaseInfo reward = new ItemBaseInfo
                {
                    ItemId = tableDiceInfo[i].id,
                    Value = count
                };

                rewardList.Add(reward);
            }

            return rewardList;
        }

        static void ShuffleArray<T>(T[] array)
        {
            int random1;
            int random2;
            T tmp;
            Random rand = new Random();
            for (int index = 0; index < array.Length; ++index)
            {
                random1 = rand.Next(0, array.Length);
                random2 = rand.Next(0, array.Length);

                tmp = array[random1];
                array[random1] = array[random2];
                array[random2] = tmp;
            }
        }

        void SpawnAllyMinions(int wave)
        {
        }

        void SpawnEnemyMinions(int wave)
        {
            if (TableManager.Get().CoopModeMinion.GetData(wave, out var minionInfo) == false)
            {
                logger.LogError($"협동모드 미니언 정보가 없습니다. 웨이브{wave}");
                return;
            }

            var fieldManager = FieldManager.Get();

            for (var i = 0; i < 15; ++i)
            {
                var propertyNumber = i + 1;
                var propertyName = $"minionId{propertyNumber:D2}";
                var idPropertyInfo = minionInfo.GetType().GetProperty(propertyName);
                var diceId = (int) idPropertyInfo.GetValue(minionInfo);
                if (diceId == 0)
                {
                    continue;
                }

                var classLvPropertyInfo = minionInfo.GetType().GetProperty($"classLv{propertyNumber:D2}");
                var classLv = (int) classLvPropertyInfo.GetValue(minionInfo);

                var diceLvPropertyInfo = minionInfo.GetType().GetProperty($"diceLv{propertyNumber:D2}");
                var diceLv = (int) diceLvPropertyInfo.GetValue(minionInfo);

                Server.CreateActorWithDiceId(diceId, GameConstants.ServerTag, GameConstants.TopCamp,
                    (byte) classLv, (byte) diceLv,
                    new Vector3[] {fieldManager.GetTopListPos(i)}, 0);
            }
        }

        public override void OnTowerDestroyed(ActorProxy destroyedTower)
        {
            EndGame(false, false);
        }

        void UpdateBoss(int wave)
        {
            if (wave == 1 || wave % 5 == 0)
            {
                foreach (var boss in _bosses)
                {
                    if (boss.isHatched == false)
                    {
                        boss.isHatched = true;
                    }
                }

                var bossEgg = SpawnBoss(wave);
                if (bossEgg != null)
                {
                    _bosses.Add(bossEgg);
                }
            }
        }

        private BossActorProxy SpawnBoss(int wave)
        {
            var tableManager = TableManager.Get();

            if (_bossIndex > _bossCount - 1)
            {
                logger.LogError($"overflow boss monster index. index: {_bossIndex}");
                return null;
            }

            if (tableManager.CoopMode.GetData((int) ECoopModeKey.bossID_1st + _bossIndex, out var bossId) == false)
            {
                return null;
            }

            if (tableManager.CoopModeBossInfo.GetData(bossId.value, out var bossInfo) == false)
            {
                logger.LogError($"보스데이터를 찾을 수 없습니다. id:{bossId}");
                return null;
            }

            var actorProxy = Object.Instantiate(_prefabHolder.BossActorProxyPrefab,
                FieldManager.Get().ts_TopPlayer.transform.position, GetRotation(false));
            actorProxy.dataId = bossId.value;
            actorProxy.ownerTag = GameConstants.ServerTag;
            actorProxy.team = GameConstants.TopCamp;
            actorProxy.spawnSlot = 0;
            actorProxy.power = bossInfo.power;
            actorProxy.maxHealth = bossInfo.maxHealth;
            actorProxy.currentHealth = bossInfo.maxHealth;
            actorProxy.effect = bossInfo.effect;
            actorProxy.attackSpeed = bossInfo.attackSpeed;
            actorProxy.moveSpeed = bossInfo.moveSpeed;
            actorProxy.spawnTime = (float) Server.Time.Time;
            actorProxy.waveSpawned = wave;
            actorProxy.bossIndex = _bossIndex;
            _bossIndex++;

            ServerObjectManager.Spawn(actorProxy.NetIdentity);

            return actorProxy;
        }
    }
}