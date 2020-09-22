using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgInGameUpDiceReq : Serializer<MsgInGameUpDiceReq>
    {
        // 업그레이드 대상 주사위 아이디
        public int DiceId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgInGameUpDiceAck : Serializer<MsgInGameUpDiceAck>
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgInGameUpDiceNotify : Serializer<MsgInGameUpDiceNotify>
    {
        // 플레이어 UID
        public int PlayerUId;

        // 업그레이드 대상 주사위 아이디
        public int DiceId;

        // 인게임 업그레이드 수치
        public short InGameUp;
    }
}
