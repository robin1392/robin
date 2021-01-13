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
        BOX = 6,            // 박스
        DICE = 7,            //  주사위
        GUADIAN = 8,     // 수호자
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
        GOING,          // 진행중
        END,                // 종료
    }


    public enum BOX_OPEN_TYPE : int
    {
        NONE = -1,
        NEED_KEY = 1,           // 열쇠 필요
        DIRECTLY = 2,            // 즉시 열림
    }


    public enum QUEST_GROUP : int
    {
        NONE = -1,
        MATCH,
        SHOP,
        ITEM,
        MONSTER,

        VIP = 99,
    }


    public enum QUEST_STATUS : int
    {
        NONE = -1,
        LOCK,           // 잠김
        OPEN,          // 열림(진행중)
        COMPLETE,    // 완료(보상 획득전)
        CLOSE,         // 닫힘(보상 획득 완료) 
    }


    public enum QUEST_COMPLETE_TYPE : int
    {
        NONE = -1,
        PLAY_DEATH_MATCH,
        PLAY_COOP_MATCH,
        PLAY_ALL_MATCH,
        WIN_ALL_MATCH,
        OPEN_BOX,
        BUY_PRODUCTION,
        VIEW_AD,
        USE_GOLD,
        USE_DIAMOND,
        UPGRADE_DICE,
        SPAWN_GUARDIAN,
        KILL_ALL_BOSS,
        KILL_BOSS_1,
        KILL_BOSS_2,
        KILL_BOSS_3,
        GET_KEY,
        GET_BOX,
    }
}
