using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgLeaveGameReq
    {
    }


    [Serializable]
    public class MsgLeaveGameAck
    {
        public int ErrorCode;
    }


    [Serializable]
    public class MsgLeaveGameNotify
    {
        public ushort PlayerUId;
    }


}
