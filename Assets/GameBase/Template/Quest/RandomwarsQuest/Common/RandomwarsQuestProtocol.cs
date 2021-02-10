using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Quest.RandomwarsQuest.Common
{
    public enum ERandomwarsQuestProtocol
    {
        Begin = 20000,

        QuestInfoReq,
        QuestInfoAck,

        QuestRewardReq,
        QuestRewardAck,

        QuestDailyRewardReq,
        QuestDailyRewardAck,

        End,
    }


    public class RandomwarsQuestProtocol : MessageControllerBase
    {
        public RandomwarsQuestProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsQuestProtocol.QuestInfoReq, ReceiveQuestInfoReq},
                {(int)ERandomwarsQuestProtocol.QuestInfoAck, ReceiveQuestInfoAck},
                {(int)ERandomwarsQuestProtocol.QuestRewardReq, ReceiveQuestRewardReq},
                {(int)ERandomwarsQuestProtocol.QuestRewardAck, ReceiveQuestRewardAck},
                {(int)ERandomwarsQuestProtocol.QuestDailyRewardReq, ReceiveQuestDailyRewardReq},
                {(int)ERandomwarsQuestProtocol.QuestDailyRewardAck, ReceiveQuestDailyRewardAck},
           };
        }


        #region QuestInfo ---------------------------------------------------------------------
        public bool QuestInfoReq(ISender sender, ReceiveQuestInfoAckDelegate callback)
        {
            ReceiveQuestInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsQuestProtocol.QuestInfoReq, "questinfo", json.ToString());
        }

        public delegate (ERandomwarsQuestErrorCode errorCode, MsgQuestInfo questInfo) ReceiveQuestInfoReqDelegate(string accessToken);
        public ReceiveQuestInfoReqDelegate ReceiveQuestInfoReqHandler;
        public bool ReceiveQuestInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveQuestInfoReqHandler(accessToken);
            return QuestInfoAck(sender, res.errorCode, res.questInfo);
        }

        public bool QuestInfoAck(ISender sender, ERandomwarsQuestErrorCode errorCode, MsgQuestInfo questInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("questInfo", JsonConvert.SerializeObject(questInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveQuestInfoAckDelegate(ERandomwarsQuestErrorCode errorCode, MsgQuestInfo questInfo);
        public ReceiveQuestInfoAckDelegate ReceiveQuestInfoAckHandler;
        public bool ReceiveQuestInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsQuestErrorCode errorCode = (ERandomwarsQuestErrorCode)(int)jObject["errorCode"];
            MsgQuestInfo questInfo = JsonConvert.DeserializeObject<MsgQuestInfo>(jObject["questInfo"].ToString());
            return ReceiveQuestInfoAckHandler(errorCode, questInfo);
        }
        #endregion    

        #region QuestReward ---------------------------------------------------------------------
        public bool QuestRewardReq(ISender sender, int questId, ReceiveQuestRewardAckDelegate callback)
        {
            ReceiveQuestRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("questId", questId);
            return sender.SendHttpPost((int)ERandomwarsQuestProtocol.QuestRewardReq, "questreward", json.ToString());
        }

        public delegate (ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo) ReceiveQuestRewardReqDelegate(string accessToken, int questId);
        public ReceiveQuestRewardReqDelegate ReceiveQuestRewardReqHandler;
        public bool ReceiveQuestRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int questId = (int)jObject["questId"];
            var res = ReceiveQuestRewardReqHandler(accessToken, questId);
            return QuestRewardAck(sender, res.errorCode, res.arrayQuestData, res.arrayRewardInfo);
        }

        public bool QuestRewardAck(ISender sender, ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveQuestRewardAckDelegate(ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo);
        public ReceiveQuestRewardAckDelegate ReceiveQuestRewardAckHandler;
        public bool ReceiveQuestRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsQuestErrorCode errorCode = (ERandomwarsQuestErrorCode)(int)jObject["errorCode"];
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            MsgReward[] arrayRewardInfo = JsonConvert.DeserializeObject<MsgReward[]>(jObject["arrayRewardInfo"].ToString());
            return ReceiveQuestRewardAckHandler(errorCode, arrayQuestData, arrayRewardInfo);
        }
        #endregion      

        #region QuestDailyReward ---------------------------------------------------------------------
        public bool QuestDailyRewardReq(ISender sender, int rewardId, int index, ReceiveQuestDailyRewardAckDelegate callback)
        {
            ReceiveQuestDailyRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            json.Add("index", index);
            return sender.SendHttpPost((int)ERandomwarsQuestProtocol.QuestDailyRewardReq, "questdailyreward", json.ToString());
        }

        public delegate (ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo, MsgQuestDayReward dailyRewardInfo) ReceiveQuestDailyRewardReqDelegate(string accessToken, int rewardId, int index);
        public ReceiveQuestDailyRewardReqDelegate ReceiveQuestDailyRewardReqHandler;
        public bool ReceiveQuestDailyRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            int index = (int)jObject["index"];
            var res = ReceiveQuestDailyRewardReqHandler(accessToken, rewardId, index);
            return QuestDailyRewardAck(sender, res.errorCode, res.arrayQuestData, res.arrayRewardInfo, res.dailyRewardInfo);
        }

        public bool QuestDailyRewardAck(ISender sender, ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo, MsgQuestDayReward dailyRewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("dailyRewardInfo", JsonConvert.SerializeObject(dailyRewardInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveQuestDailyRewardAckDelegate(ERandomwarsQuestErrorCode errorCode, MsgQuestData[] arrayQuestData, MsgReward[] arrayRewardInfo, MsgQuestDayReward dailyRewardInfo);
        public ReceiveQuestDailyRewardAckDelegate ReceiveQuestDailyRewardAckHandler;
        public bool ReceiveQuestDailyRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsQuestErrorCode errorCode = (ERandomwarsQuestErrorCode)(int)jObject["errorCode"];
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            MsgReward[] arrayRewardInfo = JsonConvert.DeserializeObject<MsgReward[]>(jObject["arrayRewardInfo"].ToString());
            MsgQuestDayReward dailyRewardInfo = JsonConvert.DeserializeObject<MsgQuestDayReward>(jObject["dailyRewardInfo"].ToString());
            return ReceiveQuestDailyRewardAckHandler(errorCode, arrayQuestData, arrayRewardInfo, dailyRewardInfo);
        }
        #endregion  
    }
}