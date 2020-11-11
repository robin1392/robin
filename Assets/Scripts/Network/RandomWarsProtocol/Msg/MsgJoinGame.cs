using System;
using System.IO;

namespace RandomWarsProtocol.Msg
{

    [Serializable]
    public class MsgJoinGameReq
    {
        public string PlayerSessionId;

        // 사용 덱 인덱스
        public sbyte DeckIndex;
    }


    [Serializable]
    public class MsgJoinGameAck
    {
        // 에러 코드
        public int ErrorCode;

        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinGameNotify
    {
        // 다른 플레이어 정보
        public MsgPlayerInfo OtherPlayerInfo;
    }

    [Serializable]
    public class MsgJoinCoopGameReq
    {
        public string PlayerSessionId;

        // 사용 덱 인덱스
        public sbyte DeckIndex;
    }


    [Serializable]
    public class MsgJoinCoopGameAck
    {
        // 에러 코드
        public int ErrorCode;

        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinCoopGameNotify
    {
        // 아군(협동) 플레이어 정보
        public MsgPlayerInfo CoopPlayerInfo;

        // 상대방 플레이어 정보
        public MsgPlayerInfo[] OtherPlayerInfo;
    }
}