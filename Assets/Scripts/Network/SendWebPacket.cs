using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 
using WebPacketDefine;


public partial class WebPacket : Singleton<WebPacket>
{

    #region user auth
    //
    public void SendUserAuth( string userkey ,  NetCallBack cbSuccess , NetCallBackFail cbFail = null )
    {
        UserAuthReq req = new UserAuthReq();
        req.userId = userkey;
        
        //
        string jsonBody = JsonHelper.ToJson<UserAuthReq>(req);
        UnityUtil.Print("send  : " + jsonBody);
        
        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_UserAuth;
        requestData.extraUrl = "/userauth";
        
        requestData.FillPacket(jsonBody , cbSuccess , cbFail);
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }
    
    #endregion

    #region match req

    public void SendMatchRequest(string userkey, NetCallBack cbSuccess, NetCallBackFail cbFail = null)
    {
        MatchRequestReq req = new MatchRequestReq();
        req.userId = userkey;
        
        string jsonBody = JsonHelper.ToJson<MatchRequestReq>(req);
        UnityUtil.Print("send  : " + jsonBody);

        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_Match;
        requestData.extraUrl = "/matchrequest";
        
        requestData.FillPacket(jsonBody , cbSuccess , cbFail);
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }

    public IEnumerator StartMatchStatus()
    {
        yield return new WaitForSeconds(1.0f);
        
        MatchStatusReq req = new MatchStatusReq();
        req.ticketId = UserInfoManager.Get().GetUserInfo().ticketId;

        string jsonBody = JsonHelper.ToJson<MatchStatusReq>(req);
        UnityUtil.Print("send  : " + jsonBody);

        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_MatchStatus;
        requestData.extraUrl = "/matchrequest";
        
        requestData.FillPacket(jsonBody , null , null);
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }

    #endregion
    
}


