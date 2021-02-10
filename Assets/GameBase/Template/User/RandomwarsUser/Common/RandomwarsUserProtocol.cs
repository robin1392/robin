using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.User.RandomwarsUser.Common
{
    public enum ERandomwarsProtocol
    {
        Begin = 20000,

        UserInfoReq,
        UserInfoAck,

        UserTrophyRewardReq,
        UserTrophyRewardAck,

        End,
    }


    public class RandomwarsUserProtocol : MessageControllerBase
    {
        public RandomwarsUserProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsProtocol.UserInfoReq, ReceiveUserInfoReq},
                {(int)ERandomwarsProtocol.UserInfoAck, ReceiveUserInfoAck},
                {(int)ERandomwarsProtocol.UserTrophyRewardReq, ReceiveUserTrophyRewardReq},
                {(int)ERandomwarsProtocol.UserTrophyRewardAck, ReceiveUserTrophyRewardAck},
           };
        }


        #region UserInfo ---------------------------------------------------------------------
        public bool UserInfoReq(ISender sender, ReceiveUserInfoAckDelegate callback)
        {
            ReceiveUserInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserInfoReq, "userinfo", json.ToString());
        }

        public delegate (ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, MsgUserDeck[] arrayUserDeck, MsgUserDice[] arrayUserDice, MsgUserBox[] arrayUserBox, MsgQuestInfo questInfo, MsgSeasonInfo seasonInfo) ReceiveUserInfoReqDelegate(string accessToken);
        public ReceiveUserInfoReqDelegate ReceiveUserInfoReqHandler;
        public bool ReceiveUserInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveUserInfoReqHandler(accessToken);
            return UserInfoAck(sender, res.errorCode, res.userInfo, res.arrayUserDeck, res.arrayUserDice, res.arrayUserBox, res.questInfo, res.seasonInfo);
        }

        public bool UserInfoAck(ISender sender, ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, MsgUserDeck[] arrayUserDeck, MsgUserDice[] arrayUserDice, MsgUserBox[] arrayUserBox, MsgQuestInfo questInfo, MsgSeasonInfo seasonInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("userInfo", JsonConvert.SerializeObject(userInfo));
            json.Add("arrayUserDeck", JsonConvert.SerializeObject(arrayUserDeck));
            json.Add("arrayUserDice", JsonConvert.SerializeObject(arrayUserDice));
            json.Add("arrayUserBox", JsonConvert.SerializeObject(arrayUserBox));
            json.Add("questInfo", JsonConvert.SerializeObject(questInfo));
            json.Add("seasonInfo", JsonConvert.SerializeObject(seasonInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserInfoAckDelegate(ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, MsgUserDeck[] arrayUserDeck, MsgUserDice[] arrayUserDice, MsgUserBox[] arrayUserBox, MsgQuestInfo questInfo, MsgSeasonInfo seasonInfo);
        public ReceiveUserInfoAckDelegate ReceiveUserInfoAckHandler;
        public bool ReceiveUserInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            MsgUserInfo userInfo = JsonConvert.DeserializeObject<MsgUserInfo>(jObject["userInfo"].ToString());
            MsgUserDeck[] arrayUserDeck = JsonConvert.DeserializeObject<MsgUserDeck[]>(jObject["arrayUserDeck"].ToString());
            MsgUserDice[] arrayUserDice = JsonConvert.DeserializeObject<MsgUserDice[]>(jObject["arrayUserDice"].ToString());
            MsgUserBox[] arrayUserBox = JsonConvert.DeserializeObject<MsgUserBox[]>(jObject["arrayUserBox"].ToString());
            MsgQuestInfo questInfo = JsonConvert.DeserializeObject<MsgQuestInfo>(jObject["questInfo"].ToString());
            MsgSeasonInfo seasonInfo = JsonConvert.DeserializeObject<MsgSeasonInfo>(jObject["seasonInfo"].ToString());
            return ReceiveUserInfoAckHandler(errorCode, userInfo, arrayUserDeck, arrayUserDice, arrayUserBox, questInfo, seasonInfo);
        }
        #endregion    


        #region UserTrophyReward ---------------------------------------------------------------------
        public bool UserTrophyRewardReq(ISender sender, int rewardId, int targetType, ReceiveUserTrophyRewardAckDelegate callback)
        {
            ReceiveUserTrophyRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            json.Add("targetType", targetType);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserTrophyRewardReq, "usertrophyreward", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData) ReceiveUserTrophyRewardReqDelegate(string accessToken, int rewardId, int targetType);
        public ReceiveUserTrophyRewardReqDelegate ReceiveUserTrophyRewardReqHandler;
        public bool ReceiveUserTrophyRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            int targetType = (int)jObject["targetType"];
            var res = ReceiveUserTrophyRewardReqHandler(accessToken, rewardId, targetType);
            return UserTrophyRewardAck(sender, res.errorCode, res.arrayRewardId, res.arrayRewardInfo, res.arrayQuestData);
        }

        public bool UserTrophyRewardAck(ISender sender, ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayRewardId", JsonConvert.SerializeObject(arrayRewardId));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserTrophyRewardAckDelegate(ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData);
        public ReceiveUserTrophyRewardAckDelegate ReceiveUserTrophyRewardAckHandler;
        public bool ReceiveUserTrophyRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            int[] arrayRewardId = JsonConvert.DeserializeObject<int[]>(jObject["arrayRewardId"].ToString());
            MsgReward[] arrayRewardInfo = JsonConvert.DeserializeObject<MsgReward[]>(jObject["arrayRewardInfo"].ToString());
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveUserTrophyRewardAckHandler(errorCode, arrayRewardId, arrayRewardInfo, arrayQuestData);
        }
        #endregion                      

    }
}