using System;
using Service.Core;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgDeactiveWaitingObjectNotify
    {
        public ushort PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    public class MsgAddSpNotify
    {
        public ushort PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    public class MsgSpawnNotify
    {
        public int Wave;
        public MsgSpawnInfo[] SpawnInfo;
    }


    [Serializable]
    public class MsgCoopSpawnNotify
    {
        public int Wave;
        public int PlayerUId;
        public MsgSpawnInfo[] SpawnInfo;
    }


    [Serializable]
    public class MsgMonsterSpawnNotify
    {
        public int PlayerUId;
        public MsgMonster SpawnMonster;
    }


    [Serializable]
    public class MsgEndGameNotify
    {
        public int ErrorCode;
        public GAME_RESULT GameResult;
        public short WinningStreak;
        public ItemBaseInfo[] NormalReward;
        public ItemBaseInfo[] StreakReward;
        public ItemBaseInfo[] PerfectReward;
        public AdRewardInfo LoseReward;
        public QuestData[] QuestData;
    }


    [Serializable]
    public class MsgEndCoopGameNotify
    {
        public int ErrorCode;
        public GAME_RESULT GameResult;
        public ItemBaseInfo[] NormalReward;
        public QuestData[] QuestData;
    }


    [Serializable]
    public class MsgDisconnectGameNotify 
    {
        public ushort PlayerUId;
    }


    [Serializable]
    public class MsgPauseGameNotify
    {
        public ushort PlayerUId;
    }


    [Serializable]
    public class MsgResumeGameNotify
    {
        public ushort PlayerUId;
    }
}
