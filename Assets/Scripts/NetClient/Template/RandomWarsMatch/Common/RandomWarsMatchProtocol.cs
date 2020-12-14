using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Newtonsoft.Json.Linq;


namespace Template.Stage.RandomWarsMatch.Common
{
    public enum ERandomWarsMatchProtocol
    {
        BEGIN = 4000000,

        REQUEST_MATCH_REQ,
        REQUEST_MATCH_ACK,
        REQUEST_MATCH_NOTIFY,

        STATUS_MATCH_REQ,
        STATUS_MATCH_ACK,
        STATUS_MATCH_NOTIFY,

        CANCEL_MATCH_REQ,
        CANCEL_MATCH_ACK,
        CANCEL_MATCH_NOTIFY,

        JOIN_STAGE_REQ,
        JOIN_STAGE_ACK,
        JOIN_STAGE_NOTIFY,


        END
    }

    public class RandomWarsMatchProtocol : MessageControllerBase
    {
        public RandomWarsMatchProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomWarsMatchProtocol.JOIN_STAGE_REQ, ReceiveJoinStageReq},
                {(int)ERandomWarsMatchProtocol.JOIN_STAGE_ACK, ReceiveJoinStageAck},
                {(int)ERandomWarsMatchProtocol.REQUEST_MATCH_REQ, ReceiveRequestMatchReq},
                {(int)ERandomWarsMatchProtocol.REQUEST_MATCH_ACK, ReceiveRequestMatchAck},
                {(int)ERandomWarsMatchProtocol.STATUS_MATCH_REQ, ReceiveStatusMatchReq},
                {(int)ERandomWarsMatchProtocol.STATUS_MATCH_ACK, ReceiveStatusMatchAck},
                {(int)ERandomWarsMatchProtocol.CANCEL_MATCH_REQ, ReceiveCancelMatchReq},
                {(int)ERandomWarsMatchProtocol.CANCEL_MATCH_ACK, ReceiveCancelMatchAck},
            };
        }


        #region Socket Controller 구현부
        // -------------------------------------------------------------------
        // Socket Controller 구현부
        // -------------------------------------------------------------------

        public bool SendJoinStageReq(ISender sender, string playerSessionId, int deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerSessionId);
                bw.Write(deckIndex);
                return sender.SendMessage((int)ERandomWarsMatchProtocol.JOIN_STAGE_REQ, ms.ToArray());
            }
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo) JoinStageReqDelegate(ISender sender, string playerSessionId, int deckIndex);
        public JoinStageReqDelegate JoinStageReqCallback;
        public bool ReceiveJoinStageReq(ISender sender, byte[] msg, int length)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                var res = JoinStageReqCallback(sender,
                    br.ReadString(),
                    br.ReadInt32());

                return SendJoinStageAck(sender, res.errorCode, res.playerInfo);
            }
        }


        public bool SendJoinStageAck(ISender sender, ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)errorCode);
                playerInfo.Write(bw);
                return sender.SendMessage((int)ERandomWarsMatchProtocol.JOIN_STAGE_ACK, ms.ToArray());
            }
        }


        public delegate bool JoinStageAckDelegate(ISender sender, ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo);
        public JoinStageAckDelegate JoinStageAckCallback;
        public bool ReceiveJoinStageAck(ISender sender, byte[] msg, int length)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                return JoinStageAckCallback(sender,
                    (ERandomWarsMatchErrorCode)br.ReadInt32(),
                    MsgPlayerInfo.Read(br));
            }
        }

        #endregion


        #region Http Controller 구현부        
        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------

        // requestmatch
        public bool SendRequestMatchReq(ISender sender, string playerGuid)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            return sender.SendHttpPost((int)ERandomWarsMatchProtocol.REQUEST_MATCH_REQ, "requestmatch", json.ToString());
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, string ticketId) ReceiveRequestMatchReqDelegate(string playerGuid);
        public ReceiveRequestMatchReqDelegate ReceiveRequestMatchReqCallback;
        public bool ReceiveRequestMatchReq(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            var res = ReceiveRequestMatchReqCallback(playerGuid);
            return SendRequestMatchAck(sender, res.errorCode, res.ticketId);
        }


        public bool SendRequestMatchAck(ISender sender, ERandomWarsMatchErrorCode errorCode, string ticketId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("ticketId", ticketId);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveRequestMatchAckDelegate(ERandomWarsMatchErrorCode errorCode, string ticketId);
        public ReceiveRequestMatchAckDelegate ReceiveRequestMatchAckCallback;
        public bool ReceiveRequestMatchAck(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomWarsMatchErrorCode errorCode = (ERandomWarsMatchErrorCode)(int)jObject["errorCode"];
            string ticketId = (string)jObject["ticketId"];
            return ReceiveRequestMatchAckCallback(errorCode, ticketId);
        }


        // matchstatus
        public bool SendStatusMatchReq(ISender sender, string ticketId)
        {
            JObject json = new JObject();
            json.Add("ticketId", ticketId);
            return sender.SendHttpPost((int)ERandomWarsMatchProtocol.STATUS_MATCH_REQ, "statusmatch", json.ToString());
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId) ReceiveStatusMatchReqDelegate(string ticketId);
        public ReceiveStatusMatchReqDelegate ReceiveStatusMatchReqCallback;
        public bool ReceiveStatusMatchReq(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string ticketId = (string)jObject["ticketId"];
            var res = ReceiveStatusMatchReqCallback(ticketId);

            return SendStatusMatchAck(sender, res.errorCode, res.serverAddr, res.port, res.playerSessionId);
        }


        public bool SendStatusMatchAck(ISender sender, ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("serverAddr", serverAddr);
            json.Add("port", port);
            json.Add("playerSessionId", playerSessionId);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveStatusMatchAckDelegate(ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId);
        public ReceiveStatusMatchAckDelegate ReceiveStatusMatchAckCallback;
        public bool ReceiveStatusMatchAck(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            return ReceiveStatusMatchAckCallback(
                (ERandomWarsMatchErrorCode)(int)jObject["errorCode"],
                jObject["serverAddr"].ToString(),
                (int)jObject["port"],
                jObject["playerSessionId"].ToString());
        }


        // cancelmatch
        public bool SendCancelMatchReq(ISender sender, string ticketId)
        {
            JObject json = new JObject();
            json.Add("ticketId", ticketId);
            return sender.SendHttpPost((int)ERandomWarsMatchProtocol.CANCEL_MATCH_REQ, "cancelmatch", json.ToString());
        }


        public delegate ERandomWarsMatchErrorCode ReceiveCancelMatchReqDelegate(string ticketId);
        public ReceiveCancelMatchReqDelegate ReceiveCancelMatchReqCallback;
        public bool ReceiveCancelMatchReq(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            var res = ReceiveCancelMatchReqCallback(
                (string)jObject["ticketId"]);

            return SendCancelMatchAck(sender, res);
        }


        public bool SendCancelMatchAck(ISender sender, ERandomWarsMatchErrorCode errorCode)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveCancelMatchAckDelegate(ERandomWarsMatchErrorCode errorCode);
        public ReceiveCancelMatchAckDelegate ReceiveCancelMatchAckCallback;
        public bool ReceiveCancelMatchAck(ISender sender, byte[] msg, int length)
        {
            string json = System.Text.Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            return ReceiveCancelMatchAckCallback(
                (ERandomWarsMatchErrorCode)(int)jObject["errorCode"]);
        }
        #endregion
    }
}