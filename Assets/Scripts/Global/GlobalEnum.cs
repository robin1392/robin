using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Global Enum 선언
/// </summary>


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
    
    
}