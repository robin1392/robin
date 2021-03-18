using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Season.RandomwarsSeason.Common
{
    public enum ERandomwarsSeasonProtocol
    {
        Begin = ETemplateType.Season * 10000,

        SeasonInfoReq,
        SeasonInfoAck,

        SeasonResetReq,
        SeasonResetAck,
        
        SeasonRankReq,
        SeasonRankAck,

        SeasonPassRewardReq,
        SeasonPassRewardAck,

        SeasonPassStepReq,
        SeasonPassStepAck,

        End,
    }


    public class RandomwarsSeasonProtocol : MessageControllerBase
    {
        public RandomwarsSeasonProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsSeasonProtocol.SeasonInfoReq, ReceiveSeasonInfoReq},
                {(int)ERandomwarsSeasonProtocol.SeasonInfoAck, ReceiveSeasonInfoAck},
                {(int)ERandomwarsSeasonProtocol.SeasonResetReq, ReceiveSeasonResetReq},
                {(int)ERandomwarsSeasonProtocol.SeasonResetAck, ReceiveSeasonResetAck},
                {(int)ERandomwarsSeasonProtocol.SeasonRankReq, ReceiveSeasonRankReq},
                {(int)ERandomwarsSeasonProtocol.SeasonRankAck, ReceiveSeasonRankAck},
                {(int)ERandomwarsSeasonProtocol.SeasonPassRewardReq, ReceiveSeasonPassRewardReq},
                {(int)ERandomwarsSeasonProtocol.SeasonPassRewardAck, ReceiveSeasonPassRewardAck},
                {(int)ERandomwarsSeasonProtocol.SeasonPassStepReq, ReceiveSeasonPassStepReq},
                {(int)ERandomwarsSeasonProtocol.SeasonPassStepAck, ReceiveSeasonPassStepAck},
            };         
        }


        #region SeasonInfo ---------------------------------------------------------------------
        public bool SeasonInfoReq(ISender sender, ReceiveSeasonInfoAckDelegate callback)
        {
            ReceiveSeasonInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsSeasonProtocol.SeasonInfoReq, "seasoninfo", json.ToString());
        }


        public delegate (ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, MsgRankInfo[] arrayRankInfo) ReceiveSeasonInfoReqDelegate(string accessToken);
        public ReceiveSeasonInfoReqDelegate ReceiveSeasonInfoReqHandler;
        public bool ReceiveSeasonInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveSeasonInfoReqHandler(accessToken);
            return SeasonInfoAck(sender, res.errorCode, res.seasonInfo, res.arrayRankInfo);     
        }        

        public bool SeasonInfoAck(ISender sender, ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, MsgRankInfo[] arrayRankInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("seasonInfo", JsonConvert.SerializeObject(seasonInfo));
            json.Add("arrayRankInfo", JsonConvert.SerializeObject(arrayRankInfo));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveSeasonInfoAckDelegate(ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, MsgRankInfo[] arrayRankInfo);
        public ReceiveSeasonInfoAckDelegate ReceiveSeasonInfoAckHandler;
        public bool ReceiveSeasonInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsSeasonErrorCode errorCode = (ERandomwarsSeasonErrorCode)(int)jObject["errorCode"];
            UserSeasonInfo seasonInfo = JsonConvert.DeserializeObject<UserSeasonInfo>(jObject["seasonInfo"].ToString());
            MsgRankInfo[] arrayRankInfo = JsonConvert.DeserializeObject<MsgRankInfo[]>(jObject["arrayRankInfo"].ToString());
            return ReceiveSeasonInfoAckHandler(errorCode, seasonInfo, arrayRankInfo);
        }
        #endregion    


        #region SeasonReset ---------------------------------------------------------------------
        public bool SeasonResetReq(ISender sender, ReceiveSeasonResetAckDelegate callback)
        {
            ReceiveSeasonResetAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsSeasonProtocol.SeasonResetReq, "seasonreset", json.ToString());
        }


        public delegate (ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, ItemBaseInfo[] arrayRewardInfo) ReceiveSeasonResetReqDelegate(string accessToken);
        public ReceiveSeasonResetReqDelegate ReceiveSeasonResetReqHandler;
        public bool ReceiveSeasonResetReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveSeasonResetReqHandler(accessToken);
            return SeasonResetAck(sender, res.errorCode, res.seasonInfo, res.arrayRewardInfo);     
        }        

        public bool SeasonResetAck(ISender sender, ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, ItemBaseInfo[] arrayRewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("seasonInfo", JsonConvert.SerializeObject(seasonInfo));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveSeasonResetAckDelegate(ERandomwarsSeasonErrorCode errorCode, UserSeasonInfo seasonInfo, ItemBaseInfo[] arrayRewardInfo);
        public ReceiveSeasonResetAckDelegate ReceiveSeasonResetAckHandler;
        public bool ReceiveSeasonResetAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsSeasonErrorCode errorCode = (ERandomwarsSeasonErrorCode)(int)jObject["errorCode"];
            UserSeasonInfo seasonInfo = JsonConvert.DeserializeObject<UserSeasonInfo>(jObject["seasonInfo"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            return ReceiveSeasonResetAckHandler(errorCode, seasonInfo, arrayRewardInfo);
        }
        #endregion       


        #region SeasonRank ---------------------------------------------------------------------
        public bool SeasonRankReq(ISender sender, int pageNo, ReceiveSeasonRankAckDelegate callback)
        {
            ReceiveSeasonRankAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("pageNo", pageNo);
            return sender.SendHttpPost((int)ERandomwarsSeasonProtocol.SeasonRankReq, "seasonrank", json.ToString());
        }


        public delegate (ERandomwarsSeasonErrorCode errorCode, int pageNo, MsgRankInfo[] arrayRankInfo) ReceiveSeasonRankReqDelegate(string accessToken, int pageNo);
        public ReceiveSeasonRankReqDelegate ReceiveSeasonRankReqHandler;
        public bool ReceiveSeasonRankReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int pageNo = (int)jObject["pageNo"];
            var res = ReceiveSeasonRankReqHandler(accessToken, pageNo);
            return SeasonRankAck(sender, res.errorCode, res.pageNo, res.arrayRankInfo);     
        }        

        public bool SeasonRankAck(ISender sender, ERandomwarsSeasonErrorCode errorCode, int pageNo, MsgRankInfo[] arrayRankInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("pageNo", pageNo);
            json.Add("arrayRankInfo", JsonConvert.SerializeObject(arrayRankInfo));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveSeasonRankAckDelegate(ERandomwarsSeasonErrorCode errorCode, int pageNo, MsgRankInfo[] arrayRankInfo);
        public ReceiveSeasonRankAckDelegate ReceiveSeasonRankAckHandler;
        public bool ReceiveSeasonRankAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsSeasonErrorCode errorCode = (ERandomwarsSeasonErrorCode)(int)jObject["errorCode"];
            int pageNo = (int)jObject["pageNo"];
            MsgRankInfo[] arrayRankInfo = JsonConvert.DeserializeObject<MsgRankInfo[]>(jObject["arrayRankInfo"].ToString());
            return ReceiveSeasonRankAckHandler(errorCode, pageNo, arrayRankInfo);
        }
        #endregion            


        #region SeasonPassReward ---------------------------------------------------------------------
        public bool SeasonPassRewardReq(ISender sender, int rewardId, int targetType, ReceiveSeasonPassRewardAckDelegate callback)
        {
            ReceiveSeasonPassRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            json.Add("targetType", targetType);
            return sender.SendHttpPost((int)ERandomwarsSeasonProtocol.SeasonPassRewardReq, "seasonpassreward", json.ToString());
        }


        public delegate (ERandomwarsSeasonErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveSeasonPassRewardReqDelegate(string accessToken, int rewardId, int targetType);
        public ReceiveSeasonPassRewardReqDelegate ReceiveSeasonPassRewardReqHandler;
        public bool ReceiveSeasonPassRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            int targetType = (int)jObject["targetType"];
            var res = ReceiveSeasonPassRewardReqHandler(accessToken, rewardId, targetType);
            return SeasonPassRewardAck(sender, res.errorCode, res.arrayRewardId, res.arrayRewardInfo, res.arrayQuestData);     
        }        

        public bool SeasonPassRewardAck(ISender sender, ERandomwarsSeasonErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayRewardId", JsonConvert.SerializeObject(arrayRewardId));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveSeasonPassRewardAckDelegate(ERandomwarsSeasonErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveSeasonPassRewardAckDelegate ReceiveSeasonPassRewardAckHandler;
        public bool ReceiveSeasonPassRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsSeasonErrorCode errorCode = (ERandomwarsSeasonErrorCode)(int)jObject["errorCode"];
            int[] arrayRewardId = JsonConvert.DeserializeObject<int[]>(jObject["arrayRewardId"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveSeasonPassRewardAckHandler(errorCode, arrayRewardId, arrayRewardInfo, arrayQuestData);
        }
        #endregion                      


        #region SeasonPassStep ---------------------------------------------------------------------
        public bool SeasonPassStepReq(ISender sender, int rewardId, ReceiveSeasonPassStepAckDelegate callback)
        {
            ReceiveSeasonPassStepAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            return sender.SendHttpPost((int)ERandomwarsSeasonProtocol.SeasonPassStepReq, "seasonpassstep", json.ToString());
        }


        public delegate (ERandomwarsSeasonErrorCode errorCode, int rewardId, ItemBaseInfo useItemInfo, ItemBaseInfo rewardInfo, QuestData[] arrayQuestData) ReceiveSeasonPassStepReqDelegate(string accessToken, int rewardId);
        public ReceiveSeasonPassStepReqDelegate ReceiveSeasonPassStepReqHandler;
        public bool ReceiveSeasonPassStepReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            var res = ReceiveSeasonPassStepReqHandler(accessToken, rewardId);
            return SeasonPassStepAck(sender, res.errorCode, res.rewardId, res.useItemInfo, res.rewardInfo, res.arrayQuestData);     
        }        

        public bool SeasonPassStepAck(ISender sender, ERandomwarsSeasonErrorCode errorCode, int rewardId, ItemBaseInfo useItemInfo, ItemBaseInfo rewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("rewardId", rewardId);
            json.Add("useItemInfo", JsonConvert.SerializeObject(useItemInfo));
            json.Add("rewardInfo", JsonConvert.SerializeObject(rewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveSeasonPassStepAckDelegate(ERandomwarsSeasonErrorCode errorCode, int rewardId, ItemBaseInfo useItemInfo, ItemBaseInfo rewardInfo, QuestData[] arrayQuestData);
        public ReceiveSeasonPassStepAckDelegate ReceiveSeasonPassStepAckHandler;
        public bool ReceiveSeasonPassStepAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsSeasonErrorCode errorCode = (ERandomwarsSeasonErrorCode)(int)jObject["errorCode"];
            int rewardId = (int)jObject["rewardId"]; 
            ItemBaseInfo useItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo>(jObject["useItemInfo"].ToString());
            ItemBaseInfo rewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo>(jObject["rewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveSeasonPassStepAckHandler(errorCode, rewardId, useItemInfo, rewardInfo, arrayQuestData);
        }
        #endregion                              
    }
}