using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public class RandomWarsDiceProtocol : MessageControllerBase
    {
        public RandomWarsDiceProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomWarsDiceProtocol.UPDATE_DECK_REQ, ReceiveUpdateDeckReq},
                {(int)ERandomWarsDiceProtocol.UPDATE_DECK_ACK, ReceiveUpdateDeckAck},
                {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, ReceiveLevelupDiceReq},
                {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_ACK, ReceiveLevelupDiceAck},
                {(int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, ReceiveOpenBoxReq},
                {(int)ERandomWarsDiceProtocol.OPEN_BOX_ACK, ReceiveOpenBoxAck},
            };
        }

#region Http Controller 구현부        
        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------

        // updatedeck
        public bool SendUpdateDeckReq(ISender sender, string playerGuid, int deckIndex, int[] deckInfo)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("deckIndex", deckIndex);
            json.Add("deckInfo", JsonConvert.SerializeObject(deckInfo));
            return sender.SendHttpPost((int)ERandomWarsDiceProtocol.UPDATE_DECK_REQ, "updatedeck", json.ToString());
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo) ReceiveUpdateDeckReqDelegate(string playerGuid, int deckIndex, int[] deckInfo);
        public ReceiveUpdateDeckReqDelegate ReceiveUpdateDeckReqCallback;
        public bool ReceiveUpdateDeckReq(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int deckIndex = (int)jObject["deckIndex"];
            int[] deckInfo = JsonConvert.DeserializeObject<int[]>(jObject["deckInfo"].ToString());
            var res = ReceiveUpdateDeckReqCallback(playerGuid, deckIndex, deckInfo);
            return SendUpdateDeckAck(sender, res.errorCode, res.deckIndex, res.deckInfo);     
        }


        public bool SendUpdateDeckAck(ISender sender, ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("deckIndex", deckIndex);
            json.Add("deckInfo", JsonConvert.SerializeObject(deckInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUpdateDeckAckDelegate(ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo);
        public ReceiveUpdateDeckAckDelegate ReceiveUpdateDeckAckCallback;
        public bool ReceiveUpdateDeckAck(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            ERandomWarsDiceErrorCode errorCode = (ERandomWarsDiceErrorCode)(int)jObject["errorCode"];
            int deckIndex = (int)jObject["deckIndex"];
            int[] deckInfo = JsonConvert.DeserializeObject<int[]>(jObject["deckInfo"].ToString());
            return ReceiveUpdateDeckAckCallback(errorCode, deckIndex, deckInfo);
        }
        

        // levelupdice
        public bool SendLevelupDiceReq(ISender sender, string playerGuid, int diceId)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("diceId", diceId);
            sender.SendHttpPost((int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, "levelupdice", json.ToString());
            return true;
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold) ReceiveLevelupDiceReqDelegate(string playerGuid, int diceId);
        public ReceiveLevelupDiceReqDelegate ReceiveLevelupDiceReqCallback;
        public bool ReceiveLevelupDiceReq(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int diceId = (int)jObject["diceId"];
            var res = ReceiveLevelupDiceReqCallback(playerGuid, diceId);
            return SendLevelupDiceAck(sender, res.errorCode, res.diceId, res.level, res.count, res.gold);
        }


        public bool SendLevelupDiceAck(ISender sender, ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("diceId", diceId);
            json.Add("level", level);
            json.Add("count", count);
            json.Add("gold", gold);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveLevelupDiceAckDelegate(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold);
        public ReceiveLevelupDiceAckDelegate ReceiveLevelupDiceAckCallback;
        public bool ReceiveLevelupDiceAck(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            ERandomWarsDiceErrorCode errorCode = (ERandomWarsDiceErrorCode)(int)jObject["errorCode"];
            int diceId = (int)jObject["diceId"];
            short level = (short)jObject["level"];
            short count = (short)jObject["count"];
            int gold = (int)jObject["gold"];
            return ReceiveLevelupDiceAckCallback(errorCode, diceId, level, count, gold);
        }


        // openBox
        public bool SendOpenBoxReq(ISender sender, string playerGuid, int boxId)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("boxId", boxId);
            sender.SendHttpPost((int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, "openBox", json.ToString());
            return true;
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo) ReceiveOpenBoxReqDelegate(string playerGuid, int boxId);
        public ReceiveOpenBoxReqDelegate ReceiveOpenBoxReqCallback;
        public bool ReceiveOpenBoxReq(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int boxId = (int)jObject["boxId"];
            var res = ReceiveOpenBoxReqCallback(playerGuid, boxId);
            return SendOpenBoxAck(sender, res.errorCode, res.rewardInfo);
        }


        public bool SendOpenBoxAck(ISender sender, ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("rewardInfo", JsonConvert.SerializeObject(rewardInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveOpenBoxAckDelegate(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo);
        public ReceiveOpenBoxAckDelegate ReceiveOpenBoxAckCallback;
        public bool ReceiveOpenBoxAck(ISender sender, byte[] msg, int lenght)
        {
            string json = Encoding.Default.GetString(msg, 0, lenght);
            JObject jObject = JObject.Parse(json);
            ERandomWarsDiceErrorCode errorCode = (ERandomWarsDiceErrorCode)(int)jObject["errorCode"];
            MsgOpenBoxReward[] rewardInfo = JsonConvert.DeserializeObject<MsgOpenBoxReward[]>(jObject["rewardInfo"].ToString());
            return ReceiveOpenBoxAckCallback(errorCode, rewardInfo);
        }
#endregion

    }
}