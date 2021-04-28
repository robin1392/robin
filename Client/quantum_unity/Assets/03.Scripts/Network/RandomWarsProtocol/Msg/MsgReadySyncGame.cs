using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgReadySyncGameReq
    {
    }


    [Serializable]
    public class MsgReadySyncGameAck
    {
        public int ErrorCode;
    }


    [Serializable]
    public class MsgReadySyncGameNotify
    {
        public ushort PlayerUId;
    }
}
