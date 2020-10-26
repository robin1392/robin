using System;
using System.Collections.Generic;
using RandomWarsService.Network.Http;

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


        public void UserAuthReq(string userId)
        {
            var jObjcet = new JsonObject();
            jObjcet.Add("userId", userId);

            string json = _jsonSerializer.SerializeObject(jObjcet);
            _httpService.Send((int)GameProtocol.AUTH_USER_REQ, "userauth", json);
        }


        public string AuthUserAck(GameErrorCode errorCode, MsgUserInfo userInfo, MsgUserDeck[] userDeck, MsgUserDice[] userDice)
        {
            var jObjcet = new JsonObject();
            jObjcet.Add("errorCode", errorCode);
            jObjcet.Add("userInfo", userInfo);
            jObjcet.Add("userDeck", userDeck);
            jObjcet.Add("userDice", userDice);
            return _jsonSerializer.SerializeObject(jObjcet);
        }
    }
}
