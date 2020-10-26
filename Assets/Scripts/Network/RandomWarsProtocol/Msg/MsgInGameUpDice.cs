using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgInGameUpDiceReq
    {
        // 업그레이드 대상 주사위 아이디
        public int DiceId;
    }


    [Serializable]
    public class MsgInGameUpDiceAck
    {
        public short ErrorCode;

        // 업그레이드 대상 주사위 아이디
        public int DiceId;

        // 인게임 업그레이드 수치
        public short InGameUp;

        // 현재 Sp
        public int CurrentSp;
    }


    [Serializable]
    public class MsgInGameUpDiceNotify
    {
        // 플레이어 UID
        public ushort PlayerUId;

        // 업그레이드 대상 주사위 아이디
        public int DiceId;

        // 인게임 업그레이드 수치
        public short InGameUp;
    }
}
