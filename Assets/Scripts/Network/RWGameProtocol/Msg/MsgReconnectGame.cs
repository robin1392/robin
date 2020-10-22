using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgReconnectGameReq
    {
    }


    [Serializable]
    public class MsgReconnectGameAck
    {
        public short ErrorCode;
        public MsgPlayerBase PlayerBase;
        public MsgPlayerBase OtherPlayerBase;
    }


    [Serializable]
    public class MsgReconnectGameNotify
    {
        public ushort PlayerUId;
    }
}
