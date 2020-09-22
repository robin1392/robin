using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceReq : Serializer<MsgLevelUpDiceReq>
    {
        public short ResetFieldNum;
        public short LeveupFieldNum;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceAck : Serializer<MsgLevelUpDiceAck>
    {
        public short ErrorCode;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLevelUpDiceNotify : Serializer<MsgLevelUpDiceNotify>
    {
        public int PlayerUId;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }
}
