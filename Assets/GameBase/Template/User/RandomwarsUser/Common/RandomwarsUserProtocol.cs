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

        UserRewardReq,
        UserRewardAck,

        UserTutorialEndReq,
        UserTutorialEndAck,


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
                {(int)ERandomwarsProtocol.UserRewardReq, ReceiveUserRewardReq},
                {(int)ERandomwarsProtocol.UserRewardAck, ReceiveUserRewardAck},
                {(int)ERandomwarsProtocol.UserTutorialEndReq, ReceiveUserTutorialEndReq},
                {(int)ERandomwarsProtocol.UserTutorialEndAck, ReceiveUserTutorialEndAck},           };
        }


        #region UserInfo ---------------------------------------------------------------------
        public bool UserInfoReq(ISender sender, ReceiveUserInfoAckDelegate callback)
        {
            ReceiveUserInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserInfoReq, "userinfo", json.ToString());
        }

        public delegate (ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, ItemBaseInfo[] arrayBaseItem, QuestInfo questInfo, UserSeasonInfo seasonInfo) ReceiveUserInfoReqDelegate(string accessToken);
        public ReceiveUserInfoReqDelegate ReceiveUserInfoReqHandler;
        public bool ReceiveUserInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveUserInfoReqHandler(accessToken);
            return UserInfoAck(sender, res.errorCode, res.userInfo, res.arrayUserDeck, res.arrayUserDice, res.arrayBaseItem, res.questInfo, res.seasonInfo);
        }

        public bool UserInfoAck(ISender sender, ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, ItemBaseInfo[] arrayBaseItem, QuestInfo questInfo, UserSeasonInfo seasonInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("userInfo", JsonConvert.SerializeObject(userInfo));
            json.Add("arrayUserDeck", JsonConvert.SerializeObject(arrayUserDeck));
            json.Add("arrayUserDice", JsonConvert.SerializeObject(arrayUserDice));
            json.Add("arrayBaseItem", JsonConvert.SerializeObject(arrayBaseItem));
            json.Add("questInfo", JsonConvert.SerializeObject(questInfo));
            json.Add("seasonInfo", JsonConvert.SerializeObject(seasonInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserInfoAckDelegate(ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, ItemBaseInfo[] arrayBaseItem, QuestInfo questInfo, UserSeasonInfo seasonInfo);
        public ReceiveUserInfoAckDelegate ReceiveUserInfoAckHandler;
        public bool ReceiveUserInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            MsgUserInfo userInfo = JsonConvert.DeserializeObject<MsgUserInfo>(jObject["userInfo"].ToString());
            UserDeck[] arrayUserDeck = JsonConvert.DeserializeObject<UserDeck[]>(jObject["arrayUserDeck"].ToString());
            UserDice[] arrayUserDice = JsonConvert.DeserializeObject<UserDice[]>(jObject["arrayUserDice"].ToString());
            ItemBaseInfo[] arrayBaseItem = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayBaseItem"].ToString());
            QuestInfo questInfo = JsonConvert.DeserializeObject<QuestInfo>(jObject["questInfo"].ToString());
            UserSeasonInfo seasonInfo = JsonConvert.DeserializeObject<UserSeasonInfo>(jObject["seasonInfo"].ToString());
            return ReceiveUserInfoAckHandler(errorCode, userInfo, arrayUserDeck, arrayUserDice, arrayBaseItem, questInfo, seasonInfo);
        }
        #endregion    


        #region UserReward ---------------------------------------------------------------------
        public bool UserRewardReq(ISender sender, int rewardId, int targetType, ReceiveUserRewardAckDelegate callback)
        {
            ReceiveUserRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            json.Add("targetType", targetType);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserRewardReq, "userreward", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveUserRewardReqDelegate(string accessToken, int rewardId, int targetType);
        public ReceiveUserRewardReqDelegate ReceiveUserRewardReqHandler;
        public bool ReceiveUserRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            int targetType = (int)jObject["targetType"];
            var res = ReceiveUserRewardReqHandler(accessToken, rewardId, targetType);
            return UserRewardAck(sender, res.errorCode, res.arrayRewardId, res.arrayRewardInfo, res.arrayQuestData);
        }

        public bool UserRewardAck(ISender sender, ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayRewardId", JsonConvert.SerializeObject(arrayRewardId));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserRewardAckDelegate(ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveUserRewardAckDelegate ReceiveUserRewardAckHandler;
        public bool ReceiveUserRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            int[] arrayRewardId = JsonConvert.DeserializeObject<int[]>(jObject["arrayRewardId"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveUserRewardAckHandler(errorCode, arrayRewardId, arrayRewardInfo, arrayQuestData);
        }
        #endregion                      

        #region UserTutorialEnd ---------------------------------------------------------------------
        public bool UserTutorialEndReq(ISender sender, ReceiveUserTutorialEndAckDelegate callback)
        {
            ReceiveUserTutorialEndAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserTutorialEndReq, "usertutorialend", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, bool endTutorial) ReceiveUserTutorialEndReqDelegate(string accessToken);
        public ReceiveUserTutorialEndReqDelegate ReceiveUserTutorialEndReqHandler;
        public bool ReceiveUserTutorialEndReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveUserTutorialEndReqHandler(accessToken);
            return UserTutorialEndAck(sender, res.errorCode, res.endTutorial);
        }

        public bool UserTutorialEndAck(ISender sender, ERandomwarsUserErrorCode errorCode, bool endTutorial)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("endTutorial", (bool)endTutorial);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserTutorialEndAckDelegate(ERandomwarsUserErrorCode errorCode, bool endTutorial);
        public ReceiveUserTutorialEndAckDelegate ReceiveUserTutorialEndAckHandler;
        public bool ReceiveUserTutorialEndAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            bool endTutorial = (bool)jObject["endTutorial"];
            return ReceiveUserTutorialEndAckHandler(errorCode, endTutorial);
        }
        #endregion    
    }
}