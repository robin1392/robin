using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgPauseGameReq
    {
    }


    [Serializable]
    public class MsgPauseGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgPauseGameNotify
    {
        public int PlayerUId;
    }
}
