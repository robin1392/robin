using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Template.Stage.RandomWarsMatch.Common;

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

            NetService.Get().Send(ERandomWarsMatchProtocol.STATUS_MATCH_REQ, ticketId);
            return true;

        }


        bool OnStatusMatchController(ERandomWarsMatchErrorCode errorCode, string ServerAddr, int port, string PlayerSessionId)
        {
            if (string.IsNullOrEmpty(PlayerSessionId))
            {
                Task.Delay(1000).ContinueWith(t => NetService.Get().Send(ERandomWarsMatchProtocol.STATUS_MATCH_REQ, UserInfoManager.Get().GetUserInfo().ticketId));
            }
            else
            {
                // 우선 그냥 배틀로 지정하자
                NetService.Get().ConnectGameServer(Global.PLAY_TYPE.BATTLE, ServerAddr, port, PlayerSessionId);
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

            UnityUtil.Print("RECV CANCEL MATCH => index", string.Format("playerGuid:{0}, errorCode:[{1}]", UserInfoManager.Get().GetUserInfo().playerGuid
                , errorCode.ToString()), "green");
            return true;
        }
    }
}
