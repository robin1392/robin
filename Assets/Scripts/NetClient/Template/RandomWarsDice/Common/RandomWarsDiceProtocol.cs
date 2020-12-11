using System;
using System.Collections.Generic;
using System.IO;
using Service.Net;

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
                // {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, HttpReceiveLevelupDiceReq},
                // {(int)ERandomWarsDiceProtocol.LEVELUP_DICE_ACK, HttpReceiveUpdateDeckAck},
                // {(int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, HttpReceiveOpenBoxReq},
            };
        }

#region Http Controller 구현부        
        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------

        // updatedeck
        public bool SendUpdateDeckReq(ISender sender, string playerGuid, int deckIndex, int[] deckInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(playerGuid);
                bw.Write(deckIndex);

                int length = (deckInfo == null) ? 0 : deckInfo.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    bw.Write(deckInfo[i]);
                }

                return sender.SendMessage((int)ERandomWarsDiceProtocol.UPDATE_DECK_REQ, "dice/updatedeck", ms.ToArray());
            }
        }


        public delegate (ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo) ReceiveUpdateDeckReqDelegate(string playerGuid, int deckIndex, int[] deckInfo);
        public ReceiveUpdateDeckReqDelegate ReceiveUpdateDeckReqCallback;
        public bool ReceiveUpdateDeckReq(ISender sender, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);

                string playerGuid = br.ReadString();
                int deckIndex = br.ReadInt32();

                int length = br.ReadInt32();
                int[] deckInfo = new int[length];
                for (int i = 0; i < length; i++)
                {
                    deckInfo[i] = br.ReadInt32();
                }

                var res = ReceiveUpdateDeckReqCallback(playerGuid, deckIndex, deckInfo);
                return SendUpdateDeckAck(sender, res.errorCode, res.deckIndex, res.deckInfo);
            }
        }


        public bool SendUpdateDeckAck(ISender sender, ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo)
        {
            using (var ms = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write((int)errorCode);
                bw.Write(deckIndex);

                int length = (deckInfo == null) ? 0 : deckInfo.Length;
                bw.Write(length);
                for (int i = 0; i < length; i++)
                {
                    bw.Write(deckInfo[i]);
                }

                return sender.SendMessage((int)ERandomWarsDiceProtocol.UPDATE_DECK_ACK, ms.ToArray());
            }
        }


        public delegate bool ReceiveUpdateDeckAckDelegate(ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo);
        public ReceiveUpdateDeckAckDelegate ReceiveUpdateDeckAckCallback;
        public bool ReceiveUpdateDeckAck(ISender sender, byte[] msg)
        {
            using (var ms = new MemoryStream(msg))
            {
                BinaryReader br = new BinaryReader(ms);

                ERandomWarsDiceErrorCode errorCode = (ERandomWarsDiceErrorCode)br.ReadInt32();
                int deckIndex = br.ReadInt32();

                int length = br.ReadInt32();
                int[] deckInfo = new int[length];
                for (int i = 0; i < length; i++)
                {
                    deckInfo[i] = br.ReadInt32();
                }

                return ReceiveUpdateDeckAckCallback(errorCode, deckIndex, deckInfo);
            }
        }
        

        // levelupdice
        // public bool HttpSendLevelupDiceReq(HttpClient client, string playerGuid, int diceId)
        // {
        //     JObject json = new JObject();
        //     json.Add("playerGuid", playerGuid);
        //     json.Add("diceId", diceId);
        //     client.Send((int)ERandomWarsDiceProtocol.LEVELUP_DICE_REQ, "item/levelupdice", json.ToString());
        //     return true;
        // }


        // public delegate (ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold) HttpReceiveLevelupDiceReqDelegate(string playerGuid, int diceId);
        // public HttpReceiveLevelupDiceReqDelegate HttpReceiveLevelupDiceReqCallback;
        // public string HttpReceiveLevelupDiceReq(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     var res = HttpReceiveLevelupDiceReqCallback(
        //         jObject["playerGuid"].ToString(),
        //         (int)jObject["diceId"]);

        //     return HttpSendLevelupDiceAck(res.errorCode, res.diceId, res.level, res.count, res.gold);
        // }


        // public string HttpSendLevelupDiceAck(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold)
        // {
        //     JObject json = new JObject();
        //     json.Add("errorCode", (int)errorCode);
        //     json.Add("diceId", diceId);
        //     json.Add("level", level);
        //     json.Add("count", count);
        //     json.Add("gold", gold);
        //     return json.ToString();
        // }


        // public delegate bool HttpReceiveLevelupDiceAckDelegate(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold);
        // public HttpReceiveLevelupDiceAckDelegate HttpReceiveLevelupDiceAckCallback;
        // public string HttpReceiveLevelupDiceAck(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     HttpReceiveLevelupDiceAckCallback(
        //         (ERandomWarsDiceErrorCode)(int)jObject["errorCode"], 
        //         (int)jObject["diceId"],
        //         (short)jObject["level"],
        //         (short)jObject["count"],
        //         (int)jObject["gold"]);

        //     return "";
        // }

        // // openBox
        // public bool HttpSendOpenBoxReq(HttpClient client, string playerGuid, int boxId)
        // {
        //     JObject json = new JObject();
        //     json.Add("playerGuid", playerGuid);
        //     json.Add("boxId", boxId);
        //     client.Send((int)ERandomWarsDiceProtocol.OPEN_BOX_REQ, "player/openBox", json.ToString());
        //     return true;
        // }


        // public delegate (ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo) HttpReceiveOpenBoxReqDelegate(string playerGuid, int boxId);
        // public HttpReceiveOpenBoxReqDelegate HttpReceiveOpenBoxReqCallback;
        // public string HttpReceiveOpenBoxReq(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     var res = HttpReceiveOpenBoxReqCallback(
        //         jObject["playerGuid"].ToString(),
        //         (int)jObject["boxId"]);

        //     return HttpSendOpenBoxAck(res.errorCode, res.rewardInfo);
        // }


        // public string HttpSendOpenBoxAck(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo)
        // {
        //     JObject json = new JObject();
        //     json.Add("errorCode", (int)errorCode);
        //     json.Add("rewardInfo", JsonConvert.SerializeObject(rewardInfo));
        //     return json.ToString();
        // }


        // public delegate bool HttpReceiveOpenBoxAckDelegate(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo);
        // public HttpReceiveOpenBoxAckDelegate HttpReceiveOpenBoxAckCallback;
        // public string HttpReceiveOpenBoxAck(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     HttpReceiveOpenBoxAckCallback(
        //         (ERandomWarsDiceErrorCode)(int)jObject["errorCode"], 
        //         JsonConvert.DeserializeObject<MsgOpenBoxReward[]>(jObject["rewardInfo"].ToString()));

        //     return "";
        // }
#endregion

    }
}