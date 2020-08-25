using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ErrorDefine;
using WebPacketDefine;

public partial class WebPacket : Singleton<WebPacket>
{
    #region packet fail

    public void RecvFail( WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail )
    {
#if UNITY_EDITOR
        UnityUtil.Print("Recv Packet Error    :  ", content, "red");
#endif

        //
        ErrorDefine.ErrorCode errorCode = ErrorCode.ErrorCode_None;
        //
        if (cbFail != null)
            cbFail(errorCode);

    }
    #endregion
    
    
    #region packet success

    public void RecvPacket( WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail )
    {
        
#if UNITY_EDITOR
        UnityUtil.Print("Complete Packet     :  ", content, "green");
#endif

        Nev.JSONObject jsoncontent = Nev.JSONObject.Parse(content);
        //JSONArray jsonArray = JSONArray.Parse(content);
        //JsonUtility.FromJson<>(content);
        
        //
        PacketParse(packID, jsoncontent, cbSuccess, cbFail);
    }

    #endregion
    
    #region packet content

    public void PacketParse( WebProtocol packID, Nev.JSONObject content, NetCallBack cbSuccess, NetCallBackFail cbFail)
    {
        switch (packID)
        {
            case WebProtocol.WebPD_UserAuth:
                RecvUserAuth(content["userId"].ToString());
                break;
            case WebProtocol.WebPD_Match:
                //content[""]
                break;
        }

        if (cbSuccess != null)
            cbSuccess();
    }
    #endregion


    #region user auth

    public void RecvUserAuth(string userkey)
    {
        UserInfoManager.Get().SetUserKey(userkey);
    }
    #endregion
    
    
    #region match

    public void MatchResponse(string ticketId)
    {
        
    }
    #endregion
    

}
