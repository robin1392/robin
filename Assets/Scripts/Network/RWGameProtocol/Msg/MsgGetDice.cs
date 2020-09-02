using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Msg
{
    /// <summary>
    /// 인게임 생성 주사위 정보
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MsgGameDice
    {
        public int Id;
        public short SlotNum;
        public short Level;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceReq : Serializer<MsgGetDiceReq>
    {
        public int UseSp;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceAck : Serializer<MsgGetDiceAck>
    {
        public short ErrorCode;
        public int CurrentSp;
        public MsgGameDice DiceInfo;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceNotify : Serializer<MsgGetDiceNotify>
    {
        public int DeckNum;
        public int SlotNum;
    }
}
