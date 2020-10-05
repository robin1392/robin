using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgPauseGameReq : Serializer<MsgPauseGameReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgPauseGameAck : Serializer<MsgPauseGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgPauseGameNotify : Serializer<MsgPauseGameNotify>
    {
        public int PlayerUId;
    }
}
