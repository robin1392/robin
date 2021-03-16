using System.Collections.Generic;
using System.Text;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Template.Account.GameBaseAccount.Common
{
    public enum EGameBaseAccountProtocol
    {
        Begin = 10000,

        AccountLoginReq,
        AccountLoginAck,

        AccountPlatfomrLinkReq,
        AccountPlatfomrLinkAck,

        End,
    }


    public class GameBaseAccountProtocol : MessageControllerBase
    {
        public GameBaseAccountProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)EGameBaseAccountProtocol.AccountLoginReq, ReceiveAccountLoginReq},
                {(int)EGameBaseAccountProtocol.AccountLoginAck, ReceiveAccountLoginAck},
                {(int)EGameBaseAccountProtocol.AccountPlatfomrLinkReq, ReceiveAccountPlatformLinkReq},
                {(int)EGameBaseAccountProtocol.AccountPlatfomrLinkAck, ReceiveAccountPlatformLinkAck},
            };
        }


        #region Login ---------------------------------------------------------------------
        public bool AccountLoginReq(ISender sender, string platformId, int platformType, string guid, string adid, string appid, string version, string os, string osVersion, string device, string country, ReceiveAccountLoginAckDelegate callback)
        {
            ReceiveAccountLoginAckHandler = callback;
            JObject json = new JObject();
            json.Add("platformId", platformId);
            json.Add("platformType", platformType);
            json.Add("guid", guid);
            json.Add("adid", adid);
            json.Add("appid", appid);
            json.Add("version", version);
            json.Add("os", os);
            json.Add("osVersion", osVersion);
            json.Add("device", device);
            json.Add("country", country);
            return sender.SendHttpPost((int)EGameBaseAccountProtocol.AccountLoginReq, "accountlogin", json.ToString());
        }

        public delegate (EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo) ReceiveAccountLoginReqDelegate(string platformId, int platformType, string guid, string adid, string appid, string version, string os, string osVersion, string device, string country);
        public ReceiveAccountLoginReqDelegate ReceiveAccountLoginReqHandler;
        public bool ReceiveAccountLoginReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string platformId = (string)jObject["platformId"];
            int platformType = (int)jObject["platformType"];
            string guid = (string)jObject["guid"];
            string adid = (string)jObject["adid"];
            string appid = (string)jObject["appid"];
            string version = (string)jObject["version"];
            string os = (string)jObject["os"];
            string osVersion = (string)jObject["osVersion"];
            string device = (string)jObject["device"];
            string country = (string)jObject["country"];
            var res = ReceiveAccountLoginReqHandler(platformId, platformType, guid, adid, appid, version, os, osVersion, device, country);
            return AccountLoginAck(sender, res.errorCode, res.accountInfo);
        }

        public bool AccountLoginAck(ISender sender, EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("accountInfo", JsonConvert.SerializeObject(accountInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveAccountLoginAckDelegate(EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo);
        public ReceiveAccountLoginAckDelegate ReceiveAccountLoginAckHandler;
        public bool ReceiveAccountLoginAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            EGameBaseAccountErrorCode errorCode = (EGameBaseAccountErrorCode)(int)jObject["errorCode"];
            AccountInfo accountInfo = JsonConvert.DeserializeObject<AccountInfo>(jObject["accountInfo"].ToString());
            return ReceiveAccountLoginAckHandler(errorCode, accountInfo);
        }
        #endregion


        #region AccountPlatformLink ---------------------------------------------------------------------
        public bool AccountPlatformLinkReq(ISender sender, string platformId, int platformType, bool isConfirm, ReceiveAccountPlatformLinkAckDelegate callback)
        {
            ReceiveAccountPlatformLinkAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("platformId", platformId);
            json.Add("platformType", platformType);
            json.Add("isConfirm", isConfirm);
            return sender.SendHttpPost((int)EGameBaseAccountProtocol.AccountPlatfomrLinkReq, "accountplatformlink", json.ToString());
        }

        public delegate (EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo, bool needConfirm) ReceiveAccountPlatformLinkReqDelegate(string accessToken, string platformId, int platformType, bool isConfirm);
        public ReceiveAccountPlatformLinkReqDelegate ReceiveAccountPlatformLinkReqHandler;
        public bool ReceiveAccountPlatformLinkReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string platformId = (string)jObject["platformId"];
            int platformType = (int)jObject["platformType"];
            bool isConfirm = (bool)jObject["isConfirm"];
            var res = ReceiveAccountPlatformLinkReqHandler(accessToken, platformId, platformType, isConfirm);
            return AccountPlatformLinkAck(sender, res.errorCode, res.accountInfo, res.needConfirm);
        }

        public bool AccountPlatformLinkAck(ISender sender, EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo, bool needConfirm)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("accountInfo", JsonConvert.SerializeObject(accountInfo));
            json.Add("needConfirm", needConfirm);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveAccountPlatformLinkAckDelegate(EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo, bool needConfirm);
        public ReceiveAccountPlatformLinkAckDelegate ReceiveAccountPlatformLinkAckHandler;
        public bool ReceiveAccountPlatformLinkAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            EGameBaseAccountErrorCode errorCode = (EGameBaseAccountErrorCode)(int)jObject["errorCode"];
            AccountInfo accountInfo = JsonConvert.DeserializeObject<AccountInfo>(jObject["accountInfo"].ToString());
            bool needConfirm = (bool)jObject["needConfirm"];
            return ReceiveAccountPlatformLinkAckHandler(errorCode, accountInfo, needConfirm);
        }
        #endregion                 
    }
}