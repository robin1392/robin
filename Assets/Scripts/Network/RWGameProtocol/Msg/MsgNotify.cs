using System;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgDeactiveWaitingObjectNotify : Serializer<MsgDeactiveWaitingObjectNotify>
    {
        public int PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgAddSpNotify : Serializer<MsgAddSpNotify>
    {
        public int PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgSpawnNotify : Serializer<MsgSpawnNotify>
    {
        public int Wave;
    }


    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MsgEndGameNotify : Serializer<MsgEndGameNotify>
    {
        public int WinPlayerUId;
    }
}
