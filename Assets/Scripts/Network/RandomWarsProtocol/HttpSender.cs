using System;
using System.Collections.Generic;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;
using Newtonsoft.Json;

namespace RandomWarsProtocol
{
    public class HttpSender
    {
        HttpClient _httpService;


        public HttpSender(HttpClient httpService)
        {
            _httpService = httpService;
        }


        public void UserAuthReq(MsgUserAuthReq msg)
        {
            _httpService.Send((int)GameProtocol.AUTH_USER_REQ, "userauth", JsonConvert.SerializeObject(msg));
        }


        public string AuthUserAck(MsgUserAuthAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        public void UpdateDeckReq(MsgUpdateDeckReq msg)
        {
            _httpService.Send((int)GameProtocol.UPDATE_DECK_REQ, "deckupdate", JsonConvert.SerializeObject(msg));
        }


        public string UpdateDeckAck(MsgUpdateDeckAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        public void StartMatchReq(MsgStartMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.START_MATCH_REQ, "matchrequest", JsonConvert.SerializeObject(msg));
        }


        public string StartMatchAck(MsgStartMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        public void StatusMatchReq(MsgStatusMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STATUS_MATCH_REQ, "matchstatus", JsonConvert.SerializeObject(msg));
        }


        public string StatusMatchAck(MsgStatusMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }

        public void StopMatchReq(MsgStopMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STOP_MATCH_REQ, "matchstop", JsonConvert.SerializeObject(msg));
        }


        public string StopMatchAck(MsgStopMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        public void OpenBoxReq(MsgOpenBoxReq msg)
        {
            _httpService.Send((int)GameProtocol.OPEN_BOX_REQ, "boxopen", JsonConvert.SerializeObject(msg));
        }


        public string OpenBoxAck(MsgOpenBoxAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }

        public void LevelUpDiceReq(MsgLevelUpDiceReq msg)
        {
            _httpService.Send((int)GameProtocol.LEVELUP_DICE_REQ, "dicelevelup", JsonConvert.SerializeObject(msg));
        }


        public string LevelUpDiceAck(MsgLevelUpDiceAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }
    }
}
