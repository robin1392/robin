using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgReconnectGameReq
    {
    }


    [Serializable]
    public class MsgReconnectGameAck
    {
        public int ErrorCode;
        public MsgPlayerBase PlayerBase;
        public MsgPlayerBase OtherPlayerBase;
    }


    [Serializable]
    public class MsgReconnectGameNotify
    {
        public ushort PlayerUId;
    }
}
