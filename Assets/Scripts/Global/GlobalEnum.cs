﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Global Enum 선언
/// </summary>
namespace ED
{
    public enum DICE_CAST_TYPE
    {
        MINION = 0,
        MAGIC = 1,
        INSTALLATION = 2,
        HERO = 3,
    }
    
    public enum DICE_MOVE_TYPE
    {
        GROUND = 0,
        FLYING = 1,
        ALL = 2,
    }
}

public partial class Global
{
    public enum PLAY_TYPE
    {
        BATTLE,
        COOP,
    }

    
    #region game state & action enum
    public enum E_GAMESTATE
    {
        STATE_START,
        STATE_LOADING,
        STATE_MAIN,
        STATE_INGAME,
        STATE_COOP,
    }

    public enum E_STATEACTION
    {
        ACTION_START,
        ACTION_LOADING,
        ACTION_MAIN,
        ACTION_INGAME,
        ACTION_COOP,
    }

    public enum E_STARTSTEP
    {
        START_NONE,
        START_CONNECT,
        START_VERSION,
        START_DATADOWN,
        START_USERDATA,
    }
    #endregion
    
    #region game object load type
    public enum E_LOADTYPE
    {
        LOAD_MINION,
        LOAD_MAGIC,
        LOAD_MAIN_MINION,
        LOAD_MAIN_MAGIC,
        LOAD_GUARDIAN,
        LOAD_COOP_BOSS,
        LOAD_MAX,
    }
    #endregion
    
    
    #region network match status

    public enum E_MATCHSTEP
    {
        MATCH_NONE,
        MATCH_START,
        MATCH_CONNECT,
        MATCH_CANCEL,
    }
    #endregion
    
    
    #region lang code
    public enum COUNTRYCODE
    {
        nonCode = -1,
        KO,
        EN,
        TW,
        RU,
        JP,
        BR,
        CN
    }

    public enum COUNTRY
    {
        non = -1,
        Korean,
        English,
        Taiwan,
        Russia,
        Japan,
        Brazil,
        Chinese
    }
    
    #endregion
    
    
    #region ui enum
    // main ui dice info
    public enum E_DICEINFOSLOT
    {
        Info_Type  = 0,
        Info_Hp = 1,
        Info_AtkPower = 2,
        Info_AtkSpeed = 3,
        Info_MoveSpeed = 4,
        Info_SearchRange = 5,
        Info_etc = 6,
        Info_Sp = 7,
    }
    
    
    
    #endregion
    
    
    #region sound

    public enum E_SOUND
    {
        // BGM
        BGM_LOBBY = 0,
        BGM_INGAME_BATTLE,
        BGM_INGAME_COOP,
        BGM_INGAME_WIN,
        BGM_INGAME_LOSE,
        
        // SFX
        SFX_MINION_DEATH = 100,
        SFX_MINION_GENERATE,
        SFX_MINION_HIT,
        SFX_MINION_BOW_SHOT,
        SFX_FIREBALL_FIRE,
        SFX_FIREBALL_EXPLOSION,

        // UI
        SFX_UI_BUTTON = 10000,
        SFX_WIN,
        SFX_LOSE,
        SFX_UI_PERFECT,
        SFX_UI_SCREEN_SWIPE,
        SFX_UI_DICE_SELECT,
        SFX_UI_DICE_INFO_EFX,
        SFX_UI_DICE_LVUP_EFX,
        SFX_UI_DICE_POINT_UP,
        SFX_UI_LVUP_RESULT,
        SFX_UI_BOX_NORMAL_APPEAR,
        SFX_UI_BOX_COOP_APPEAR,
        SFX_UI_BOX_COMMON_FALLDOWN,
        SFX_UI_BOX_COMMON_OPEN,
        SFX_UI_BOX_COMMON_OPEN_REPEAT,
        SFX_UI_BOX_COMMON_ITEM_APPEAR,
        SFX_UI_BOX_COMMON_GET_GOLD,
        SFX_UI_BOX_COMMON_GET_DIAMOND,
        SFX_UI_BOX_COMMON_GET_DICE,
        SFX_UI_BOX_COMMON_GET_DICE_LEGEND,
        SFX_UI_BOX_COMMON_RESULT,
        SFX_UI_BOX_COMMON_RESULT_ITEM,
    }
    
    #endregion
    
}