using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceReq : Serializer<MsgGetDiceReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceAck : Serializer<MsgGetDiceAck>
    {
        public short ErrorCode;

        // 주사위 아이디
        public int DiceId;

        // 슬롯 번호
        public short SlotNum;

        // 레벨
        public short Level;

        // 현재 sp
        public int CurrentSp;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceNotify : Serializer<MsgGetDiceNotify>
    {
        // 플레이어 UId
        public int PlayerUId;

        // 주사위 아이디
        public int DiceId;

        // 슬롯 번호
        public short SlotNum;

        // 레벨
        public short Level;
    }
}
