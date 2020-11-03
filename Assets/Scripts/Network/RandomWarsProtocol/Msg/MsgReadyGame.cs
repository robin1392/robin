using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgReadyGameReq
    {
    }


    [Serializable]
    public class MsgReadyGameAck
    {
        public int ErrorCode;
    }   
}
