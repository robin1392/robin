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
        public int GameMode;
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
        public MsgUserBox BoxInfo;
        public MsgUserGoods UserGoods;
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
        public MsgUserDice UserDice;
        public MsgUserGoods UserGoods;
    }


    [Serializable]
    public class MsgSeasonInfoReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgSeasonInfoAck
    {
        public GameErrorCode ErrorCode;
        // 시즌 식별 번호
        public int SeasonIndex;
        // 시즌 상태(진행중, 정산중, 종료 등등)
        public byte SeasonState;
        // 시즌 남은 시간(초단위)
        public int SeasonRemainTime;
        // 현재 나의 순위
        public int myRanking;
        // 시즌 획득 트로피
        public int myTrophy;
        // Top 랭킹 순위
        public MsgRankInfo[] TopRankInfo;
    }



    [Serializable]
    public class MsgGetRankReq
    {
        public string UserId;
        public int PageNo;
    }


    [Serializable]
    public class MsgGetRankAck
    {
        public GameErrorCode ErrorCode;
        public int PageNo;
        public MsgRankInfo[] RankInfo;
    }
}
