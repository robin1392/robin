using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgEndSyncGameReq
    {
    }


    [Serializable]
    public class MsgEndSyncGameAck
    {
        public int ErrorCode;
    }


    [Serializable]
    public class MsgEndSyncGameNotify
    {
        public ushort PlayerUId;
    }
}
