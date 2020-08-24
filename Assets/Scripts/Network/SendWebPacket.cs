using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PacketDefine;


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
        UnityUtil.Print(jsonBody);

        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_None;
        requestData.recvCB = new RecvCallback(RecvPacket);
        requestData.recvFailCB = new RecvCallback(RecvFail);

        requestData.cb_Success = cbSuccess;
        requestData.cb_Fail = cbFail;

        requestData.packetData = jsonBody;
        
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }
    #endregion
    
}


