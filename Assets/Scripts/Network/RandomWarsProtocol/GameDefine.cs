﻿using System;
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

    public enum REWARD_TYPE : byte
    {
        NONE = 0,
        TROPHY,
        GOLD,
        DIAMOND,
        KEY,
        BOX,
        DICE_NORMAL,
        DICE_MAGIC,
        DICE_EPIC,
        DICE_LEGEND,
    }


    public enum BOX_TYPE : byte
    {
        NONE,
        NORMAL_BOX,
        COOP_BOX,
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
