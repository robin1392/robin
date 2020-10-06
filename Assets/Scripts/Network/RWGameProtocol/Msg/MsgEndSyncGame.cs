using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgEndSyncGameReq : Serializer<MsgEndSyncGameReq>
    {
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgEndSyncGameAck : Serializer<MsgEndSyncGameAck>
    {
        public short ErrorCode;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgEndSyncGameNotify : Serializer<MsgEndSyncGameNotify>
    {
    }
}
