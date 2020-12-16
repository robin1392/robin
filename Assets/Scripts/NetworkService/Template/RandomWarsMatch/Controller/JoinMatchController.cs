using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Net;
using Template.Stage.RandomWarsMatch.Common;
using Newtonsoft.Json;

namespace Template.Stage.RandomWarsMatch
{
    public partial class RandomWarsMatchTemplate
    {
        bool OnJoinMatchController(ISender sender, ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo)
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



        bool OnJoinMatchNotifyController(ISender sender, MsgPlayerInfo otherPlayerInfo)
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
