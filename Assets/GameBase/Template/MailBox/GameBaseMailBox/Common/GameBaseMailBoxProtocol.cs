using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Net;
using Service.Core;

namespace Template.MailBox.GameBaseMailBox.Common
{
    public enum EGameBaseMailBoxProtocol
    {
        Begin = 80000,

        MailInfoReq,
        MailInfoAck,

        MailRefreshReq,
        MailRefreshAck,

        MailReceiveReq,
        MailReceiveAck,

        MailReceiveAllReq,
        MailReceiveAllAck,

        MailSendReq,
        MailSendAck,

        SystemMailSendReq,
        SystemMailSendAck,
    }

    public class GameBaseMailBoxProtocol : MessageControllerBase
    {
        public GameBaseMailBoxProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                { (int)EGameBaseMailBoxProtocol.MailInfoReq, ReceiveMailBoxInfoReq },
                { (int)EGameBaseMailBoxProtocol.MailInfoAck, ReceiveMailBoxInfoAck },
                { (int)EGameBaseMailBoxProtocol.MailRefreshReq, ReceiveMailBoxRefreshReq },
                { (int)EGameBaseMailBoxProtocol.MailRefreshAck, ReceiveMailBoxRefreshAck },
                { (int)EGameBaseMailBoxProtocol.MailReceiveReq, ReceiveMailReceiveReq },
                { (int)EGameBaseMailBoxProtocol.MailReceiveAck, ReceiveMailReceiveAck },
                { (int)EGameBaseMailBoxProtocol.MailSendReq, ReceiveMailSendReq },
                { (int)EGameBaseMailBoxProtocol.MailSendAck, ReceiveMailSendAck },
                { (int)EGameBaseMailBoxProtocol.SystemMailSendReq, ReceiveSystemMailSendReq },
                { (int)EGameBaseMailBoxProtocol.SystemMailSendAck, ReceiveSystemMailSendAck }
            };
        }

        #region MailBoxInfo ---------------------------------------------------------------------
        public bool MailBoxInfoReq(ISender sender, ReceiveMailBoxInfoAckDelegate callback)
        {
            ReceiveMailBoxInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailInfoReq, "mailboxinfo", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo) ReceiveMailBoxInfoReqDelegate(string userId);

        public ReceiveMailBoxInfoReqDelegate ReceiveMailBoxInfoReqHandler;

        public bool ReceiveMailBoxInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveMailBoxInfoReqHandler(accessToken);
            return MailBoxInfoAck(sender, res.errorCode, res.arrayMailInfo);
        }

        public bool MailBoxInfoAck(ISender sender, GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailInfo", JsonConvert.SerializeObject(arrayMailInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailBoxInfoAckDelegate(GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo);

        public ReceiveMailBoxInfoAckDelegate ReceiveMailBoxInfoAckHandler;

        public bool ReceiveMailBoxInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            MailInfo[] arrayMailInfo = JsonConvert.DeserializeObject<MailInfo[]>(jObject["arrayMailInfo"].ToString());
            return ReceiveMailBoxInfoAckHandler(errorCode, arrayMailInfo);
        }
        #endregion

        #region MailBoxRefresh ---------------------------------------------------------------------
        public bool MailBoxRefreshReq(ISender sender, ReceiveMailBoxRefreshAckDelegate callback)
        {
            ReceiveMailBoxRefreshAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailInfoReq, "mailboxrefresh", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo) ReceiveMailBoxRefreshReqDelegate(string userId);

        public ReceiveMailBoxRefreshReqDelegate ReceiveMailBoxRefreshReqHandler;

        public bool ReceiveMailBoxRefreshReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveMailBoxRefreshReqHandler(accessToken);
            return MailBoxRefreshAck(sender, res.errorCode, res.arrayMailInfo);
        }

        public bool MailBoxRefreshAck(ISender sender, GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailInfo", JsonConvert.SerializeObject(arrayMailInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailBoxRefreshAckDelegate(GameBaseMailBoxErrorCode errorCode, MailInfo[] arrayMailInfo);

        public ReceiveMailBoxRefreshAckDelegate ReceiveMailBoxRefreshAckHandler;

        public bool ReceiveMailBoxRefreshAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            MailInfo[] arrayMailInfo = JsonConvert.DeserializeObject<MailInfo[]>(jObject["arrayMailInfo"].ToString());
            return ReceiveMailBoxRefreshAckHandler(errorCode, arrayMailInfo);
        }
        #endregion

        #region MailReceive ---------------------------------------------------------------------
        public bool MailReceiveReq(ISender sender, string mailId, ReceiveMailReceiveAckDelegate callback)
        {
            ReceiveMailReceiveAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("mailId", mailId);
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailReceiveReq, "mailreceive", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo) ReceiveMailReceiveReqDelegate(string userId, string mailId);
        public ReceiveMailReceiveReqDelegate ReceiveMailReceiveReqHandler;
        public bool ReceiveMailReceiveReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string mailId = (string)jObject["mailId"];
            var res = ReceiveMailReceiveReqHandler(accessToken, mailId);
            return MailReceiveAck(sender, res.errorCode, res.arrayMailItemInfo);
        }

        public bool MailReceiveAck(ISender sender, GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailItemInfo", JsonConvert.SerializeObject(arrayMailItemInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailReceiveAckDelegate(GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo);

        public ReceiveMailReceiveAckDelegate ReceiveMailReceiveAckHandler;

        public bool ReceiveMailReceiveAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayMailItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayMailItemInfo"].ToString());
            return ReceiveMailReceiveAckHandler(errorCode, arrayMailItemInfo);
        }
        #endregion

        #region MailReceiveAll ---------------------------------------------------------------------
        public bool MailReceiveAllReq(ISender sender, ReceiveMailReceiveAllAckDelegate callback)
        {
            ReceiveMailReceiveAllAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailReceiveAllReq, "mailreceiveall", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo) ReceiveMailReceiveAllReqDelegate(string userId);
        public ReceiveMailReceiveAllReqDelegate ReceiveMailReceiveAllReqHandler;
        public bool ReceiveMailReceiveAllReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveMailReceiveAllReqHandler(accessToken);
            return MailReceiveAllAck(sender, res.errorCode, res.arrayMailItemInfo);
        }

        public bool MailReceiveAllAck(ISender sender, GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailItemInfo", JsonConvert.SerializeObject(arrayMailItemInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailReceiveAllAckDelegate(GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo);
        public ReceiveMailReceiveAllAckDelegate ReceiveMailReceiveAllAckHandler;
        public bool ReceiveMailReceiveAllAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayMailItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayMailItemInfo"].ToString());
            return ReceiveMailReceiveAllAckHandler(errorCode, arrayMailItemInfo);
        }
        #endregion

        #region MailSend ---------------------------------------------------------------------
        public bool MailSendReq(ISender sender, string userId, int mailTableId, List<string> listCustomText, List<Dictionary<string, int>> mailItems, ReceiveMailSendAckDelegate callback)
        {
            ReceiveMailSendAckHandler = callback;
            JObject json = new JObject();
            json.Add("userId", userId);
            json.Add("mailTableId", mailTableId);
            json.Add("listCustomText", JsonConvert.SerializeObject(listCustomText));
            json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailSendReq, "mailsend", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode erorCode, MailInfo mailInfo) ReceiveMailSendReqDelegate(string userId, int mailTableId, List<string> listCustomText, List<Dictionary<string, int>> mailItems);
        public ReceiveMailSendReqDelegate ReceiveMailSendReqHandler;
        public bool ReceiveMailSendReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string userId = (string)jObject["userId"];
            int mailTableId = (int)jObject["mailTableId"];
            List<string> listCustomText = JsonConvert.DeserializeObject<List<string>>(jObject["listCustomText"].ToString());
            List<Dictionary<string, int>> mailItems = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(jObject["mailItems"].ToString());
            var res = ReceiveMailSendReqHandler(userId, mailTableId, listCustomText, mailItems);
            return MailSendAck(sender, res.erorCode, res.mailInfo);
        }

        public bool MailSendAck(ISender sender, GameBaseMailBoxErrorCode errorCode, MailInfo mailInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("mailInfo", JsonConvert.SerializeObject(mailInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailSendAckDelegate(GameBaseMailBoxErrorCode errorCode, MailInfo mailInfo);
        public ReceiveMailSendAckDelegate ReceiveMailSendAckHandler;
        public bool ReceiveMailSendAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            MailInfo mailInfo = JsonConvert.DeserializeObject<MailInfo>(jObject["mailinfo"].ToString());
            return ReceiveMailSendAckHandler(errorCode, mailInfo);
        }
        #endregion

        #region SystemMailSend ---------------------------------------------------------------------
        public bool SystemMailSendReq(ISender sender, int mailTableId, List<string> listCustomText, List<Dictionary<string, int>> mailItems, string sendTime, string endTime, int storeDay, ReceiveSystemMailSendAckDelegate callback)
        {
            ReceiveSystemMailSendAckHandler = callback;
            JObject json = new JObject();
            json.Add("mailTableId", mailTableId);
            json.Add("listCustomText", JsonConvert.SerializeObject(listCustomText));
            json.Add("mailItems", JsonConvert.SerializeObject(mailItems));
            json.Add("sendTime", sendTime);
            json.Add("endTime", endTime);
            json.Add("storeDay", storeDay);
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.SystemMailSendReq, "systemmailsend", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, bool result) ReceiveSystemMailSendReqDelegate(int mailTableId, List<string> listCustomText, List<Dictionary<string, int>> mailItems, string sendTime, string endTime, int storeDay);
        public ReceiveSystemMailSendReqDelegate ReceiveSystemMailSendReqHandler;
        public bool ReceiveSystemMailSendReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            int mailTableId = (int)jObject["mailTableId"];
            List<string> listCustomText = JsonConvert.DeserializeObject<List<string>>(jObject["listCustomText"].ToString());
            List<Dictionary<string, int>> mailItems = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(jObject["mailItems"].ToString());
            string sendTime = (string)jObject["sendTime"];
            string endTime = (string)jObject["endTime"];
            int storeDay = (int)jObject["storeDay"];
            var res = ReceiveSystemMailSendReqHandler(mailTableId, listCustomText, mailItems, sendTime, endTime, storeDay);
            return SystemMailSendAck(sender, res.errorCode, res.result);
        }

        public bool SystemMailSendAck(ISender sender, GameBaseMailBoxErrorCode errorCode, bool result)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("result", result);
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveSystemMailSendAckDelegate(GameBaseMailBoxErrorCode errorCode, bool result);
        public ReceiveSystemMailSendAckDelegate ReceiveSystemMailSendAckHandler;
        public bool ReceiveSystemMailSendAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            bool result = (bool)jObject["result"];
            return ReceiveSystemMailSendAckHandler(errorCode, result);
        }
        #endregion
    }
}