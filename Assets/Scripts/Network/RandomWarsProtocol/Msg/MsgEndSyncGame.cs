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
        public int RemainWaveTime;
        public byte SpawnCount;
    }


    [Serializable]
    public class MsgEndSyncGameNotify
    {
        public ushort PlayerUId;
        public int RemainWaveTime;
        public byte SpawnCount;
    }
}
