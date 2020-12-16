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
        bool OnReadyMatchController(ISender sender, ERandomWarsMatchErrorCode errorCode)
        {
            return true;
        }


        bool OnReadyMatchNotifyController(ISender sender, ushort playerUId, int currentSp)
        {
            UnityUtil.Print("Notify Wait", "DeActive Wait Game Start", "white");

            // ingame
            // 둘다 준비 끝낫다고 노티 이므로 
            // 게임 시작하자
            //if (ED.InGameManager.Get() != null)
            //    ED.InGameManager.Get().RecvInGameManager(GameProtocol.DEACTIVE_WAITING_OBJECT_NOTIFY, playerUId, currentSp);

            return true;
        }
    }
}
