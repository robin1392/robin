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
    }
}
