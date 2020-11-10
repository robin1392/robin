﻿using System;

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
    }


    [Serializable]
    public class MsgCoopSpawnNotify
    {
        public int Wave;
        public int PlayerUId;
        public MsgBossMonster SpawnBossMonster;
    }


    [Serializable]
    public class MsgEndGameNotify
    {
        public int ErrorCode;
        public GAME_RESULT GameResult;
        public byte WinningStreak;
        public MsgReward[] NormalReward;
        public MsgReward[] StreakReward;
        public MsgReward[] PerfectReward;
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
