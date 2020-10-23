using System;
using System.IO;

namespace RandomWarsProtocol.Msg
{


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
