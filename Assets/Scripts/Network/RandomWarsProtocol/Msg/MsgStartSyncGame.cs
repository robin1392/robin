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
        public int ErrorCode;
    }


    [Serializable]
    public class MsgStartSyncGameNotify
    {
        // 플레이어 정보.
        public int PlayerSpawnCount;
        public MsgPlayerInfo PlayerInfo;
        public MsgGameDice[] GameDiceData;
        public MsgInGameUp[] InGameUp;
        public MsgMinionStatusRelay LastStatusRelay;

        // 상대방 정보
        public int OtherPlayerSpawnCount;
        public MsgPlayerInfo OtherPlayerInfo;
        public MsgGameDice[] OtherGameDiceData;
        public MsgInGameUp[] OtherInGameUp;
        public MsgMinionStatusRelay OtherLastStatusRelay;
    }
}
