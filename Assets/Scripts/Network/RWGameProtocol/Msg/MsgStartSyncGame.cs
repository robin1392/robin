using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
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
    public struct MsgGameDice
    {
        public int DiceId;
        public short SlotNum;
        public short Level;
    }


    [Serializable]
    public struct MsgInGameUp
    {
        public int DiceId;
        public short Grade;
    }


    [Serializable]
    public class MsgStartSyncGameReq
    {
        public int PlayerId;
        public int PlayerSpawnCount;
        public MsgSyncMinionData[] SyncMinionData;
        
        public int OtherPlayerId;
        public int OtherPlayerSpawnCount;
        public MsgSyncMinionData[] OtherSyncMinionData;
    }


    [Serializable]
    public class MsgStartSyncGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgStartSyncGameNotify
    {
        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
        public MsgGameDice[] GameDiceData;
        public MsgInGameUp[] InGameUp;
        public MsgSyncMinionData[] SyncMinionData;
        public int PlayerSpawnCount;

        // 상대방 정보
        public MsgPlayerInfo OtherPlayerInfo;
        public MsgGameDice[] OtherGameDiceData;
        public MsgInGameUp[] OtherInGameUp;
        public MsgSyncMinionData[] OtherSyncMinionData;
        public int OtherPlayerSpawnCount;
    }
}
