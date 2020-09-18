using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Global Enum 선언
/// </summary>
namespace ED
{
    public enum DICE_GRADE
    {
        NORMAL = 0,
        MAGIC = 1,
        HEROIC = 2,
        LEGENDARY = 3,
    }
    
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
    
    
}