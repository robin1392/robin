using System;
using System.Collections.Generic;
using Service.Net;
using System.IO;
using System.Text;
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


    public class RandomWarsAccountProtocol : MessageControllerBase
    {
        public RandomWarsAccountProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, ReceiveLoginAccountReq},
                {(int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_ACK, ReceiveLoginAccountAck},
            };
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
#region Http Controller 구현부        
        public bool SendLoginAccountReq(ISender sender, string platformId, EPlatformType platformType)
        {
            JObject json = new JObject();
            json.Add("platformId", platformId);
            json.Add("platformType", (int)platformType);
            return sender.SendHttpPost((int)ERandomWarsAccountProtocol.LOGIN_ACCOUNT_REQ, "accountlogin", json.ToString());
        }


        public delegate (ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo) ReceiveLoginAccountReqDelegate(string platformId, EPlatformType platformType);
        public ReceiveLoginAccountReqDelegate ReceiveLoginAccountReqCallback;
        public bool ReceiveLoginAccountReq(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            string platformId = (string)jObject["platformId"];
            EPlatformType platformType = (EPlatformType)(int)jObject["platformType"];
            var res = ReceiveLoginAccountReqCallback( 
                platformId, 
                platformType);
            return SendLoginAccountAck(sender, res.errorCode, res.accountInfo);
        }


        public bool SendLoginAccountAck(ISender sender, ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("accountInfo", JsonConvert.SerializeObject(accountInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveLoginAccountAckDelegate(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo);
        public ReceiveLoginAccountAckDelegate ReceiveLoginAccountAckCallback;
        public bool ReceiveLoginAccountAck(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            ERandomWarsAccountErrorCode errorCode = (ERandomWarsAccountErrorCode)(int)jObject["errorCode"];
            MsgAccount accountInfo = JsonConvert.DeserializeObject<MsgAccount>(jObject["accountInfo"].ToString());
            return ReceiveLoginAccountAckCallback(errorCode, accountInfo);
        }
#endregion

    }
}