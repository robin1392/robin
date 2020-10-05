using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReconnectGameReq : Serializer<MsgReconnectGameReq>
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 60)]
        public string PlayerSessionId;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReconnectGameAck : Serializer<MsgReconnectGameAck>
    {
        public short ErrorCode;
        public int Wave;
        public MsgPlayerInfo PlayerInfo;
    }
}
