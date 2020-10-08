using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgLevelUpDiceReq
    {
        public short ResetFieldNum;
        public short LeveupFieldNum;
    }


    [Serializable]
    public class MsgLevelUpDiceAck
    {
        public short ErrorCode;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }


    [Serializable]
    public class MsgLevelUpDiceNotify
    {
        public int PlayerUId;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }
}
