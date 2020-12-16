using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Template.Stage.RandomWarsMatch.Common;
using Newtonsoft.Json;

namespace Template.Stage.RandomWarsMatch
{
    public partial class RandomWarsMatchTemplate
    {

        bool OnRequestMatchController(ERandomWarsMatchErrorCode errorCode, string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                UnityUtil.Print("ticket id null");
                return false;
            }


            UserInfoManager.Get().SetTicketId(ticketId);
            UnityUtil.Print("RECV MATCH START => ticketId", ticketId, "green");

            NetworkService.Get().GameSession.Send(ERandomWarsMatchProtocol.STATUS_MATCH_REQ, ticketId);
            return true;

        }


        bool OnStatusMatchController(ERandomWarsMatchErrorCode errorCode, string ServerAddr, int port, string PlayerSessionId)
        {
            if (string.IsNullOrEmpty(PlayerSessionId))
            {
                Task.Delay(1000).ContinueWith(t => NetworkService.Get().GameSession.Send(ERandomWarsMatchProtocol.STATUS_MATCH_REQ, UserInfoManager.Get().GetUserInfo().ticketId));
            }
            else
            {
                // 우선 그냥 배틀로 지정하자
                NetworkService.Get().ConnectGameServer(Global.PLAY_TYPE.BATTLE, ServerAddr, port, PlayerSessionId);
            }

            UnityUtil.Print("RECV MATCH STATUS => ", string.Format("server:{0}, player-session-id:{1}", ServerAddr + ":" + port, PlayerSessionId), "green");

            return true;
        }


        bool OnCancelMatchController(ERandomWarsMatchErrorCode errorCode)
        {
            if (errorCode == ERandomWarsMatchErrorCode.SUCCESS)
            {
                UI_SearchingPopup searchingPopup = GameObject.FindObjectOfType<UI_SearchingPopup>();
                searchingPopup.ClickSearchingCancelResult();
            }

            UnityUtil.Print("RECV CANCEL MATCH => index", string.Format("playerGuid:{0}, errorCode:[{1}]", UserInfoManager.Get().GetUserInfo().userID
                , errorCode.ToString()), "green");
            return true;
        }


        bool OnJoinMatchController(ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo)
        {
            UnityUtil.Print(" join recv ", "errocode : " + errorCode, "white");
            UnityUtil.Print("join my info ", playerInfo.PlayerUId + "  " + playerInfo.Name + " , " + playerInfo.IsBottomPlayer, "white");
            UnityUtil.Print(" join recv ", JsonConvert.SerializeObject(playerInfo), "white");

            if (errorCode != ERandomWarsMatchErrorCode.SUCCESS)
            {
                return false;
            }


            NetworkManager.Get().GetNetInfo().SetPlayerInfo(playerInfo);
            NetworkManager.Get().IsMaster = playerInfo.IsMaster;
            GameStateManager.Get().CheckSendInGame();
            return true;
        }


        bool OnNotifyJoinMatchController(ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo otherPlayerInfo)
        {
            UnityUtil.Print("other info ", otherPlayerInfo.PlayerUId + "  " + otherPlayerInfo.Name + " , " + otherPlayerInfo.IsBottomPlayer, "white");
            UnityUtil.Print(" join recv ", JsonConvert.SerializeObject(otherPlayerInfo), "white");


            // menu
            NetworkManager.Get().GetNetInfo().SetOtherInfo(otherPlayerInfo);
            GameStateManager.Get().CheckSendInGame();
            return true;
        }
    }
}
