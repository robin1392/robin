using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceReq : Serializer<MsgLevelUpDiceReq>
    {
        public int ResetFieldNum;
        public int LeveupFieldNum;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceAck : Serializer<MsgLevelUpDiceAck>
    {
        public short ErrorCode;
        public int ResetFieldNum;
        public int LeveupFieldNum;
        public int LevelupDiceId;
        public int Level;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceNotify : Serializer<MsgLevelUpDiceNotify>
    {
        public int ResetFieldNum;
        public int LeveupFieldNum;
        public int LevelupDiceId;
        public int Level;
    }
}
