using System;
using System.Collections.Generic;
using System.Text;
using Service.Core;

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
        public UserDeck[] UserDeck;
        public UserDice[] UserDice;
        public UserBox[] UserBox;
        public QuestInfo QuestInfo;
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
    public class MsgEndTutorialReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgEndTutorialAck
    {
        public GameErrorCode ErrorCode;
        public bool EndTutorial;
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
        public ItemBaseInfo[] BoxReward;
        public UserBox BoxInfo;
        public MsgUserGoods UserGoods;
        // 퀘스트 데이터
        public QuestData[] QuestData;
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
        public UserDice UserDice;
        public MsgUserGoods UserGoods;
        // 퀘스트 데이터
        public QuestData[] QuestData;
    }


    [Serializable]
    public class UserSeasonInfoReq
    {
        public string UserId;
    }


    [Serializable]
    public class UserSeasonInfoAck
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
        // 시즌 초기화 필요
        public bool NeedSeasonReset;
        // 랭킹 리셋 남은 시간(초단위)
        public int RankingResetRemainTime;
    }


    [Serializable]
    public class MsgSeasonResetReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgSeasonResetAck
    {
        public GameErrorCode ErrorCode;
        // 시즌 아이디
        public int SeasonId;
        // 시즌 상태(진행중, 정산중, 종료 등등)
        public byte SeasonState;
        // 시즌 남은 시간(초단위)
        public int SeasonRemainTime;
        // 리셋 랭킹 포인트
        public int ResetRankingPoint;
        // 리셋 시즌 포인트
        public int ResetSeasonPoint;
        // 리셋 시즌 패스 보상 단계
        public int ResetSeasonPassRewardStep;
        // 프리 시즌 여부
        public bool IsFreeSeason;
        // 최종 순위
        public int myRanking;
        // 시즌 보상
        public ItemBaseInfo[] arraySeasonReward;
    }


    [Serializable]
    public class MsgSeasonPassRewardStepReq
    {
        public string UserId;
        // 오픈 테이블 아이디
        public int OpenRewardId;
    }


    [Serializable]
    public class MsgSeasonPassRewardStepAck
    {
        public GameErrorCode ErrorCode;
        // 오픈 테이블 아이디
        public int OpenRewardId;
        // 사용 아이템 정보
        public ItemBaseInfo UseItemInfo;
        // 획득 보상 정보
        public ItemBaseInfo RewardInfo;
        // 퀘스트 데이터
        public QuestData[] QuestData;
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

    [Serializable]
    public class MsgSeasonPassInfoReq
    {
        public string UserId;
    }

    [Serializable]
    public class MsgSeasonPassInfoAck
    {
        public GameErrorCode ErrorCode;
        // 시즌 패스 정보
        public MsgSeasonPassInfo SeasonPassInfo;
    }


    [Serializable]
    public class MsgGetSeasonPassRewardReq
    {
        public string UserId;
        // 요청 보상 아이디
        public int RewardId;
        // 보상 대상 타입(REWARD_TARGET_TYPE 참조)
        public int RewardTargetType;
    }


    [Serializable]
    public class MsgGetSeasonPassRewardAck
    {
        public GameErrorCode ErrorCode;
        // 보상 획득 아이디 배열
        public int[] GetRewardIds;
        // 보상 정보
        public ItemBaseInfo[] RewardInfo;
        // 퀘스트 데이터
        public QuestData[] QuestData;
    }

    [Serializable]
    public class MsgClassRewardInfoReq
    {
        public string UserId;
    }


    [Serializable]
    public class MsgClassRewardInfoAck
    {
        public GameErrorCode ErrorCode;
        // 누적 트로피
        public int TotalTrophy;
        // 보상 획득 아이디 배열
        public int[] GetRewardIds;
    }


    [Serializable]
    public class MsgGetClassRewardReq
    {
        public string UserId;
        // 요청 보상 아이디
        public int RewardId;
        // 보상 대상 타입(REWARD_TARGET_TYPE 참조)
        public int RewardTargetType;
    }


    [Serializable]
    public class MsgGetClassRewardAck
    {
        public GameErrorCode ErrorCode;
        // 보상 획득 아이디 배열
        public int[] GetRewardIds;
        // 보상 정보
        public ItemBaseInfo[] RewardInfo;
        // 퀘스트 데이터
        public QuestData[] QuestData;
    }


    [Serializable]
    public class QuestInfoReq
    {
        public string UserId;
    }


    [Serializable]
    public class QuestInfoAck
    {
        public GameErrorCode ErrorCode;
        public QuestInfo QuestInfo;
    }


    [Serializable]
    public class MsgQuestRewardReq
    {
        public string UserId;
        public int QuestId;
    }


    [Serializable]
    public class MsgQuestRewardAck
    {
        public GameErrorCode ErrorCode;
        public ItemBaseInfo[] RewardInfo;
        // 퀘스트 데이터
        public QuestData[] QuestData;
    }

    [Serializable]
    public class QuestDayRewardReq
    {
        public string UserId;
        // 보상 아이디
        public int RewardId;
        // 보상 인덱스
        public byte Index;
    }


    [Serializable]
    public class QuestDayRewardAck
    {
        public GameErrorCode ErrorCode;
        // 획득한 보상 정보
        public ItemBaseInfo[] RewardInfo;
        // 일일 보상 정보
        public QuestDayReward DayRewardInfo;
        // 퀘스트 데이터
        public QuestData[] QuestData;
    }
}
