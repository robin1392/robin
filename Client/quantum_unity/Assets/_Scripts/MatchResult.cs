using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Aws;
using MirageTest.Scripts;
using Quantum;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Core;
using Actor = Quantum.Actor;
using Random = UnityEngine.Random;

public static class MatchResult
{
    public static unsafe List<MatchReport> GetResult(Frame f, _Scripts.MatchData matchData)
    {
        PlayerRef winnerRef = PlayerRef.None;
        var towers = f.Filter<Tower>();
        while (towers.Next(out var e, out var tower))
        {
            var actor = f.Get<Actor>(e);
            winnerRef = actor.Owner;
        }

        RWPlayer winner = new RWPlayer();
        RWPlayer loser = new RWPlayer();
        for (var i = 0; i < f.Global->Players.Length; ++i)
        {
            var player = f.Global->Players[i];
            if (player.PlayerRef == winnerRef)
            {
                winner = player;
            }
            else
            {
                loser = player;
            }
        }
        
        return EndGame(winner, loser, false, false, matchData);
    }

    private static List<MatchReport> EndGame(RWPlayer winner, RWPlayer loser, bool winByDefault, bool perfect, _Scripts.MatchData matchData)
    {
        var victoryReport = new MatchReport();
        victoryReport.GameResult = winByDefault ? GAME_RESULT.VICTORY_BY_DEFAULT : GAME_RESULT.VICTORY;
        victoryReport.Nick = winner.NickName;
        victoryReport.IsPerfect = perfect;

        var victoryMatchPlayer = matchData.PlayerInfos.First(p => p.NickName == winner.NickName);
        var defeatMatchPlayer = matchData.PlayerInfos.First(p => p.NickName == loser.NickName);

        // 연승 수치 적용
        victoryReport.WinStreak =
            (short) ((victoryMatchPlayer.WinStreak > 0) ? Math.Min(victoryMatchPlayer.WinStreak + 1, 15) : 1);

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
                ItemId = (int) EItemListKey.gold,
                Value = addGold,
            });
        }

        addKey = (victoryMatchPlayer.WinStreak - 1) / 3;
        if (addKey != 0)
        {
            victoryReport.StreakRewards.Add(new ItemBaseInfo
            {
                ItemId = (int) EItemListKey.key,
                Value = addKey,
            });
        }

        // 완벽한 승리 보상
        if (perfect)
        {
            addGold = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.PerfectReward_Gold].value;
            if (addGold != 0)
            {
                victoryReport.PerfectRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.gold,
                    Value = addGold,
                });
            }

            addKey = TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.PerfectWinReward_Key].value;
            if (addKey != 0)
            {
                victoryReport.PerfectRewards.Add(new ItemBaseInfo
                {
                    ItemId = (int) EItemListKey.key,
                    Value = addKey,
                });
            }
        }


        // 패자 레포트 작성
        var defeatReport = new MatchReport();
        defeatReport.GameResult = GAME_RESULT.DEFEAT;
        defeatReport.Nick = loser.NickName;
        defeatReport.IsPerfect = perfect;

        // 연승 수치 적용
        defeatReport.WinStreak =
            (short) ((defeatMatchPlayer.WinStreak < 0) ? Math.Max(defeatMatchPlayer.WinStreak - 1, -15) : -1);

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
        TableManager.Get().Vsmode.GetData((int) EVsmodeKey.LoseUserCondition01, out var tDataLoseUserCondition01);
        var rate = Random.Range(0, 100);
        if (rate < tDataLoseUserCondition01.value)
        {
            // 연패수 체크
            TableManager.Get().Vsmode.GetData((int) EVsmodeKey.LoseUserCondition02, out var tDataLoseUserCondition02);
            if (defeatMatchPlayer.WinStreak < tDataLoseUserCondition02.value * -1)
            {
                // 상대방 트로피 차이
                TableManager.Get().Vsmode
                    .GetData((int) EVsmodeKey.LoseUserCondition03, out var tDataLoseUserCondition03);
                if (Math.Abs(defeatMatchPlayer.Trophy - victoryMatchPlayer.Trophy) > tDataLoseUserCondition03.value)
                {
                    TableManager.Get().Vsmode.GetData((int) EVsmodeKey.LoseUserRewardId, out var tDataLoseUserRewardId);
                    TableManager.Get().Vsmode.GetData((int) EVsmodeKey.LoseUserRewardValue,
                        out var tDataLoseUserRewardValue);

                    defeatReport.LoseReward.RewardId = Guid.NewGuid().ToString();
                    ;
                    defeatReport.LoseReward.ItemId = tDataLoseUserRewardId.value;
                    defeatReport.LoseReward.Value = tDataLoseUserRewardValue.value;
                }
            }
        }

        return new List<MatchReport>(){victoryReport, defeatReport};
        
    }
    
    static List<UserMatchResult> ToMatchResults(List<MatchReport> matchReport)
    {
        return matchReport.Select(report =>
        {
            var isVictory = (report.GameResult == GAME_RESULT.VICTORY ||
                             report.GameResult == GAME_RESULT.VICTORY_BY_DEFAULT);

            return new UserMatchResult()
            {
                MatchResult = isVictory ? 1 : 2,
                UserId = report.Nick,
                listReward = report.NormalRewards.Concat(report.PerfectRewards).Concat(report.StreakRewards).ToList(),
                LoseReward = report.LoseReward,
            };
        }).ToList();
    }
}