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
        public GameErrorCode ErrorCode;
        public MsgUserInfo UserInfo;
        public MsgUserDeck[] UserDeck;
        public MsgUserDice[] UserDice;
        public MsgUserBox[] UserBox;
    }


    [Serializable]
    public class MsgEditUserNameReq
    {
        public string UserId;
        public string UserName;
    }


    [Serializable]
    public class MsgEditUserNameAck
    {
        public GameErrorCode ErrorCode;
        public string UserId;
        public string UserName;
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
        public GameErrorCode ErrorCode;
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
        public GameErrorCode ErrorCode;
        public string UserId;
        public string TicketId;
    }


    [Serializable]
    public class MsgStatusMatchReq
    {
        public string UserId;
        public string TicketId;
    }


    [Serializable]
    public class MsgStatusMatchAck
    {
        public GameErrorCode ErrorCode;
        public string UserId;
        public string ServerAddr;
        public int Port;
        public string PlayerSessionId;
    }


    [Serializable]
    public class MsgStopMatchReq
    {
        public string UserId;
        public string TicketId;
    }

    [Serializable]
    public class MsgStopMatchAck
    {
        public GameErrorCode ErrorCode;
        public string UserId;
    }


    [Serializable]
    public class MsgOpenBoxReq
    {
        public string UserId;
        public int BoxId;
    }


    [Serializable]
    public class MsgOpenBoxAck
    {
        public GameErrorCode ErrorCode;
        public MsgReward[] BoxReward;
    }


    [Serializable]
    public class MsgLevelUpDiceReq
    {
        public string UserId;
        public int DiceId;
    }


    [Serializable]
    public class MsgLevelUpDiceAck
    {
        public GameErrorCode ErrorCode;
    }


    [Serializable]
    public class MsgGetRankReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgGetRankAck
    {
        public GameErrorCode ErrorCode;
    }
}
