using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using Mirage.Logging;
using MirageTest.Aws;
using MirageTest.Scripts.Entities;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Service.Core;
using UnityEngine;
using Debug = ED.Debug;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts.GameMode
{
    public class BattleMode : GameModeBase
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(BattleMode));
        private readonly int endWave;

        public BattleMode(PrefabHolder prefabHolder, ServerObjectManager serverObjectManager) : base(prefabHolder,
            serverObjectManager)
        {
            endWave = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.EndWave].value;
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

            //액터보다 플레이어 스테이트가 먼저 생성되야해서 한 틱 양보
            await UniTask.Yield();

            var player1TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: true);
            SpawnTower(PlayerState1, player1TowerPosition);

            var player2TowerPosition = FieldManager.Get().GetPlayerPos(isBottomPlayer: false);
            SpawnTower(PlayerState2, player2TowerPosition);
        }

        void SpawnTower(PlayerState playerState, Vector3 position)
        {
            var tower = UnityEngine.Object.Instantiate(_prefabHolder.ActorProxy, position, Quaternion.identity);
            tower.team = playerState.team;
            tower.ownerTag = playerState.ownerTag;
            tower.actorType = ActorType.Tower;

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
                        var ordered = server.Towers.OrderByDescending(t => t.currentHealth);
                        var winnerTower = ordered.ElementAt(0);
                        var loserTower = ordered.ElementAt(1);
                        var winner = PlayerStates.First(p => p.ownerTag == winnerTower.ownerTag);
                        var loser = PlayerStates.First(p => p.ownerTag == loserTower.ownerTag);
                        EndGame(winner, loser, false);
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

        private async UniTask Spawn(List<ActorProxy> actorProxies)
        {
            if (IsGameEnd)
            {
                return;
            }

            foreach (var actorProxy in actorProxies)
            {
                ServerObjectManager.Spawn(actorProxy.NetIdentity);

                if (IsGameEnd)
                {
                    return;
                }
            }
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
            EndGame(winner, loser, winnerTower.currentHealth > getDefenderTowerHp * 100);
        }

        private void EndGame(PlayerState winner, PlayerState loser, bool perfect)
        {
            IsGameEnd = true;
            var victoryReport = new UserMatchResult();
            victoryReport.MatchResult = (int) GAME_RESULT.VICTORY;
            ;
            victoryReport.UserId = winner.userId;

            // 연승 수치 적용

            var winnerMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == winner.userId);
            var defeatMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == loser.userId);

            // 트로피 추가
            int victoryTrophy = (defeatMatchPlayer.Trophy - winnerMatchPlayer.Trophy) / 12 + 30;
            victoryReport.listReward = new List<ItemBaseInfo>();
            victoryReport.listReward.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.thropy,
                Value = victoryTrophy,
            });

            victoryReport.listReward.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.rankthropy,
                Value = victoryTrophy,
            });

            victoryReport.listReward.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.seasonthropy,
                Value = 1,
            });

            // 승리 골드 보상
            int addGold = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.NormalWinReward_Gold].value;
            if (addGold != 0)
            {
                victoryReport.listReward.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.gold,
                    Value = addGold,
                });
            }

            int addKey = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.NormalWinReward_Key].value;
            if (addKey != 0)
            {
                victoryReport.listReward.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.key,
                    Value = addKey,
                });
            }

            //연승 보상
            // 완벽한 승리 보상

            // 패자 레포트 작성
            var defeatReport = new UserMatchResult();
            defeatReport.MatchResult = (int) GAME_RESULT.DEFEAT;
            ;
            defeatReport.UserId = loser.userId;

            // 연승 수치 적용


            // 트로피 차감
            int defeatTrophy = ((winnerMatchPlayer.Trophy - defeatMatchPlayer.Trophy) / 12 + 30);
            defeatTrophy = Math.Min(defeatMatchPlayer.Trophy, defeatTrophy);

            defeatReport.listReward = new List<ItemBaseInfo>();
            if (defeatTrophy != 0)
            {
                // 패배 트로피
                defeatReport.listReward.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.thropy,
                    Value = defeatTrophy * -1,
                });

                // 패배 랭킹 포인트
                defeatReport.listReward.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.rankthropy,
                    Value = defeatTrophy * -1,
                });
            }


            // // 패배 보상 지급
            // // 확률 체크
            // TDataVsmode tDataLoseUserCondition01;
            // TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition02, out tDataLoseUserCondition01);
            // var rate = Random.Range(0, 100);
            // if (rate < tDataLoseUserCondition01.value)
            // {
            //     // 연패수 체크
            //     TDataVsmode tDataLoseUserCondition02;
            //     TableManager.Get().Vsmode.GetData((int)EVsmodeKey.LoseUserCondition02, out tDataLoseUserCondition02);
            //     if (defeatPlayer.User.WinStreak < tDataLoseUserCondition02.value * -1)
            //     {
            //         // 상대방 트로피 차이
            //         TDataVsmode tDataLoseUserCondition03;
            //         TableManager.Instance.Vsmode.GetData((int)EVsmodeKey.LoseUserCondition03, out tDataLoseUserCondition03);
            //         if( Math.Abs(defeatPlayer.User.Trophy  - victoryPlayer.User.Trophy) > tDataLoseUserCondition03.value)
            //         {
            //             TDataVsmode tDataLoseUserRewardId;
            //             TableManager.Instance.Vsmode.GetData((int)EVsmodeKey.LoseUserRewardId, out tDataLoseUserRewardId);
            //             TDataVsmode tDataLoseUserRewardValue;
            //             TableManager.Instance.Vsmode.GetData((int)EVsmodeKey.LoseUserRewardValue, out tDataLoseUserRewardValue);
            //
            //             defeatReport.LoseReward.ItemId = tDataLoseUserRewardId.value;
            //             defeatReport.LoseReward.Value = tDataLoseUserRewardValue.value;
            //         }
            //     }
            // }

            // victoryReport.QuestCompleteParam.Add(new QuestCompleteParam
            // {
            //     QuestCompleteType = EQuestInfoKey.playAllMatch,
            //     Value = 1
            // });
            //
            // victoryReport.QuestCompleteParam.Add(new QuestCompleteParam
            // {
            //     QuestCompleteType = EQuestInfoKey.winAllMatch,
            //     Value = 1
            // });
            //
            // victoryReport.QuestCompleteParam.Add(new QuestCompleteParam
            // {
            //     QuestCompleteType = EQuestInfoKey.playDeathMatch,
            //     Value = 1
            // });
            //
            // if (totalGetKey > 0)
            // {
            //     victoryReport.QuestCompleteParam.Add(new QuestCompleteParam
            //     {
            //         QuestCompleteType = EQuestInfoKey.getKey,
            //         Value = totalGetKey
            //     });
            // }
            //
            // defeatReport.QuestCompleteParam.Add(new QuestCompleteParam
            // {
            //     QuestCompleteType = EQuestInfoKey.playAllMatch,
            //     Value = 1
            // });
            //
            // defeatReport.QuestCompleteParam.Add(new QuestCompleteParam
            // {
            //     QuestCompleteType = EQuestInfoKey.playDeathMatch,
            //     Value = 1
            // });

            Server.OnGameEnd(new List<UserMatchResult>() {victoryReport, defeatReport});
        }

        private void EndGamePlayerDidNothing(PlayerState[] losers)
        {
            IsGameEnd = true;
            var matchResult1 = CreateMatchResultEndGamePlayerDidNothing(losers[0], losers[1]);
            var matchResult2 = CreateMatchResultEndGamePlayerDidNothing(losers[1], losers[0]);
            var matchResults = new List<UserMatchResult>() {matchResult1, matchResult2};
            Server.OnGameEnd(matchResults);
        }

        UserMatchResult CreateMatchResultEndGamePlayerDidNothing(PlayerState defeatPlayer, PlayerState other)
        {
            var result = new UserMatchResult();
            result.MatchResult = (int) GAME_RESULT.DEFEAT;
            result.UserId = defeatPlayer.userId;

            var otherMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == other.userId);
            var defeatMatchPlayer = Server.MatchData.PlayerInfos.First(p => p.UserId == defeatPlayer.userId);
            int defeatTrophy = ((otherMatchPlayer.Trophy - defeatMatchPlayer.Trophy) / 12 + 30);
            defeatTrophy = Math.Min(otherMatchPlayer.Trophy, defeatTrophy);

            // 패배 트로피
            result.listReward.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.thropy,
                Value = defeatTrophy * -1,
            });

            return result;
        }
    }
}