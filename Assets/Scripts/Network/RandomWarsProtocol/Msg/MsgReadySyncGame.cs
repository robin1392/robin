﻿using System;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgReadySyncGameReq
    {
    }


    [Serializable]
    public class MsgReadySyncGameAck
    {
        public short ErrorCode;
    }


    [Serializable]
    public class MsgReadySyncGameNotify
    {
        public ushort PlayerUId;
    }
}