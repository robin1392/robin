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
            };         
        }


        #region Login ---------------------------------------------------------------------
        public bool AccountLoginReq(ISender sender, string platformId, int platformType, string guid, string adid, string appid, string version, ReceiveAccountLoginAckDelegate callback)
        {
            ReceiveAccountLoginAckHandler = callback;
            JObject json = new JObject();
            json.Add("platformId", platformId);
            json.Add("platformType", platformType);
            json.Add("guid", guid);
            json.Add("adid", adid);
            json.Add("appid", appid);
            json.Add("version", version);
            return sender.SendHttpPost((int)EGameBaseAccountProtocol.AccountLoginReq, "accountlogin", json.ToString());
        }

        public delegate (EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo) ReceiveAccountLoginReqDelegate(string platformId, int platformType, string guid, string adid, string appid, string version);
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
            var res = ReceiveAccountLoginReqHandler(platformId, platformType, guid, adid, appid, version);
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
    }
}