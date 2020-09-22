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
        netMatchStep = Global.E_MATCHSTEP.MATCH_START;
        
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
        //
        if (netMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
        {
            netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
            yield break;
        }
        
        yield return new WaitForSeconds(1.0f);
        
        MatchStatusReq req = new MatchStatusReq();
        req.ticketId = UserInfoManager.Get().GetUserInfo().ticketId;

        string jsonBody = JsonHelper.ToJson<MatchStatusReq>(req);
        UnityUtil.Print("send  : " + jsonBody);

        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_MatchStatus;
        requestData.extraUrl = "/matchstatus";
        
        requestData.FillPacket(jsonBody , null , null);
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }

    #endregion
    
    #region deck update

    public void SendDeckUpdateRequest(int deckIndex, int[] deckIds , NetCallBack cbSuccess, NetCallBackFail cbFail = null)
    {
        _isPacketSend = true;
        
        DeckUpdateReq req = new DeckUpdateReq();

        req.userId = UserInfoManager.Get().GetUserInfo().userID;
        req.deckIndex = (sbyte)deckIndex;
        req.diceIds = new[] {deckIds[0], deckIds[1], deckIds[2], deckIds[3], deckIds[4]};
        
        string jsonBody = JsonHelper.ToJson<DeckUpdateReq>(req);
        UnityUtil.Print("send  : " + jsonBody);

        SendQueue requestData = new SendQueue();
        requestData.packetDef = WebProtocol.WebPD_DeckUpdate;
        requestData.extraUrl = "/deckupdate";
        
        requestData.FillPacket(jsonBody , cbSuccess , cbFail);
        
        WebNetworkCommon.Get().SendPacket(requestData);
    }
    
    #endregion
    
}


