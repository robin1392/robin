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

        //Nev.JSONObject jsoncontent = Nev.JSONObject.Parse(content);
        
        //
        PacketParse(packID, content, cbSuccess, cbFail);
    }

    #endregion
    
    #region packet content

    public void PacketParse( WebProtocol packID, string content, NetCallBack cbSuccess, NetCallBackFail cbFail)
    {
        switch (packID)
        {
            case WebProtocol.WebPD_UserAuth:
            {
                UserAuthRes res = JsonUtility.FromJson<UserAuthRes>(content);
                //RecvUserAuth(content["userId"].ToString());
                RecvUserAuth(res.userId);
                break;
            }
            case WebProtocol.WebPD_Match:
            {
                MatchRequestAck res = JsonUtility.FromJson<MatchRequestAck>(content);
                //MatchResponse(content["TicketId"].ToString());
                MatchResponse(res.TicketId);
                break;
            }
            case WebProtocol.WebPD_MatchStatus:
            {
                MatchStatusAck res = JsonUtility.FromJson<MatchStatusAck>(content);
                MatchStatsAck(res);
                break;
            }
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
        UserInfoManager.Get().SetTicketId(ticketId);

        StartCoroutine(StartMatchStatus());
    }

    public void MatchStatsAck(MatchStatusAck res)
    {
        if (res.gameSessionId.Length == 0)
        {
            //_matchStatus = EMatchStatus.Request;
            StartCoroutine(StartMatchStatus());
            return;
        }
        
        // go match -> socket
    }
    #endregion
    

}
