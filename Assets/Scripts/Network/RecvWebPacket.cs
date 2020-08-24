using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Nev;

public partial class WebPacket : Singleton<WebPacket>
{
    #region packet fail

    public void RecvFail(PacketDefine.WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail )
    {
        
    }
    #endregion
    
    
    #region packet success

    public void RecvPacket(PacketDefine.WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail )
    {
        
#if UNITY_EDITOR
        UnityUtil.Print("Complete Packet     :  ", content, "green");
#endif
        
        //JSONArray jsonArray = JSONArray.Parse(content);
        //JsonUtility.FromJson<>(content);
        
        //
        PacketParse(packID, content, cbSuccess, cbFail);
    }

    #endregion
    
    #region packet content

    public void PacketParse(PacketDefine.WebProtocol packID, string content, NetCallBack cbSuccess, NetCallBackFail cbFail)
    {
        
    }
    #endregion

}
