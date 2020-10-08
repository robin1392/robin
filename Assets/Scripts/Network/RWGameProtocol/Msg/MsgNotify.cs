using System;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgDeactiveWaitingObjectNotify
    {
        public int PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    public class MsgAddSpNotify
    {
        public int PlayerUId;
        public int CurrentSp;
    }


    [Serializable]
    public class MsgSpawnNotify
    {
        public int Wave;
    }


    [Serializable]
    public class MsgEndGameNotify
    {
        public int WinPlayerUId;
    }


    [Serializable]
    public class MsgDisconnectGameNotify 
    {
        public int PlayerUId;
    }
}
