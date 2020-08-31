using System;
using System.Runtime.InteropServices;


namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReadyGameReq : Serializer<MsgReadyGameReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgReadyGameAck : Serializer<MsgReadyGameAck>
    {
        public short ErrorCode;
    }   
}
