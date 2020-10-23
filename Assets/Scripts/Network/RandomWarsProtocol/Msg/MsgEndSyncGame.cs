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
        public short ErrorCode;
    }


    [Serializable]
    public class MsgEndSyncGameNotify
    {
        public ushort PlayerUId;
    }
}
