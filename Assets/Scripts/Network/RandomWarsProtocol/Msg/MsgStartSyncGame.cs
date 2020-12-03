using System;
using System.IO;

namespace RandomWarsProtocol.Msg
{


    [Serializable]
    public class MsgStartSyncGameReq
    {
    }


    [Serializable]
    public class MsgStartSyncGameAck
    {
        public int ErrorCode;
        public int Wave;

        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
        public MsgGameDice[] GameDiceData;
        public MsgInGameUp[] InGameUp;
        public MsgMinionStatusRelay LastStatusRelay;

        // 상대방 정보
        public MsgPlayerInfo OtherPlayerInfo;
        public MsgGameDice[] OtherGameDiceData;
        public MsgInGameUp[] OtherInGameUp;
        public MsgMinionStatusRelay OtherLastStatusRelay;
    }


    [Serializable]
    public class MsgStartSyncGameNotify
    {
    }
}
