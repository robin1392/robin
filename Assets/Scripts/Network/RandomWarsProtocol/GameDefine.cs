using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsProtocol
{
    public enum GAME_RESULT : byte
    {
        NONE,
        // 승리
        VICTORY,
        // 부전승
        VICTORY_BY_DEFAULT,
        // 패배
        DEFEAT,
        // 무승부
        DRAW
    }


    public enum DICE_GRADE : byte
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
        DICE = 6,            //  주사위
        GUADIAN = 7,     // 수호자
        BOX = 8,            // 박스
    }


    public enum BOX_TYPE : byte
    {
        NONE,
        NORMAL_BOX,
        COOP_BOX,
    }


    public enum GLOBAL_DATA_KEY
    {
        NONE,
        START_COOLTIME,
        WAVE_TIME,
        DICE_START_LEVEL_NORMAL,
        DICE_START_LEVEL_MAGIC,
        DICE_START_LEVEL_EPIC,
        DICE_START_LEVEL_LEGEND,
    }


    public enum BOSS_TYPE : byte
    {
        NONE,
        GROUND,
        SKY,
        BOTH
    }

    public enum BOSS_TARGET_TYPE : byte
    {
        NONE,
        GROUND,
        SKY,
        BOTH
    }

    public enum BOSS_ATK_SPEED : byte
    {
        NONE,
        SLOW,
        NORMAL,
        FAST,
    }

    public enum BOSS_MOVE_SPEED : byte
    {
        NONE,
        SLOW,
        NORMAL,
        FAST,
    }

    public enum SEASON_STATE : byte
    {
        NONE,
        PRE,            // 프리시즌
        GOING,          // 진행중
        ACCOUNT,        // 정산중
        END,                // 종료
    }
}
