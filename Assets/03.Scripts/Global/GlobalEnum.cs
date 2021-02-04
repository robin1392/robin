using System.Collections;
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
        //Info_Type  = 0,
        Info_Hp = 0,
        Info_MoveSpeed,
        Info_AtkPower,
        Info_AtkSpeed,
        Info_Skill,
        Info_Cooltime,
        Info_Range,
        Info_Target,
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
        SFX_COMMON_EXPLOSION,

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
        
        // Ingame UI
        SFX_INGAME_UI_GET_DICE = 20000,
        SFX_INGAME_UI_DICE_MERGE,
        SFX_INGAME_UI_SP_LEVEL_UP,
        SFX_INGAME_UI_DICE_LEVEL_UP,
        SFX_INGAME_UI_INFO_ON,
        SFX_INGAME_UI_INFO_OFF,
        SFX_INGAME_UI_RESULT_REWARD,
        
        // Ingame
        SFX_INGAME_COMMON_EXPLOSION = 30000,
        SFX_INGAME_TOWER_FALLDOWN,
        SFX_INGAME_TOWER_EXPLOSION,
        SFX_INGAME_WARRIOR_BLADE,
        SFX_INGAME_HEALER_HEAL,
        SFX_INGAME_MISSILE_FIRE,
        SFX_INGAME_MINE_DROP,
        SFX_INGAME_MISSILE_SPEAR,
        SFX_INGAME_POISON_EXPLOSION,
        SFX_INGAME_SHIELD_DEFENSE,
        SFX_INGAME_FLAME,
        SFX_INGAME_GUNNER_SHOT,
        SFX_INGAME_ROCK_ROLLING,
        SFX_INGAME_ICEBALL_MISSILE,
        SFX_INGAME_ICEBALL_EXPLOSION,
        SFX_INGAME_FLAK_SHOT,
        SFX_INGAME_FLAG,
        SFX_INGAME_MORTAR_SHOT,
        SFX_INGAME_MORTAR_MISSILE,
        SFX_INGAME_NECROMANCER_SUMMON,
        SFX_INGAME_NECROMANCER_ATTACK,
        SFX_INGAME_BULL_RUSH,
        SFX_INGAME_SUPPORT_JUMP,
        SFX_INGAME_SUPPORT_LANDING,
        SFX_INGAME_BERSERKER_WHIRL,
        SFX_INGAME_ZOMBIE_BLADE,
        SFX_INGAME_ZOMBIE_POISON,
        SFX_INGAME_LASER_BEAM,
        SFX_INGAME_LASER_SHOT,
        SFX_INGAME_MISSILE_ROCKET,
        SFX_INGAME_SNIPER_AIMING,
        SFX_INGAME_SNIPER_SHOT,
        SFX_INGAME_DRAGON_BABY_ATTACK,
        SFX_INGAME_DRAGON_TRANSFORM,
        SFX_INGAME_DRAGON_BIG_ATTACK,
        SFX_INGAME_DRAGON_BIG_BOMB,
        SFX_INGAME_TOWER_ATTACK,
        SFX_INGAME_GOLEM_PUNCH,
        SFX_INGAME_GOLEM_SPLIT,
        SFX_INGAME_GOLEM_EXPLO,
        SFX_INGAME_MAGICIAN_PUPPETS,
        SFX_INGAME_HEALER,
        SFX_INGAME_BOMBER,
        
        // 협동전
        SFX_INGAME_TOWER_BOSS_GEN_START = 40000,
        SFX_INGAME_BOSS1_JUMP,
        SFX_INGAME_BOSS1_LANDING,
        SFX_INGAME_BOSS1_PUNCHING,
    }
    
    #endregion
    
}