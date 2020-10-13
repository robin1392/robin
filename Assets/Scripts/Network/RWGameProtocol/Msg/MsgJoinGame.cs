using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgPlayerBase
    {
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
    }


    [Serializable]
    public class MsgPlayerInfo
    {
        // 플레이어 유니크 아이디(게임 세션별 유니크 생성)
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceUpgradeArray;
    }


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
        public short ErrorCode;

        // 플레이어 정보.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinGameNotify
    {
        // 다른 플레이어 정보
        public MsgPlayerInfo OtherPlayerInfo;
    }
}