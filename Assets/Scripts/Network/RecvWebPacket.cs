using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ErrorDefine;
using WebPacketDefine;
//using WebSocketSharp;

public partial class WebPacket : Singleton<WebPacket>
{
    #region packet fail

    public void RecvFail( WebProtocol packID , string content , NetCallBack cbSuccess , NetCallBackFail cbFail )
    {
#if UNITY_EDITOR
        UnityUtil.Print("Recv Packet Error    :  ", content, "red");
#endif

        //
        ErrorCode errorCode = ErrorCode.ErrorCode_None;

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
                RecvUserAuth(res.userId);
                break;
            }
            case WebProtocol.WebPD_Match:
            {
                MatchRequestAck res = JsonUtility.FromJson<MatchRequestAck>(content);
                MatchResponse(res.ticketId);
                break;
            }
            case WebProtocol.WebPD_MatchStatus:
            {
                MatchStatusAck res = JsonUtility.FromJson<MatchStatusAck>(content);
                MatchStatsAck(res);
                break;
            }
            case WebProtocol.WebPD_DeckUpdate:
            {
                content = content.Replace(@"\" , "").Replace(", " , ",").Replace("\"[ " , "[").Replace("]\"" , "]");
                DeckUpdateAck res = JsonUtility.FromJson<DeckUpdateAck>(content);
                RecvDeckUpdate(res);
                break;
            }
        }

        if (cbSuccess != null)
            cbSuccess();
    }
    #endregion


    #region user auth

    private void RecvUserAuth(string userkey)
    {
        UserInfoManager.Get().SetUserKey(userkey);
    }
    #endregion
    
    
    #region match

    private void MatchResponse(string ticketId)
    {
        //if (ticketId.IsNullOrEmpty())
        if(string.IsNullOrEmpty(ticketId))
        {
            UnityUtil.Print("ticket id null");
            return;
        }
        
        UserInfoManager.Get().SetTicketId(ticketId);

        StartCoroutine(StartMatchStatus());
    }

    private void MatchStatsAck(MatchStatusAck res)
    {
        //if (res.playerSessionId.IsNullOrEmpty())
        if(string.IsNullOrEmpty(res.playerSessionId))
        {
            //_matchStatus = EMatchStatus.Request;
            StartCoroutine(StartMatchStatus());
            return;
        }
        
        UnityUtil.Print("Server Addr  Port" , res.serverAddr+ "   " + res.port.ToString() +"   " + res.playerSessionId, "yellow");
        
        // go match -> socket
        NetworkManager.Get().SetAddr(res.serverAddr , res.port , res.playerSessionId);
        
        // 우선 그냥 배틀로 지정하자
        NetworkManager.Get().ConnectServer( PLAY_TYPE.BATTLE , GameStateManager.Get().ServerConnectCallBack);
    }
    #endregion
    
    #region deck update

    private void RecvDeckUpdate(DeckUpdateAck res)
    {
        //
        UserInfoManager.Get().GetUserInfo().SetDeck(res.deckIndex, $"{res.diceIds[0]}/{res.diceIds[1]}/{res.diceIds[2]}/{res.diceIds[3]}/{res.diceIds[4]}");
        
        _isPacketSend = false;
    }
    #endregion

}
