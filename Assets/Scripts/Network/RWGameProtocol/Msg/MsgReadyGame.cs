using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgReadyGameReq
    {
    }


    [Serializable]
    public class MsgReadyGameAck
    {
        public short ErrorCode;
    }   
}
