using System.Collections.Generic;
using MirageTest.Aws;
using RandomWarsProtocol;
using Service.Core;

namespace MirageTest.Scripts
{
    public class MatchReport
    {
        public string UserId;
        public GAME_RESULT GameResult;
        public short WinStreak;
        public List<ItemBaseInfo> NormalRewards;
        public List<ItemBaseInfo> StreakRewards;
        public List<ItemBaseInfo> PerfectRewards;
        public ItemBaseInfo LoseReward;
        public List<QuestCompleteParam> QuestCompleteParam;
        public bool IsPerfect;

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