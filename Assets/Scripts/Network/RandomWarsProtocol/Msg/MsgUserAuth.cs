using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsProtocol.Msg
{
    [Serializable]
    public class MsgUserAuthReq
    {
        public string UserId;
    }

    [Serializable]
    public class MsgUserAuthAck
    {
        public short ErrorCode;
        public MsgUserInfo UserInfo;
        public MsgUserDeck[] UserDeck;
        public MsgUserDice[] UserDice;
    }


    [Serializable]
    public class MsgUpdateDeckReq
    {
        public string UserId;
        public sbyte DeckIndex;
        public int[] DiceIds;
    }


    [Serializable]
    public class MsgUpdateDeckAck
    {
        public sbyte DeckIndex;
        public int[] DiceIds;
    }


    [Serializable]
    public class MsgStartMatchReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgStartMatchAck
    {
        public string TicketId;
    }


    [Serializable]
    public class MsgStatusMatchReq
    {
        public string TicketId;
    }


    [Serializable]
    public class MsgStatusMatchAck
    {
        public string ServerAddr;
        public int Port;
        public string PlayerSessionId;
    }


    [Serializable]
    public class MsgStopMatchReq
    {
        public string TicketId;
    }

    [Serializable]
    public class MsgStopMatchAck
    {
    }
}
