using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Match.RandomwarsMatch.Common
{
    public enum ERandomwarsMatchProtocol
    {
        Begin = ETemplateType.Match * 10000,

        MatchRequestReq,
        MatchRequestAck,

        MatchStatusReq,
        MatchStatusAck,

        MatchCancelReq,
        MatchCancelAck,

        End,
    }


    public class RandomwarsMatchProtocol : MessageControllerBase
    {
        public RandomwarsMatchProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsMatchProtocol.MatchRequestReq, ReceiveMatchRequestReq},
                {(int)ERandomwarsMatchProtocol.MatchRequestAck, ReceiveMatchRequestAck},
                {(int)ERandomwarsMatchProtocol.MatchStatusReq, ReceiveMatchStatusReq},
                {(int)ERandomwarsMatchProtocol.MatchStatusAck, ReceiveMatchStatusAck},
                {(int)ERandomwarsMatchProtocol.MatchCancelReq, ReceiveMatchCancelReq},
                {(int)ERandomwarsMatchProtocol.MatchCancelAck, ReceiveMatchCancelAck},  
            };         
        }


        #region MatchRequest ---------------------------------------------------------------------
        public bool MatchRequestReq(ISender sender, int gameMode, ReceiveMatchRequestAckDelegate callback)
        {
            ReceiveMatchRequestAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("gameMode", gameMode);
            json.Add("deckIndex", 0);
            return sender.SendHttpPost((int)ERandomwarsMatchProtocol.MatchRequestReq, "matchrequest", json.ToString());
        }

        public delegate (ERandomwarsMatchErrorCode errorCode, string ticketId) ReceiveMatchRequestReqDelegate(string accessToken, int gameMode, int deckIndex);
        public ReceiveMatchRequestReqDelegate ReceiveMatchRequestReqHandler;
        public bool ReceiveMatchRequestReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int gameMode = (int)jObject["gameMode"];
            int deckIndex = (int)jObject["deckIndex"];
            var res = ReceiveMatchRequestReqHandler(accessToken, gameMode, deckIndex);
            return MatchRequestAck(sender, res.errorCode, res.ticketId);     
        }        

        public bool MatchRequestAck(ISender sender, ERandomwarsMatchErrorCode errorCode, string ticketId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("ticketId", (string)ticketId);
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveMatchRequestAckDelegate(ERandomwarsMatchErrorCode errorCode, string ticketId);
        public ReceiveMatchRequestAckDelegate ReceiveMatchRequestAckHandler;
        public bool ReceiveMatchRequestAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsMatchErrorCode errorCode = (ERandomwarsMatchErrorCode)(int)jObject["errorCode"];
            string ticketId = (string)jObject["ticketId"];
            return ReceiveMatchRequestAckHandler(errorCode, ticketId);
        }
        #endregion    

        #region MatchStatus ---------------------------------------------------------------------
        public bool MatchStatusReq(ISender sender, string ticketId, ReceiveMatchStatusAckDelegate callback)
        {
            ReceiveMatchStatusAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("ticketId", ticketId);
            return sender.SendHttpPost((int)ERandomwarsMatchProtocol.MatchStatusReq, "matchstatus", json.ToString());
        }

        public delegate (ERandomwarsMatchErrorCode errorCode, string playerSessionId, string ipAddress, int port) ReceiveMatchStatusReqDelegate(string accessToken, string ticketId);
        public ReceiveMatchStatusReqDelegate ReceiveMatchStatusReqHandler;
        public bool ReceiveMatchStatusReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string ticketId = (string)jObject["ticketId"];
            var res = ReceiveMatchStatusReqHandler(accessToken, ticketId);
            return MatchStatusAck(sender, res.errorCode, res.playerSessionId, res.ipAddress, res.port);     
        }        

        public bool MatchStatusAck(ISender sender, ERandomwarsMatchErrorCode errorCode, string playerSessionId, string ipAddress, int port)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("playerSessionId", playerSessionId);
            json.Add("ipAddress", ipAddress);
            json.Add("port", port);
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveMatchStatusAckDelegate(ERandomwarsMatchErrorCode errorCode, string playerSessionId, string ipAddress, int port);
        public ReceiveMatchStatusAckDelegate ReceiveMatchStatusAckHandler;
        public bool ReceiveMatchStatusAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsMatchErrorCode errorCode = (ERandomwarsMatchErrorCode)(int)jObject["errorCode"];
            string playerSessionId = (string)jObject["playerSessionId"];
            string ipAddress = (string)jObject["ipAddress"];
            int port = (int)jObject["port"];
            return ReceiveMatchStatusAckHandler(errorCode, playerSessionId, ipAddress, port);
        }
        #endregion    

        #region MatchCancel ---------------------------------------------------------------------
        public bool MatchCancelReq(ISender sender, string ticketId, ReceiveMatchCancelAckDelegate callback)
        {
            ReceiveMatchCancelAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("ticketId", ticketId
            );
            return sender.SendHttpPost((int)ERandomwarsMatchProtocol.MatchCancelReq, "matchcancel", json.ToString());
        }

        public delegate ERandomwarsMatchErrorCode ReceiveMatchCancelReqDelegate(string accessToken, string ticketId);
        public ReceiveMatchCancelReqDelegate ReceiveMatchCancelReqHandler;
        public bool ReceiveMatchCancelReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string ticketId = (string)jObject["ticketId"];
            var res = ReceiveMatchCancelReqHandler(accessToken, ticketId);
            return MatchCancelAck(sender, res);     
        }        

        public bool MatchCancelAck(ISender sender, ERandomwarsMatchErrorCode errorCode)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveMatchCancelAckDelegate(ERandomwarsMatchErrorCode errorCode);
        public ReceiveMatchCancelAckDelegate ReceiveMatchCancelAckHandler;
        public bool ReceiveMatchCancelAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsMatchErrorCode errorCode = (ERandomwarsMatchErrorCode)(int)jObject["errorCode"];
            return ReceiveMatchCancelAckHandler(errorCode);
        }
        #endregion            
    }
}