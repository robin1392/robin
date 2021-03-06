using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mirage;
using Mirage.Logging;
using MirageTest.Scripts.Entities;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Core;
using Service.Template;
using UnityEngine;
using Debug = ED.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts.GameMode
{
    public class BattleMode : GameModeBase
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BattleMode));
        private readonly int endWave;
        private readonly int towerHpForGudianSpawn;
        private readonly int suddenDeathAtkSpeed;
        private readonly int suddenDeathMoveSpeed;
        private readonly int suddenDeathStartWave = 11;
        private readonly int suddenDeathSecondWave = 16;
        private readonly int suddenDeathWaveTime1;
        private readonly int suddenDeathWaveTime2;

        public bool bottomGudianSpawned;
        public bool topGudianSpawned;

        public BattleMode(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder,
            serverObjectManager)
        {
            var tableManager = TableManager.Get(); 
            endWave = tableManager.Vsmode.KeyValues[(int) EVsmodeKey.EndWave].value;
            towerHpForGudianSpawn = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
            suddenDeathAtkSpeed = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.SuddenDeathAtkSpeed].value;
            suddenDeathMoveSpeed = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.SuddenDeathMoveSpeed].value;
            suddenDeathWaveTime1 = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.SuddenDeathWaveTime1].value;
            suddenDeathWaveTime2 = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.SuddenDeathWaveTime2].value;
        }

        public override async UniTask OnBeforeGameStart()
        {
            var gameState = CreateGameState();
            var playerStates = CreatePlayerStates();
            gameState.masterOwnerTag = playerStates[0].ownerTag;
            GameState = gameState;
            PlayerStates = playerStates;

            PlayerState1.team = GameConstants.BottomCamp;
            PlayerState2.team = GameConstants.TopCamp;
            
            ServerObjectManager.Spawn(gameState.NetIdentity);
            foreach (var playerState in playerStates)
            {
                ServerObjectManager.Spawn(playerState.NetIdentity);
            }

            //???????????? ???????????? ??????????????? ?????? ?????????????????? ??? ??? ??????
            await UniTask.Yield();
            
            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            SpawnTower(PlayerState1, player1TowerPosition);

            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            SpawnTower(PlayerState2, player2TowerPosition);
        }

        void SpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = Object.Instantiate(_prefabHolder.TowerActorProxyPrefab, position, Quaternion.identity);
            tower.team = playerState.team;
            tower.ownerTag = playerState.ownerTag;

            var tableManager = TableManager.Get();
            var hp = tableManager.Vsmode.KeyValues[(int) EVsmodeKey.TowerHp].value;
            foreach (var deckDice in playerState.Deck)
            {
                if (tableManager.DiceInfo.GetData(deckDice.diceId, out var diceInfo) == false)
                {
                    Debug.LogError($"?????????????????? ????????? ??????????????????. {deckDice.diceId}");
                    continue;
                }

                if (tableManager.DiceUpgrade.GetData(
                    x => x.diceLv == deckDice.outGameLevel && x.diceGrade == diceInfo.grade,
                    out TDataDiceUpgrade diceLevelUpInfo))
                {
                    hp += diceLevelUpInfo.getTowerHp;
                }
            }

            tower.currentHealth = hp;
            
            tower.maxHealth = hp;

            ServerObjectManager.Spawn(tower.NetIdentity);
        }

        protected override void OnWave(int wave)
        {
            if (wave > endWave)
            {
                var server = ServerObjectManager.Server as RWNetworkServer;
                if (server)
                {
                    if (server.Towers.Where(t => t.currentHealth >= t.maxHealth).Count() == 2)
                    {
                        EndGamePlayerDidNothing(PlayerStates);
                        foreach (var tower in server.Towers)
                        {
                            ServerObjectManager.Destroy(tower.gameObject);
                        }
                    }
                    else
                    {
                        var ordered = server.Towers.OrderByDescending(t => t.currentHealth).ToArray();
                        var winnerTower = ordered.ElementAt(0);
                        var loserTower = ordered.ElementAt(1);
                        var winner = PlayerStates.First(p => p.ownerTag == winnerTower.ownerTag);
                        var loser = PlayerStates.First(p => p.ownerTag == loserTower.ownerTag);
                        
                        int getDefenderTowerHp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
                        EndGame(winner, loser, false, winnerTower.currentHealth > getDefenderTowerHp * 100);
                        ServerObjectManager.Destroy(loserTower.gameObject);
                    }
                }
                return;
            }

            if (GameState.suddenDeath == false)
            {
                if (wave >= suddenDeathStartWave)
                {
                    GameState.suddenDeath = true;
                }
            }

            var actorProxiesOfPlayers = new List<IEnumerable<ActorProxy>>();
            foreach (var playerState in PlayerStates)
            {
                actorProxiesOfPlayers.Add(CreateActorByPlayerFieldDice(playerState));
            }
            
            if (GameState.suddenDeath)
            {
                foreach (var actorProxiesOfPlayer in actorProxiesOfPlayers)
                {
                    foreach (var actorProxy in actorProxiesOfPlayer)
                    {
                        ApplySuddenDeathEffect(actorProxy, wave);    
                    }
                }
            }

            foreach (var actorProxiesOfPlayer in actorProxiesOfPlayers)
            {
                Spawn(actorProxiesOfPlayer).Forget();   
            }
        }

        protected override int CalcWaveTime()
        {
            if (GameState.wave >= suddenDeathSecondWave)
            {
                return suddenDeathWaveTime2;
            }
            else if (GameState.wave >= suddenDeathStartWave)
            {
                return suddenDeathWaveTime1;
            }
            
            return base.CalcWaveTime();
        }

        public override void OnHitDamageTower(ActorProxy actorProxy)
        {
            if (actorProxy.currentHealth > towerHpForGudianSpawn)
            {
                return;
            }
            
            if (actorProxy.team == GameConstants.BottomCamp)
            {
                if (bottomGudianSpawned)
                {
                    return;
                }

                bottomGudianSpawned = true;
            }
            else if (actorProxy.team == GameConstants.TopCamp)
            {
                if (topGudianSpawned)
                {
                    return;
                }

                topGudianSpawned = true;
            }
            
            var playerState = GetPlayerStateByTeam(actorProxy.team);
            var position = actorProxy.transform.position; 
            if (actorProxy.team == GameConstants.BottomCamp)
            {
                position += Vector3.forward * 3;
            }
            else
            {
                position += Vector3.back * 3;
            }
            Server.CreateActorWithGuardianId(playerState.guardianId, actorProxy.ownerTag, actorProxy.team, position);
        }

        void ApplySuddenDeathEffect(ActorProxy actorProxy, int wave)
        {
            actorProxy.attackSpeed *= (float)Math.Pow(suddenDeathAtkSpeed / 100.0f, wave - suddenDeathStartWave + 1);
            actorProxy.moveSpeed *= (float)Math.Pow(suddenDeathMoveSpeed / 100.0f, wave - suddenDeathStartWave + 1);
        }

        public override void OnTowerDestroyed(ActorProxy destroyedTower)
        {
            var server = ServerObjectManager.Server as RWNetworkServer;
            var loser = PlayerStates.First(p => p.ownerTag == destroyedTower.ownerTag);
            var winnerTower = server.Towers.First();
            var winner = PlayerStates.First(p => p.ownerTag == winnerTower.ownerTag);
            int getDefenderTowerHp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
            EndGame(winner, loser, false, winnerTower.currentHealth > getDefenderTowerHp);
        }

        private void EndGame(PlayerState winner, PlayerState loser, bool winByDefault, bool perfect)
        {
            GameState.state = EGameState.End;
            var victoryReport = new MatchReport();
            victoryReport.GameResult = winByDefault ?GAME_RESULT.VICTORY_BY_DEFAULT : GAME_RESULT.VICTORY;
            victoryReport.UserId = winner.userId;
            victoryReport.IsPerfect = perfect;

            var victoryMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == winner.userId);
            var defeatMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == loser.userId);
            
            // ?????? ?????? ??????
            victoryReport.WinStreak = (short)((victoryMatchPlayer.WinStreak > 0) ? Math.Min(victoryMatchPlayer.WinStreak + 1, 15) : 1);
            
            // ????????? ??????
            int victoryTrophy = (defeatMatchPlayer.Trophy - victoryMatchPlayer.Trophy) / 12 + 30;
            victoryReport.NormalRewards.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.thropy,
                Value = victoryTrophy,
            });

            victoryReport.NormalRewards.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.rankthropy,
                Value = victoryTrophy,
            });

            victoryReport.NormalRewards.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.seasonthropy,
                Value = 1,
            });
            
            // ?????? ?????? ??????
            int addGold = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.NormalWinReward_Gold].value;
            if (addGold != 0)
            {
                victoryReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.gold,
                    Value = addGold,
                });
            }

            int addKey = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.NormalWinReward_Key].value;
            if (addKey != 0)
            {
                victoryReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.key,
                    Value = addKey,
                });
            }

            //?????? ??????
            addGold = Math.Max(0, victoryReport.WinStreak - 1) * 5;
            if (addGold != 0)
            {
                victoryReport.StreakRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int)EItemListKey.gold,
                    Value = addGold,
                });
            }
            
            addKey = (victoryMatchPlayer.WinStreak - 1) / 3;
            if (addKey != 0)
            {
                victoryReport.StreakRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int)EItemListKey.key,
                    Value = addKey,
                });
            }
            
            // ????????? ?????? ??????
            if (perfect)
            {
                addGold = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.PerfectReward_Gold].value;
                if (addGold != 0)
                {
                    victoryReport.PerfectRewards.Add(new ItemBaseInfo
                    {
                        ItemId = (int)EItemListKey.gold,
                        Value = addGold,
                    });
                }

                addKey = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.PerfectWinReward_Key].value;
                if (addKey != 0)
                {
                    victoryReport.PerfectRewards.Add(new ItemBaseInfo
                    {
                        ItemId = (int)EItemListKey.key,
                        Value = addKey,
                    });
                }
            }
            
            
            // ?????? ????????? ??????
            var defeatReport = new MatchReport();
            defeatReport.GameResult = GAME_RESULT.DEFEAT;
            defeatReport.UserId = loser.userId;
            defeatReport.IsPerfect = perfect;

            // ?????? ?????? ??????
            defeatReport.WinStreak = (short)((defeatMatchPlayer.WinStreak < 0) ? Math.Max(defeatMatchPlayer.WinStreak - 1, -15) : -1);

            // ????????? ??????
            int defeatTrophy = ((victoryMatchPlayer.Trophy - defeatMatchPlayer.Trophy) / 12 + 30);
            defeatTrophy = Math.Min(defeatMatchPlayer.Trophy, defeatTrophy);
            
            if (defeatTrophy != 0)
            {
                // ?????? ?????????
                defeatReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.thropy,
                    Value = defeatTrophy * -1,
                });

                // ?????? ?????? ?????????
                defeatReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.rankthropy,
                    Value = defeatTrophy * -1,
                });
            }

            // ?????? ?????? ??????
            // ?????? ??????
            TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition01, out var tDataLoseUserCondition01);
            var rate = Random.Range(0, 100);
            if (rate < tDataLoseUserCondition01.value)
            {
                // ????????? ??????
                TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition02, out var tDataLoseUserCondition02);
                if (defeatMatchPlayer.WinStreak < tDataLoseUserCondition02.value * -1)
                {
                    // ????????? ????????? ??????
                    TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition03, out var tDataLoseUserCondition03);
                    if( Math.Abs(defeatMatchPlayer.Trophy  - victoryMatchPlayer.Trophy) > tDataLoseUserCondition03.value)
                    {
                        TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserRewardId, out var tDataLoseUserRewardId);
                        TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserRewardValue, out var tDataLoseUserRewardValue);

                        defeatReport.LoseReward.RewardId = Guid.NewGuid().ToString();;
                        defeatReport.LoseReward.ItemId = tDataLoseUserRewardId.value;
                        defeatReport.LoseReward.Value = tDataLoseUserRewardValue.value;
                    }
                }
            }
            
            Server.OnGameEnd(new List<MatchReport>() {victoryReport, defeatReport});
        }

        private void EndGamePlayerDidNothing(PlayerState[] losers)
        {
            GameState.state = EGameState.End;
            var matchResult1 = CreateMatchResultEndGamePlayerDidNothing(losers[0], losers[1]);
            var matchResult2 = CreateMatchResultEndGamePlayerDidNothing(losers[1], losers[0]);
            var matchResults = new List<MatchReport>() {matchResult1, matchResult2};
            Server.OnGameEnd(matchResults);
        }

        MatchReport CreateMatchResultEndGamePlayerDidNothing(PlayerState defeatPlayer, PlayerState other)
        {
            var result = new MatchReport();
            result.GameResult = GAME_RESULT.DEFEAT;
            result.UserId = defeatPlayer.userId;

            var otherMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == other.userId);
            var defeatMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == defeatPlayer.userId);
            int defeatTrophy = ((otherMatchPlayer.Trophy - defeatMatchPlayer.Trophy) / 12 + 30);
            defeatTrophy = Math.Min(otherMatchPlayer.Trophy, defeatTrophy);

            // ?????? ?????????
            result.NormalRewards.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.thropy,
                Value = defeatTrophy * -1,
            });

            return result;
        }

        public override void OnGiveUp(PlayerState playerState)
        {
            var winner = PlayerStates.First(p => p.ownerTag != playerState.ownerTag);
            int getDefenderTowerHp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
            var winnerTower = Server.Towers.Find(t => t.ownerTag == winner.ownerTag);
            var loser = playerState;
            EndGame(winner, loser, true, winnerTower.currentHealth > getDefenderTowerHp * 100);
        }
    }
}