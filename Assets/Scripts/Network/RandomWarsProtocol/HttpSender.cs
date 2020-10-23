using System;
using System.Collections.Generic;
using System.Text;
using RandomWarsService.Network.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RandomWarsProtocol
{
    public class HttpSender
    {
        HttpService _httpService;

        public HttpSender(HttpService httpService)
        {
            _httpService = httpService;
        }


        public void UserAuthReq(string userId)
        {
            var json = new JObject();
            json.Add("userId", userId);
            _httpService.Enqueue((int)GameProtocol.AUTH_USER_REQ, "userauth", json.ToString());
        }
    }
}
