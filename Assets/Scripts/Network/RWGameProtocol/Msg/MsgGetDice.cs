using System;


namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgGetDiceReq
    {
    }


    [Serializable]
    public class MsgGetDiceAck
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
    public class MsgGetDiceNotify
    {
        // 플레이어 UId
        public ushort PlayerUId;

        // 주사위 아이디
        public int DiceId;

        // 슬롯 번호
        public short SlotNum;

        // 레벨
        public short Level;
    }
}
