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

        GET_MATCH_STATUS_REQ,
        GET_MATCH_STATUS_ACK,
        GET_MATCH_STATUS_NOTIFY,

        STOP_MATCH_REQ,
        STOP_MATCH_ACK,
        STOP_MATCH_NOTIFY,

        JOIN_STAGE_REQ,
        JOIN_STAGE_ACK,
        JOIN_STAGE_NOTIFY,


        END
    }

    public class RandomWarsMatchProtocol : BaseProtocol
    {
        public RandomWarsMatchProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomWarsMatchProtocol.JOIN_STAGE_REQ, ReceiveJoinStageReq},
            };

            HttpMessageControllers = new Dictionary<int, HttpControllerDelegate>
            {
                {(int)ERandomWarsMatchProtocol.REQUEST_MATCH_REQ, HttpReceiveRequestMatchReq},
                {(int)ERandomWarsMatchProtocol.GET_MATCH_STATUS_REQ, HttpReceiveGetMatchStatusReq},
                {(int)ERandomWarsMatchProtocol.STOP_MATCH_REQ, HttpReceiveStopMatchReq},

           };
        }


#region Socket Controller 구현부
        // -------------------------------------------------------------------
        // Socket Controller 구현부
        // -------------------------------------------------------------------

        public bool SendJoinStageReq(Peer peer, string playerSessionId, int deckIndex)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerSessionId);
                bw.Write(deckIndex);
                return peer.SendPacket((int)ERandomWarsMatchProtocol.JOIN_STAGE_REQ, ms.ToArray());
            }
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo) JoinStageReqDelegate(Peer peer, string playerSessionId, int deckIndex);
        public JoinStageReqDelegate JoinStageReqCallback;
        public bool ReceiveJoinStageReq(Peer peer, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                var res = JoinStageReqCallback(peer, 
                    br.ReadString(), 
                    br.ReadInt32());

                return SendJoinStageAck(peer, res.errorCode, res.playerInfo);
            }
        }


        public bool SendJoinStageAck(Peer peer, ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)errorCode);
                playerInfo.Write(bw);
                return peer.SendPacket((int)ERandomWarsMatchProtocol.JOIN_STAGE_ACK, ms.ToArray());
            }
        }


        public delegate bool JoinStageAckDelegate(Peer peer, ERandomWarsMatchErrorCode errorCode, MsgPlayerInfo playerInfo);
        public JoinStageAckDelegate JoinStageAckCallback;
        public bool ReceiveJoinStageAck(Peer peer, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);
                return JoinStageAckCallback(peer, 
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
        public bool HttpSendRequestMatchReq(HttpClient client, string playerGuid)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            client.Send((int)ERandomWarsMatchProtocol.REQUEST_MATCH_REQ, "requestmatch", json.ToString());
            return true;
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, string ticketId) HttpReceiveRequestMatchReqDelegate(string playerGuid);
        public HttpReceiveRequestMatchReqDelegate HttpReceiveRequestMatchReqCallback;
        public string HttpReceiveRequestMatchReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveRequestMatchReqCallback(
                (string)jObject["playerGuid"]);

            return HttpSendRequestMatchAck(res.errorCode, res.ticketId);
        }


        public string HttpSendRequestMatchAck(ERandomWarsMatchErrorCode errorCode, string ticketId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("ticketId", ticketId);
            return json.ToString();
        }


        public delegate bool HttpReceiveRequestMatchAckDelegate(ERandomWarsMatchErrorCode errorCode, string ticketId);
        public HttpReceiveRequestMatchAckDelegate HttpReceiveRequestMatchAckCallback;
        public string HttpReceiveRequestMatchAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveRequestMatchAckCallback(
                (ERandomWarsMatchErrorCode)(int)jObject["errorCode"], 
                jObject["ticketId"].ToString());

            return "";
        }
        

        // getmatchstatus
        public bool HttpSendGetMatchStatusReq(HttpClient client, string ticketId)
        {
            JObject json = new JObject();
            json.Add("ticketId", ticketId);
            client.Send((int)ERandomWarsMatchProtocol.GET_MATCH_STATUS_REQ, "getmatchstatus", json.ToString());
            return true;
        }


        public delegate (ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId) HttpReceiveGetMatchStatusReqDelegate(string ticketId);
        public HttpReceiveGetMatchStatusReqDelegate HttpReceiveGetMatchStatusReqCallback;
        public string HttpReceiveGetMatchStatusReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveGetMatchStatusReqCallback(
                (string)jObject["ticketId"]);

            return HttpSendGetMatchStatusAck(res.errorCode, res.serverAddr, res.port, res.playerSessionId);
        }


        public string HttpSendGetMatchStatusAck(ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("serverAddr", serverAddr);
            json.Add("port", port);
            json.Add("playerSessionId", playerSessionId);
            return json.ToString();
        }


        public delegate bool HttpReceiveGetMatchStatusAckDelegate(ERandomWarsMatchErrorCode errorCode, string serverAddr, int port, string playerSessionId);
        public HttpReceiveGetMatchStatusAckDelegate HttpReceiveGetMatchStatusAckCallback;
        public string HttpReceiveGetMatchStatusAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveGetMatchStatusAckCallback(
                (ERandomWarsMatchErrorCode)(int)jObject["errorCode"], 
                jObject["serverAddr"].ToString(),
                (int)jObject["port"],
                jObject["playerSessionId"].ToString());

            return "";
        }

        // stopmatch
        public bool HttpSendStopMatchReq(HttpClient client, string ticketId)
        {
            JObject json = new JObject();
            json.Add("ticketId", ticketId);
            client.Send((int)ERandomWarsMatchProtocol.STOP_MATCH_REQ, "stopmatch", json.ToString());
            return true;
        }


        public delegate ERandomWarsMatchErrorCode HttpReceiveStopMatchReqDelegate(string ticketId);
        public HttpReceiveStopMatchReqDelegate HttpReceiveStopMatchReqCallback;
        public string HttpReceiveStopMatchReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveStopMatchReqCallback(
                (string)jObject["ticketId"]);

            return HttpSendStopMatchAck(res);
        }


        public string HttpSendStopMatchAck(ERandomWarsMatchErrorCode errorCode)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            return json.ToString();
        }


        public delegate bool HttpReceiveStopMatchAckDelegate(ERandomWarsMatchErrorCode errorCode);
        public HttpReceiveStopMatchAckDelegate HttpReceiveStopMatchAckCallback;
        public string HttpReceiveStopMatchAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveStopMatchAckCallback(
                (ERandomWarsMatchErrorCode)(int)jObject["errorCode"]);

            return "";
        }
                
#endregion

    }
}