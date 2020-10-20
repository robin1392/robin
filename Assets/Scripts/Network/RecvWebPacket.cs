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

        // 넷상에러가 아닌 웹 컨텐츠 에러는 별도로 검출해내기 위해서
        if (content.Contains("errorType") || content.Contains("errorMessage"))
        {
            UnityEngine.Debug.LogError( "WEB SERVER ERROR : >>>>>>> " + content);
            
            //
            ErrorCode errorCode = ErrorCode.ErrorCode_WEBSERVER;
            if (cbFail != null)
                cbFail(errorCode);
            
            return;
        }
        
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
                RecvUserAuth(res.userInfo, res.userDeck, res.userDice);
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

    private void RecvUserAuth(User userInfo, UserDeck[] userDeck, UserDice[] userDice)
    {
        UserInfoManager.Get().SetUserKey(userInfo.userId);
    }
    #endregion
    
    
    #region match

    private void MatchResponse(string ticketId)
    {
        if (netMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
        {
            netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
            return;
        }
        
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
        // 세션아이디 받앗으면 연결됫다는거니까..
        if(string.IsNullOrEmpty(res.playerSessionId))
        {
            // 취소 눌럿으면...취소해야되는데...
            if (netMatchStep == Global.E_MATCHSTEP.MATCH_CANCEL)
            {
                netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
                return;
            }
            
            StartCoroutine(StartMatchStatus());
        }
        else
        {
            netMatchStep = Global.E_MATCHSTEP.MATCH_CONNECT;
        
            UnityUtil.Print("Server Addr  Port" , res.serverAddr+ "   " + res.port.ToString() +"   " + res.playerSessionId, "yellow");
        
            // go match -> socket
            NetworkManager.Get().SetAddr(res.serverAddr , res.port , res.playerSessionId);
        
            // 우선 그냥 배틀로 지정하자
            NetworkManager.Get().ConnectServer( Global.PLAY_TYPE.BATTLE , GameStateManager.Get().ServerConnectCallBack);
        }
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
