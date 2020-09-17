using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WebPacketDefine
{
    public enum E_MatchStatus
    {
        None,
        Wait,
        Request,
        Complete,
    }


    #region web protocol
    public enum WebProtocol
    {
        WebPD_None,
        WebPD_UserAuth,
        WebPD_Match,
        WebPD_MatchStatus,
        WebPD_DeckUpdate,
        WebPD_MAX,
    }
    
    #endregion
}


namespace ErrorDefine
{
    public enum ErrorCode
    {
        ErrorCode_None,
        ErrorCode_MAX,
    }
}