using System;
using System.Collections.Generic;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;

namespace RandomWarsProtocol
{
    public class HttpSender
    {
        HttpClient _httpService;
        IJsonSerializer _jsonSerializer;


        public HttpSender(HttpClient httpService, IJsonSerializer jsonSerializer)
        {
            _httpService = httpService;
            _jsonSerializer = jsonSerializer;
        }


        public void UserAuthReq(MsgUserAuthReq msg)
        {
            _httpService.Send((int)GameProtocol.AUTH_USER_REQ, "userauth", _jsonSerializer.SerializeObject(msg));
        }


        public string AuthUserAck(MsgUserAuthAck msg)
        {
            return _jsonSerializer.SerializeObject(msg);
        }


        public void UpdateDeckReq(MsgUpdateDeckReq msg)
        {
            _httpService.Send((int)GameProtocol.UPDATE_DECK_REQ, "deckupdate", _jsonSerializer.SerializeObject(msg));
        }


        public string UpdateDeckAck(MsgUpdateDeckAck msg)
        {
            return _jsonSerializer.SerializeObject(msg);
        }


        public void StartMatchReq(MsgStartMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.START_MATCH_REQ, "matchrequest", _jsonSerializer.SerializeObject(msg));
        }


        public string StartMatchAck(MsgStartMatchAck msg)
        {
            return _jsonSerializer.SerializeObject(msg);
        }


        public void StatusMatchReq(MsgStatusMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STATUS_MATCH_REQ, "matchstatus", _jsonSerializer.SerializeObject(msg));
        }


        public string StatusMatchAck(MsgStatusMatchAck msg)
        {
            return _jsonSerializer.SerializeObject(msg);
        }

        public void StopMatchReq(MsgStopMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STOP_MATCH_REQ, "matchstop", _jsonSerializer.SerializeObject(msg));
        }


        public string StopMatchAck(MsgStopMatchAck msg)
        {
            return _jsonSerializer.SerializeObject(msg);
        }
    }
}
