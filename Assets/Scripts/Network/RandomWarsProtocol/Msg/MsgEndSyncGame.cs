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
        public byte SpawnCount;
    }


    [Serializable]
    public class MsgEndSyncGameNotify
    {
        public ushort PlayerUId;
        public byte SpawnCount;
    }
}
