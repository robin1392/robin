using System;
using Service.Core;

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
        public ItemBaseInfo[] giveUpReward;
    }


    [Serializable]
    public class MsgLeaveGameNotify
    {
        public ushort PlayerUId;
    }


}
