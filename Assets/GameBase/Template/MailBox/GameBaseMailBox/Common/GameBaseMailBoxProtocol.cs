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

        MailReceiveReq,
        MailReceiveAck,

        MailReceiveAllReq,
        MailReceiveAllAck,
    }

    public class GameBaseMailBoxProtocol : MessageControllerBase
    {
        public GameBaseMailBoxProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                { (int)EGameBaseMailBoxProtocol.MailInfoReq, ReceiveMailBoxInfoReq },
                { (int)EGameBaseMailBoxProtocol.MailInfoAck, ReceiveMailBoxInfoAck },
                { (int)EGameBaseMailBoxProtocol.MailReceiveReq, ReceiveMailReceiveReq },
                { (int)EGameBaseMailBoxProtocol.MailReceiveAck, ReceiveMailReceiveAck },
                { (int)EGameBaseMailBoxProtocol.MailReceiveAllReq, ReceiveMailReceiveAllReq },
                { (int)EGameBaseMailBoxProtocol.MailReceiveAllAck, ReceiveMailReceiveAllAck },
            };
        }

        #region MailBoxInfo ---------------------------------------------------------------------
        public bool MailBoxInfoReq(ISender sender, string accessToken, ReceiveMailBoxInfoAckDelegate callback)
        {
            ReceiveMailBoxInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", accessToken);
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

        #region MailReceive ---------------------------------------------------------------------
        public bool MailReceiveReq(ISender sender, string accessToken, string mailId, ReceiveMailReceiveAckDelegate callback)
        {
            ReceiveMailReceiveAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", accessToken);
            json.Add("mailId", mailId);
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailReceiveReq, "mailreceive", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo) ReceiveMailReceiveReqDelegate(string userId, string mailId);
        public ReceiveMailReceiveReqDelegate ReceiveMailReceiveReqHandler;
        public bool ReceiveMailReceiveReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string mailId = (string)jObject["mailId"];
            var res = ReceiveMailReceiveReqHandler(accessToken, mailId);
            return MailReceiveAck(sender, res.errorCode, res.arrayMailItemInfo, res.arrayMailInfo);
        }

        public bool MailReceiveAck(ISender sender, GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailItem", JsonConvert.SerializeObject(arrayMailItemInfo));
            json.Add("arrayMailInfo", JsonConvert.SerializeObject(arrayMailInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailReceiveAckDelegate(GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo);

        public ReceiveMailReceiveAckDelegate ReceiveMailReceiveAckHandler;

        public bool ReceiveMailReceiveAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayMailItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayMailItemInfo"].ToString());
            MailInfo[] arrayMailInfo = JsonConvert.DeserializeObject<MailInfo[]>(jObject["arrayMailInfo"].ToString());
            return ReceiveMailReceiveAckHandler(errorCode, arrayMailItemInfo, arrayMailInfo);
        }
        #endregion

        #region MailReceiveAll ---------------------------------------------------------------------
        public bool MailReceiveAllReq(ISender sender, string accessToken, ReceiveMailReceiveAllAckDelegate callback)
        {
            ReceiveMailReceiveAllAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", accessToken);
            return sender.SendHttpPost((int)EGameBaseMailBoxProtocol.MailReceiveAllReq, "mailreceiveall", json.ToString());
        }

        public delegate (GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo) ReceiveMailReceiveAllReqDelegate(string userId);
        public ReceiveMailReceiveAllReqDelegate ReceiveMailReceiveAllReqHandler;
        public bool ReceiveMailReceiveAllReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveMailReceiveAllReqHandler(accessToken);
            return MailReceiveAllAck(sender, res.errorCode, res.arrayMailItemInfo, res.arrayMailInfo);
        }

        public bool MailReceiveAllAck(ISender sender, GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayMailItemInfo", JsonConvert.SerializeObject(arrayMailItemInfo));
            json.Add("arrayMailInfo", JsonConvert.SerializeObject(arrayMailInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveMailReceiveAllAckDelegate(GameBaseMailBoxErrorCode errorCode, ItemBaseInfo[] arrayMailItemInfo, MailInfo[] arrayMailInfo);
        public ReceiveMailReceiveAllAckDelegate ReceiveMailReceiveAllAckHandler;
        public bool ReceiveMailReceiveAllAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseMailBoxErrorCode errorCode = (GameBaseMailBoxErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayMailItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayMailItemInfo"].ToString());
            MailInfo[] arrayMailInfo = JsonConvert.DeserializeObject<MailInfo[]>(jObject["arrayMailInfo"].ToString());
            return ReceiveMailReceiveAllAckHandler(errorCode, arrayMailItemInfo, arrayMailInfo);
        }
        #endregion
    }
}