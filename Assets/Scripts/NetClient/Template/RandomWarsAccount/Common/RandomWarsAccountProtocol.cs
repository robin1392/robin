using System;
using System.Collections.Generic;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Account.RandomWarsAccount.Common
{
    public enum ERandomWarsAccountProtocol
    {
        BEGIN = 1000000,
        
        LOGIN_ACCOUNT_REQ,
        LOGIN_ACCOUNT_ACK,
        LOGIN_ACCOUNT_NOTIFY,

        END
    }


    public class RandomWarsAccountProtocol : BaseProtocol
    {
        public RandomWarsAccountProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
            };

            HttpMessageControllers = new Dictionary<int, HttpControllerDelegate>
            {
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, HttpReceiveLoginAccountReq},
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_ACK, HttpReceiveLoginAccountAck},
           };            
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
#region Http Controller 구현부        
        public bool HttpSendLoginAccountReq(HttpClient client, string platformId, EPlatformType platformType)
        {
            JObject json = new JObject();
            json.Add("platformId", platformId);
            json.Add("platformType", (int)platformType);
            client.Send((int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, "account/loginAccount", json.ToString());
            return true;
        }


        public delegate (ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo) HttpReceiveLoginAccountReqDelegate(string platformId, EPlatformType platformType);
        public HttpReceiveLoginAccountReqDelegate HttpReceiveLoginAccountReqCallback;
        public string HttpReceiveLoginAccountReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveLoginAccountReqCallback(
                jObject["platformId"].ToString(),
                (EPlatformType)(int)jObject["platformType"]);

            return HttpSendLoginAccountAck(res.errorCode, res.accountInfo);
        }


        public string HttpSendLoginAccountAck(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("accountInfo", JsonConvert.SerializeObject(accountInfo, Formatting.Indented));
            return json.ToString();
        }


        public delegate bool HttpReceiveLoginAccountAckDelegate(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo);
        public HttpReceiveLoginAccountAckDelegate HttpReceiveLoginAccountAckCallback;
        public string HttpReceiveLoginAccountAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveLoginAccountAckCallback(
                (ERandomWarsAccountErrorCode)(int)jObject["errorCode"], 
                JsonConvert.DeserializeObject<MsgAccount>(jObject["accountInfo"].ToString()));

            return "";
        }

#endregion

    }
}