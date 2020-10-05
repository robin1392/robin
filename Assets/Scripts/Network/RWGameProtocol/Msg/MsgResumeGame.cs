using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgResumeGameReq : Serializer<MsgResumeGameReq>
    {
        public string PlayerSessionId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgResumeGameAck : Serializer<MsgResumeGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgResumeGameNotify : Serializer<MsgResumeGameNotify>
    {
        public int PlayerUId;
    }
}
