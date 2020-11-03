using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgMergeDiceReq
    {
        public short ResetFieldNum;
        public short LeveupFieldNum;
    }


    [Serializable]
    public class MsgMergeDiceAck
    {
        public int ErrorCode;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }


    [Serializable]
    public class MsgMergeDiceNotify
    {
        public ushort PlayerUId;
        public short ResetFieldNum;
        public short LeveupFieldNum;
        public int LevelupDiceId;
        public short Level;
    }
}
