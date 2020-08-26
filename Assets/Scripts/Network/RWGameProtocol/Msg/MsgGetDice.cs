using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Msg
{
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
        public int DeckNum;
        public int SlotNum;
        public int CurrentSp;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgGetDiceNotify : Serializer<MsgGetDiceNotify>
    {
        public int DeckNum;
        public int SlotNum;
    }
}
