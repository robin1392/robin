using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgLeaveGameReq
    {
    }


    [Serializable]
    public class MsgLeaveGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgLeaveGameNotify
    {
        public int PlayerUId;
    }


}
