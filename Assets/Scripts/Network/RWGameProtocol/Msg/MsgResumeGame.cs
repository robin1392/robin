using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgResumeGameReq
    {
    }


    [Serializable]
    public class MsgResumeGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgResumeGameNotify
    {
        public int PlayerUId;
    }
}
