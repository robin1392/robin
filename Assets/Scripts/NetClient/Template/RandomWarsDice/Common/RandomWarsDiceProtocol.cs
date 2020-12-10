using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Item.RandomWarsDice.Common
{
    public enum ERandomWarsDiceProtocol
    {
        BEGIN = 3000000,
        
        UPDATE_DECK_REQ,
        UPDATE_DECK_ACK,
        UPDATE_DECK_NOTIFY,

        LEVELUP_DICE_REQ,
        LEVELUP_DICE_ACK,
        LEVELUP_DICE_NOTIFY,

        OPEN_BOX_REQ,
        OPEN_BOX_ACK,
        OPEN_BOX_NOTIFY,


        END
    }

    public class RandomWarsDiceProtocol : BaseProtocol
    {
        public RandomWarsDiceProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
            };

            HttpMessageControllers = new Dictionary<int, HttpControllerDelegate>
            {
                {(int)ERandomWarsDiceProtocol.UPDATE_DECK_REQ, HttpReceiveUpdateDeckReq},
                {(int)ERandomWarsDiceProtocol.UPDATE_DECK_ACK, HttpReceiveUpdateDeckAck},
                {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, HttpReceiveLevelupDiceReq},
                {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_ACK, HttpReceiveLevelupDiceAck},
                {(int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, HttpReceiveOpenBoxReq},
                {(int)ERandomWarsDiceProtocol.OPEN_BOX_ACK, HttpReceiveOpenBoxAck},
          };            
        }

#region Http Controller 구현부        
        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------

        // updatedeck
        public bool HttpSendUpdateDeckReq(HttpClient client, string playerGuid, int deckIndex, int[] deckInfo)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("deckIndex", deckIndex);
            json.Add("deckInfo", JsonConvert.SerializeObject(deckInfo));
            client.Send((int)ERandomWarsDiceProtocol.UPDATE_DECK_REQ, "dice/updatedeck", json.ToString());
            return true;
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo) HttpReceiveUpdateDeckReqDelegate(string playerGuid, int deckIndex, int[] deckInfo);
        public HttpReceiveUpdateDeckReqDelegate HttpReceiveUpdateDeckReqCallback;
        public string HttpReceiveUpdateDeckReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveUpdateDeckReqCallback(
                jObject["playerGuid"].ToString(),
                (int)jObject["deckIndex"],
                JsonConvert.DeserializeObject<int[]>(jObject["deckInfo"].ToString()));

            return HttpSendUpdateDeckAck(res.errorCode, res.deckIndex, res.deckInfo);
        }


        public string HttpSendUpdateDeckAck(ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("deckIndex", deckIndex);
            json.Add("deckInfo", JsonConvert.SerializeObject(deckInfo));
            return json.ToString();
        }


        public delegate bool HttpReceiveUpdateDeckAckDelegate(ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo);
        public HttpReceiveUpdateDeckAckDelegate HttpReceiveUpdateDeckAckCallback;
        public string HttpReceiveUpdateDeckAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveUpdateDeckAckCallback(
                (ERandomWarsDiceErrorCode)(int)jObject["errorCode"], 
                (int)jObject["deckIndex"],
                JsonConvert.DeserializeObject<int[]>(jObject["deckInfo"].ToString()));

            return "";
        }
        

        // levelupdice
        public bool HttpSendLevelupDiceReq(HttpClient client, string playerGuid, int diceId)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("diceId", diceId);
            client.Send((int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, "dice/levelupdice", json.ToString());
            return true;
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold) HttpReceiveLevelupDiceReqDelegate(string playerGuid, int diceId);
        public HttpReceiveLevelupDiceReqDelegate HttpReceiveLevelupDiceReqCallback;
        public string HttpReceiveLevelupDiceReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveLevelupDiceReqCallback(
                jObject["playerGuid"].ToString(),
                (int)jObject["diceId"]);

            return HttpSendLevelupDiceAck(res.errorCode, res.diceId, res.level, res.count, res.gold);
        }


        public string HttpSendLevelupDiceAck(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("diceId", diceId);
            json.Add("level", level);
            json.Add("count", count);
            json.Add("gold", gold);
            return json.ToString();
        }


        public delegate bool HttpReceiveLevelupDiceAckDelegate(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold);
        public HttpReceiveLevelupDiceAckDelegate HttpReceiveLevelupDiceAckCallback;
        public string HttpReceiveLevelupDiceAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveLevelupDiceAckCallback(
                (ERandomWarsDiceErrorCode)(int)jObject["errorCode"], 
                (int)jObject["diceId"],
                (short)jObject["level"],
                (short)jObject["count"],
                (int)jObject["gold"]);

            return "";
        }

        // openBox
        public bool HttpSendOpenBoxReq(HttpClient client, string playerGuid, int boxId)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("boxId", boxId);
            client.Send((int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, "dice/openBox", json.ToString());
            return true;
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo) HttpReceiveOpenBoxReqDelegate(string playerGuid, int boxId);
        public HttpReceiveOpenBoxReqDelegate HttpReceiveOpenBoxReqCallback;
        public string HttpReceiveOpenBoxReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveOpenBoxReqCallback(
                jObject["playerGuid"].ToString(),
                (int)jObject["boxId"]);

            return HttpSendOpenBoxAck(res.errorCode, res.rewardInfo);
        }


        public string HttpSendOpenBoxAck(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("rewardInfo", JsonConvert.SerializeObject(rewardInfo));
            return json.ToString();
        }


        public delegate bool HttpReceiveOpenBoxAckDelegate(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo);
        public HttpReceiveOpenBoxAckDelegate HttpReceiveOpenBoxAckCallback;
        public string HttpReceiveOpenBoxAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveOpenBoxAckCallback(
                (ERandomWarsDiceErrorCode)(int)jObject["errorCode"], 
                JsonConvert.DeserializeObject<MsgOpenBoxReward[]>(jObject["rewardInfo"].ToString()));

            return "";
        }
#endregion

    }
}