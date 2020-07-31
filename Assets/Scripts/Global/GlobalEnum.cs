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
    
    
    
    
    
}