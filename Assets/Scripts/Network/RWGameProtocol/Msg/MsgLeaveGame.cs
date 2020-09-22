using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLeaveGameReq : Serializer<MsgLeaveGameReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLeaveGameAck : Serializer<MsgLeaveGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgLeaveGameNotify : Serializer<MsgLeaveGameNotify>
    {
        public int PlayerUId;
    }


}
