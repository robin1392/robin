using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PacketDefine
{

    #region web protocol
    public enum WebProtocol
    {
        WebPD_None,
        WebPD_MAX,
    }
    
    #endregion
    
    
    #region socket protocol
    public enum GameProtocol : short
    {
        GPD_MAX,
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