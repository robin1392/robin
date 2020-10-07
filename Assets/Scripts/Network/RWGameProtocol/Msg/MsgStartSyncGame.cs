using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgSyncMinionData
    {
        public int minionId;
        public int minionDataId;
        public int minionHp;
        public int minionMaxHp;
        public int minionPower;
        public int minionEffect;
        public int minionEffectUpgrade;
        public int minionEffectIngameUpgrade;
        public int minionDuration;
        public int minionCooltime;
        public MsgVector3 minionPos;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgGameDice
    {
        public int DiceId;
        public short SlotNum;
        public short Level;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgInGameUp
    {
        public int DiceId;
        public short Grade;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameReq : Serializer<MsgStartSyncGameReq>
    {
        public int PlayerId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public MsgSyncMinionData[] SyncMinionData = new MsgSyncMinionData[100];

        
        public int OtherPlayerId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public MsgSyncMinionData[] OtherSyncMinionData = new MsgSyncMinionData[100];

        public int PlayerSpawnCount;
        public int OtherPlayerSpawnCount;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameAck : Serializer<MsgStartSyncGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgStartSyncGameNotify : Serializer<MsgStartSyncGameNotify>
    {
        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public MsgGameDice[] GameDiceData = new MsgGameDice[15];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public MsgInGameUp[] InGameUp = new MsgInGameUp[5];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public MsgSyncMinionData[] SyncMinionData = new MsgSyncMinionData[100];


        // 상대방 정보
        public MsgPlayerInfo OtherPlayerInfo;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public MsgGameDice[] OtherGameDiceData = new MsgGameDice[15];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public MsgInGameUp[] OtherInGameUp = new MsgInGameUp[5];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public MsgSyncMinionData[] OtherSyncMinionData = new MsgSyncMinionData[100];

        public int PlayerSpawnCount;
        public int OtherPlayerSpawnCount;

    }
}
