using System.Collections.Generic;
using MirageTest.Aws;
using RandomWarsProtocol;
using Service.Core;

namespace MirageTest.Scripts
{
    public class MatchReport
    {
        public string UserId { get; set; }
        public GAME_RESULT GameResult { get; set; }
        public short WinStreak { get; set; }
        public List<ItemBaseInfo> NormalRewards { get; set; }
        public List<ItemBaseInfo> StreakRewards { get; set; }
        public List<ItemBaseInfo> PerfectRewards { get; set; }
        public ItemBaseInfo LoseReward { get; set; }
        public List<QuestCompleteParam> QuestCompleteParam { get; set; }
        public bool IsPerfect { get; set; }

        public bool WinLose
        {
            get
            {
                return GameResult == GAME_RESULT.VICTORY || GameResult == GAME_RESULT.VICTORY_BY_DEFAULT;
            }
        }


        public MatchReport()
        {
            GameResult = GAME_RESULT.NONE;
            UserId = string.Empty;
            NormalRewards = new List<ItemBaseInfo>();
            StreakRewards = new List<ItemBaseInfo>();
            PerfectRewards = new List<ItemBaseInfo>();
            LoseReward = new ItemBaseInfo();
            QuestCompleteParam = new List<QuestCompleteParam>();
        }
    }
}