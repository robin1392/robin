namespace Service.Core
{
    public enum EBuyType
    {
        None = 0,
        Gold,
        Diamond,
        RealMoney,
        Free,
        AdFree
    }

    public enum ITEM_GRADE : byte
    {
        NORMAL = 0,
        MAGIC = 1,
        EPIC = 2,
        LEGEND = 3,
    }


    public enum ITEM_TYPE
    {
        NONE = 0,
        GOLD = 1,           // 1: 골드
        DIAMOND = 2,        // 2: 다이아
        TROPHY = 3,           // 3: 트로피
        KEY = 4,            // 4: 열쇠
        PASS = 5,           // 5: 패스
        BOX = 6,            // 박스
        DICE = 7,            //  주사위
        GUADIAN = 8,     // 수호자
        RANDOM_DICE = 9,        // 랜덤 주사위
        EMOTION = 10,
    }

    public enum DICE_GRADE
    {
        NORMAL = 0,
        MAGIC = 1,
        EPIC = 2,
        LEGEND = 3,
    }


    public enum BOX_TYPE : byte
    {
        NONE,
        NORMAL_BOX,
        COOP_BOX,
    }    


    public enum QUEST_GROUP : int
    {
        NONE = -1,
        MATCH,
        SHOP,
        ITEM,
        MONSTER,

        VIP = 99,
        COMPLETE = 1000,
    }

    public enum QUEST_STATUS : int
    {
        NONE = -1,
        LOCK,           // 잠김
        OPEN,          // 열림(진행중)
        COMPLETE,    // 완료(보상 획득전)
        CLOSE,         // 닫힘(보상 획득 완료) 
    }

    public enum REWARD_TARGET_TYPE : int
    {
        NONE = -1,

        ALL = 1,                        // 모든 유저
        SEASON_PASS_BUY = 2     // 시즌  패스 구입 유저
    }


    public enum ESeasonType
    {
        None = 0,
        Season,         // 일반 시즌
        FreeSeason,
    }


    public enum SEASON_STATE : byte
    {
        NONE,
        GOING,          // 진행중
        END,                // 종료
        BREAK,              // 휴식
    }
}