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
        public bool bottomGudianSpawned;
        public bool topGudianSpawned;

        public BattleMode(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder,
            serverObjectManager)
        {
            endWave = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.EndWave].value;
            towerHpForGudianSpawn = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
        }

        public override async UniTask OnBeforeGameStart()
        {
            var gameState = CreateGameState();
            Debug.Log($"gameState:{gameState != null} {gameState.state}");
            var playerStates = CreatePlayerStates();
            Debug.Log($"playerStates1:{playerStates[0] != null} playerStates2:{playerStates[1] != null}");
            gameState.masterOwnerTag = playerStates[0].ownerTag;
            GameState = gameState;
            PlayerStates = playerStates;

            PlayerState1.team = GameConstants.BottomCamp;
            PlayerState2.team = GameConstants.TopCamp;
            
            Debug.Log($"ServerObjectManager:{ServerObjectManager != null}");
            ServerObjectManager.Spawn(gameState.NetIdentity);
            foreach (var playerState in playerStates)
            {
                ServerObjectManager.Spawn(playerState.NetIdentity);
            }

            //액터보다 플레이어 스테이트가 먼저 생성되야해서 한 틱 양보
            await UniTask.Yield();

            Debug.Log($"fieldManager:{FieldManager.Get() != null}");
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
                            tower.Destroy();
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
                        loserTower.Destroy();
                    }
                }
                return;
            }

            var actorProxies = new List<ActorProxy>();
            foreach (var playerState in PlayerStates)
            {
                actorProxies.AddRange(CreateActorByPlayerFieldDice(playerState));
            }

            if (wave > 10)
            {
                foreach (var actorProxy in actorProxies)
                {
                    ApplyDeathMatchEffect(actorProxy, wave);
                }
            }

            Spawn(actorProxies).Forget();
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
                position += Vector3.forward;
            }
            else
            {
                position += Vector3.back;
            }
            Server.CreateActorWithGuardianId(playerState.guardianId, actorProxy.ownerTag, actorProxy.team, position);
        }

        void ApplyDeathMatchEffect(ActorProxy actorProxy, int wave)
        {
            actorProxy.power *= Mathf.Pow(2f, GameState.wave - 10);
            actorProxy.attackSpeed *= Mathf.Pow(0.9f, GameState.wave - 10);
            actorProxy.attackSpeed = Mathf.Max(actorProxy.attackSpeed * 0.5f, actorProxy.attackSpeed);
        }

        public override void OnTowerDestroyed(ActorProxy destroyedTower)
        {
            var server = ServerObjectManager.Server as RWNetworkServer;
            var loser = PlayerStates.First(p => p.ownerTag == destroyedTower.ownerTag);
            var winnerTower = server.Towers.First();
            var winner = PlayerStates.First(p => p.ownerTag == winnerTower.ownerTag);
            int getDefenderTowerHp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetDefenderTowerHp].value;
            EndGame(winner, loser, false, winnerTower.currentHealth > getDefenderTowerHp * 100);
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
            
            // 연승 수치 적용
            victoryReport.WinStreak = (short)((victoryMatchPlayer.WinStreak > 0) ? Math.Min(victoryMatchPlayer.WinStreak + 1, 15) : 1);
            
            // 트로피 추가
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
            
            // 승리 골드 보상
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

            //연승 보상
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
            
            // 완벽한 승리 보상
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
            
            
            // 패자 레포트 작성
            var defeatReport = new MatchReport();
            defeatReport.GameResult = GAME_RESULT.DEFEAT;
            defeatReport.UserId = loser.userId;
            defeatReport.IsPerfect = perfect;

            // 연승 수치 적용
            defeatReport.WinStreak = (short)((defeatMatchPlayer.WinStreak < 0) ? Math.Max(defeatMatchPlayer.WinStreak - 1, -15) : -1);

            // 트로피 차감
            int defeatTrophy = ((victoryMatchPlayer.Trophy - defeatMatchPlayer.Trophy) / 12 + 30);
            defeatTrophy = Math.Min(defeatMatchPlayer.Trophy, defeatTrophy);
            
            if (defeatTrophy != 0)
            {
                // 패배 트로피
                defeatReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.thropy,
                    Value = defeatTrophy * -1,
                });

                // 패배 랭킹 포인트
                defeatReport.NormalRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.rankthropy,
                    Value = defeatTrophy * -1,
                });
            }

            // 패배 보상 지급
            // 확률 체크
            TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition01, out var tDataLoseUserCondition01);
            var rate = Random.Range(0, 100);
            if (rate < tDataLoseUserCondition01.value)
            {
                // 연패수 체크
                TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition02, out var tDataLoseUserCondition02);
                if (defeatMatchPlayer.WinStreak < tDataLoseUserCondition02.value * -1)
                {
                    // 상대방 트로피 차이
                    TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition03, out var tDataLoseUserCondition03);
                    if( Math.Abs(defeatMatchPlayer.Trophy  - victoryMatchPlayer.Trophy) > tDataLoseUserCondition03.value)
                    {
                        TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserRewardId, out var tDataLoseUserRewardId);
                        TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserRewardValue, out var tDataLoseUserRewardValue);

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

            // 패배 트로피
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